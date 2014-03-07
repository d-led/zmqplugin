#ifdef _MSC_VER
#define ZMQ_PLUGIN __declspec(dllexport)
#else
#define ZMQ_PLUGIN
#endif

extern "C" ZMQ_PLUGIN int load_plugin (const char* config) {
	return 42;
}
