#pragma once

#include <zmq.hpp>
#include <string>

namespace zmq_plugin {

    class call_dispatcher;

class plugin_server {
    zmq::context_t context;
    zmq::socket_t socket;
    bool cancelled;
    std::string connection_string;
    call_dispatcher& dispatcher;

  public:
    plugin_server(std::string const &connection,call_dispatcher& d);

    void reply(std::string const &rep);

    void loop();

    ~plugin_server();
};

}
