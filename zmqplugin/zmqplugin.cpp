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

namespace zmq_plugin {

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

		void loop() {
			while (!cancelled) {
				zmq::message_t request;
				socket.recv (&request);
				std::string request_string(static_cast<char*>(request.data()),request.size());
				std::string rep = (request_string=="hello") ?
					"World!" : request_string;
				zmq::message_t reply(rep.length());
				memcpy ((void *) reply.data (), rep.c_str(), rep.length());
				socket.send (reply);
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
}

extern "C" ZMQ_PLUGIN int load_plugin (const char* config) {
	zmq_plugin::config cfg;
	picojson::convert::from_string(config,cfg);
	return try_connect(cfg);
}
