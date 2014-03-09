#include "call_dispatcher.h"

namespace zmq_plugin {

void call_dispatcher::configure(std::string const &key, wrapped_call call) {
    calls[key] = call;
}

std::string call_dispatcher::try_dispatch(std::string const &key,
                                          std::vector<std::string> const &parameters) {
    auto call_function = calls.find(key);
    if (call_function != calls.end()) {
        return call_function->second(parameters);
    } else {
        throw std::runtime_error(std::string("no such function : ").append(key));
    }
}
}