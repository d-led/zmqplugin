#pragma once

#include <unordered_map>
#include <functional>
#include <string>

namespace zmq_plugin {

class call_dispatcher {
  public:
    typedef std::function<std::string(std::vector<std::string> const &)> wrapped_call;
    typedef std::unordered_map<std::string, wrapped_call> call_map;

  public:
    void configure(std::string const &key, wrapped_call call);
    std::string try_dispatch(std::string const &key, std::vector<std::string> const &parameters);

  private:
    call_map calls;
};
}
