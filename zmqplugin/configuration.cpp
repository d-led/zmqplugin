#include "configuration.h"
#include "call_dispatcher.h"

#include <vector>
#include <msgpack.hpp>
#include <numeric>
#include <chrono>
#include <iostream>
#include <stdexcept>

namespace zmq_plugin {

void configure(call_dispatcher &dispatcher) {
    dispatcher.configure("add", [](std::vector<std::string> const &parameters) {
        if (parameters.size() != 1)
            throw std::invalid_argument("add requires a list of integers");
        try {
            std::vector<int> args;
            msgpack::unpacked argv;
            msgpack::unpack(&argv, parameters.front().data(), parameters.front().size());
            argv.get().convert(&args);
            int res = std::accumulate(args.begin(), args.end(), 0);
            msgpack::sbuffer sbuf;
            msgpack::pack(sbuf, res);
            return std::string(sbuf.data(), sbuf.size());
        }
        catch (...) {
            throw std::invalid_argument("add requires a list of integers");
        }
    });

    dispatcher.configure("sort_max", [](std::vector<std::string> const &parameters) {
        if (parameters.size() != 1)
            throw std::invalid_argument("add requires a list of integers");
        try {
            auto start = std::chrono::high_resolution_clock::now();
            // unpack
            msgpack::unpacked argv;
            msgpack::unpack(&argv, parameters.front().data(), parameters.front().size());
            std::vector<int> numbers;
            argv.get().convert(&numbers);

            // calculate
            std::sort(numbers.begin(), numbers.end());
            int res = numbers.back();

            // pack
            msgpack::sbuffer sbuf;
            msgpack::pack(sbuf, res);

            auto end = std::chrono::high_resolution_clock::now();
            std::cout << std::chrono::duration_cast<std::chrono::milliseconds>(end - start).count()
                      << " ms (unpack/calculate/pack)" << std::endl;
            return std::string(sbuf.data(), sbuf.size());
        }
        catch (...) {
            throw std::invalid_argument("sort_max requires a list of integers");
        }
    });
}
}