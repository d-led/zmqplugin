#pragma once

#include <msgpack.hpp>
#include <string>
#include <vector>

namespace zmq_plugin {

struct call {
    std::string name;
    std::vector<std::string> parameters;
    MSGPACK_DEFINE(name, parameters);
};
}
