#ifdef _MSC_VER
#define ZMQ_PLUGIN __declspec(dllexport)
#else
#define ZMQ_PLUGIN
#endif

#include "call_dispatcher.h"
#include "configuration.h"
#include "call.h"
#include "plugin_server.h"

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
#include <chrono>

namespace zmq_plugin {

static call_dispatcher dispatcher;

///
int try_connect(config const &cfg) {
    try {
        if (cfg.protocol == "msgpack") {
            std::thread([=]() {
                            plugin_server server(cfg.bind_string,dispatcher);
                            server.loop();
                        }).detach();
            return 0;
        } else {
            return 1;
        }
    }
    catch (zmq::error_t e) {
        return 33;
    }
    catch (...) {
        return 42;
    }
}

void benchmark() {
    size_t count = 10000000;
    std::cout << "benchmarking native [" << count << "]..." << std::endl;
    std::vector<int> numbers(count);
    std::generate(numbers.begin(), numbers.end(), []() {
        static int i = 1;
        return i++;
    });
    std::reverse(numbers.begin(), numbers.end());
    std::cout << "Data size: " << (sizeof(numbers) + count * sizeof(int)) / 1048576. << " MB"
              << std::endl;
    for (int i = 0; i < 3; i++) {
        auto start = std::chrono::high_resolution_clock::now();

        // calculate
        std::sort(numbers.begin(), numbers.end());
        int res = numbers.back();

        auto end = std::chrono::high_resolution_clock::now();
        std::cout << std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count()
                  << " ms" << std::endl;
    }
}
}

extern "C" ZMQ_PLUGIN int load_plugin(const char *config) {
    zmq_plugin::benchmark();
    zmq_plugin::configure(zmq_plugin::dispatcher);
    zmq_plugin::config cfg;
    picojson::convert::from_string(config, cfg);
    return try_connect(cfg);
}
