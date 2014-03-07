#ifdef _MSC_VER
#define ZMQ_PLUGIN __declspec(dllexport)
#else
#define ZMQ_PLUGIN
#endif

#include <picojson_serializer.h>
#include <string>
#include <iostream>

namespace zmq_plugin {

	///
	struct config {
		std::string protocol;

		friend class picojson::convert::access;
		template<class Archive>
		void json(Archive & ar)
		{
			ar & picojson::convert::member("protocol", protocol);
		}
	};

	///
	int try_connect(config const& cfg) {
		if (cfg.protocol == "msgpack") {
			return 0;
		} else {
			return 1;
		}
		return 42;
	}
}

extern "C" ZMQ_PLUGIN int load_plugin (const char* config) {
	zmq_plugin::config cfg;
	picojson::convert::from_string(config,cfg);
	return try_connect(cfg);
}
