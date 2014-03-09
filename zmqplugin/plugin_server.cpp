#include "plugin_server.h"
#include "call.h"
#include "call_dispatcher.h"
#include <msgpack.hpp>
#include <iostream>

namespace zmq_plugin {
    
    plugin_server::plugin_server(std::string const &connection,call_dispatcher& d)
        : context(1), socket(context, ZMQ_PAIR), cancelled(false), connection_string(connection), dispatcher(d) {
        socket.bind(connection.c_str());
    }

    void plugin_server::reply(std::string const &rep) {
        zmq::message_t reply(rep.length());
        memcpy((void *)reply.data(), rep.c_str(), rep.length());
        socket.send(reply);
    }

    void plugin_server::loop() {
        while (!cancelled) {
            zmq::message_t request;
            socket.recv(&request);
            std::string request_string(static_cast<char *>(request.data()), request.size());

            if (request_string == "hello") {
                reply("World!");
                continue;
            }

            msgpack::unpacked request_message;
            msgpack::unpack(&request_message, request_string.data(), request_string.size());

            try {
                msgpack::object obj = request_message.get();
                call c;
                obj.convert(&c);

                if (c.name == "echo") {
                    reply(request_string);
                    continue;
                }

                reply(dispatcher.try_dispatch(c.name, c.parameters));
            }
            catch (std::exception &e) {
                msgpack::sbuffer sbuf;
                msgpack::pack(sbuf, std::string(e.what()));
                reply(std::string(sbuf.data(), sbuf.size()));
            }
        }
    }

    plugin_server::~plugin_server() {
        if (socket.connected())
            socket.disconnect(connection_string.c_str());
        std::cerr << "disconnecting" << std::endl;
    }

}
