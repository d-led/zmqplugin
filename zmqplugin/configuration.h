#pragma once

#include <picojson_serializer.h>

namespace zmq_plugin {

struct config {
    std::string protocol;
    std::string bind_string;

    friend class picojson::convert::access;
    template <class Archive> void json(Archive &ar) {
        ar &picojson::convert::member("protocol", protocol);
        ar &picojson::convert::member("bind_string", bind_string);
    }
};

class call_dispatcher;

void configure(call_dispatcher &);
}

