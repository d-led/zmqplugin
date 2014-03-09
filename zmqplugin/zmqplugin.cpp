#ifdef _MSC_VER
#define ZMQ_PLUGIN __declspec(dllexport)
#else
#define ZMQ_PLUGIN
#endif

#include <picojson_serializer.h>
#include <string>
#include <iostream>
#include <zmq.hpp>
#include <thread>
#include <vector>
#include <msgpack.hpp>
#include <unordered_map>
#include <functional>
#include <numeric>
#include <exception>

namespace zmq_plugin {

    static std::unordered_map<std::string,std::function<std::string(std::vector<std::string>)>> call_map;

    struct call {
        std::string name;
        std::vector<std::string> parameters;
        MSGPACK_DEFINE(name, parameters);
    };

    ///
    struct config {
        std::string protocol;
        std::string bind_string;

        friend class picojson::convert::access;
        template<class Archive>
        void json(Archive & ar)
        {
            ar & picojson::convert::member("protocol", protocol);
            ar & picojson::convert::member("bind_string", bind_string);
        }
    };

    class plugin_server {
        zmq::context_t context;
        zmq::socket_t socket;
        bool cancelled;
        std::string connection_string;
    public:
        plugin_server(std::string const& connection):
            context(1),
            socket(context, ZMQ_PAIR),
            cancelled(false),
            connection_string(connection)
        {
            socket.bind(connection.c_str());
        }

        void reply(std::string const& rep) {
            zmq::message_t reply(rep.length());
            memcpy ((void *) reply.data (), rep.c_str(), rep.length());
            socket.send (reply);
        }

        void loop() {
            while (!cancelled) {
                zmq::message_t request;
                socket.recv (&request);
                std::string request_string(static_cast<char*>(request.data()),request.size());

                if (request_string=="hello") {
                    reply("World!");
                    continue;
                }

                msgpack::unpacked request_message;
                msgpack::unpack(&request_message,
                    request_string.data(),
                    request_string.size());

                try {
                    msgpack::object obj = request_message.get();
                    call c;
                    obj.convert(&c);

                    auto call_function = call_map.find(c.name);
                    if (call_function!=call_map.end()) {
                        reply(call_function->second(c.parameters));
                    } else {
                        throw std::runtime_error(std::string("no such function : ")
                            .append(c.name));
                    }
                } catch (std::exception& e) {
                    msgpack::sbuffer sbuf;
                    msgpack::pack(sbuf, std::string(e.what()));
                    reply(std::string(sbuf.data(),sbuf.size()));
                }
            }
        }

        ~plugin_server() {
            if (socket.connected())
                socket.disconnect(connection_string.c_str());
        }
    };

    ///
    int try_connect(config const& cfg) {
        try {
            if (cfg.protocol == "msgpack") {
                std::thread([=](){
                    plugin_server server(cfg.bind_string);
                    server.loop();
                }).detach();
                return 0;
            } else {
                return 1;
            }
        } catch (zmq::error_t e){
            return 33;
        } catch (...) {
            return 42;
        }
    }

    void initialize_dispatcher() {
        call_map["add"] =[](std::vector<std::string> parameters) {
            if (parameters.size()!=1)
                throw std::invalid_argument("add requires a list of integers");
            try {
                std::vector<int> args;
                msgpack::unpacked argv;
                msgpack::unpack(&argv,parameters.front().data(),parameters.front().size());
                argv.get().convert(&args);
                int res = std::accumulate(args.begin(),args.end(),0);
                msgpack::sbuffer sbuf;
                msgpack::pack(sbuf, res);
                return std::string(sbuf.data(),sbuf.size());
            } catch (...) {
                throw std::invalid_argument("add requires a list of integers");
            }
        };
    }
}

extern "C" ZMQ_PLUGIN int load_plugin (const char* config) {
    zmq_plugin::initialize_dispatcher();
    zmq_plugin::config cfg;
    picojson::convert::from_string(config,cfg);
    return try_connect(cfg);
}
