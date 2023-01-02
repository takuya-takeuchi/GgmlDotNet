#ifndef _CPP_CONFIG_H_
#define _CPP_CONFIG_H_

#include "export.hpp"
#include <iostream>
#include <string>
#include <sstream>

#define PROJECT_NAME  "OpenJpegDotNetNative"
#define VERSION_MAJOR "0"
#define VERSION_MINOR "0"
#define VERSION_PATCH "1"
#define VERSION_DATE  "20210417"

DLLEXPORT std::string* get_version()
{
    std::stringstream ss;
    ss << VERSION_MAJOR << "." << VERSION_MINOR << "." << VERSION_PATCH << "." << VERSION_DATE;
    return new std::string(ss.str());
}

#endif
