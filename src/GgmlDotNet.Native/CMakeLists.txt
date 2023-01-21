cmake_minimum_required(VERSION 3.13.0)
cmake_policy(SET CMP0053 NEW)
cmake_policy(SET CMP0054 NEW)

set(PROJ_NAME GgmlDotNetNative)
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

project(${PROJ_NAME} VERSION 1.0.0.0)

# OS info
message("-------------------------------------------------------")
message("-- CMAKE_SYSTEM_INFO_FILE: ${CMAKE_SYSTEM_INFO_FILE}")
message("-- CMAKE_SYSTEM_NAME:      ${CMAKE_SYSTEM_NAME}")
message("-- CMAKE_SYSTEM_PROCESSOR: ${CMAKE_SYSTEM_PROCESSOR}")
message("-- CMAKE_SYSTEM:           ${CMAKE_SYSTEM}")
message("-- CMAKE_C_COMPILER:       ${CMAKE_C_COMPILER}")
message("-- CMAKE_CXX_COMPILER:     ${CMAKE_CXX_COMPILER}")
string (REGEX MATCH "\\.el[1-9]" os_version_suffix ${CMAKE_SYSTEM})
message("-- os_version_suffix:      ${os_version_suffix}")
message("-- Project: ${CMAKE_CURRENT_SOURCE_DIR}")
message("-- Binaries: ${CMAKE_CURRENT_BINARY_DIR}")
message("-------------------------------------------------------")

# Version info
set(VERSION_MAJOR 1)
set(VERSION_MINOR 0)
set(VERSION_PATCH 0)
set(VERSION_DATE 0)

# Binary is not be able to be executed in host, set 0 (success) forcibly
set(THREADS_PTHREAD_ARG "0" CACHE STRING "Forcibly set by CMakeLists.txt." FORCE)

# Only GCC requires -fPIC
if ("${CMAKE_CXX_COMPILER_ID}" STREQUAL "GNU")
	set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -fPIC")
	set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -fPIC")
endif()

if ("${CMAKE_SYSTEM_NAME}" STREQUAL "Android")
	set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -fPIC")
    # use rtti to use dynamic_cast
	set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -fPIC")
endif()

# set architecture
if (NOT ${CMAKE_GENERATOR} MATCHES "Visual Studio")
    if ("${ARCH_TYPE}" STREQUAL "32")
        set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -m32")
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -m32")
    elseif ("${ARCH_TYPE}" STREQUAL "64")
        set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -m64")
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -m64")
    endif()
endif()

# Select the release build type by default
if (NOT CMAKE_BUILD_TYPE)
    set(CMAKE_BUILD_TYPE "Release" CACHE STRING "" FORCE)
    message("-- CMAKE_BUILD_TYPE: ${CMAKE_BUILD_TYPE}")
endif()

# get root directory path
get_filename_component(GGMLDOTNET_ROOT_DIR "../.." ABSOLUTE)

if (MSVC OR MSYS OR MINGW)
    include_directories("${ggml_SRC_DIR}/include")
elseif("${CMAKE_SYSTEM_NAME}" STREQUAL "iOS")
    include_directories("${ggml_SRC_DIR}/include")
elseif(APPLE)
    include_directories("${ggml_SRC_DIR}/include")
elseif("${CMAKE_SYSTEM_NAME}" STREQUAL "Android")
    include_directories("${ggml_SRC_DIR}/include")
elseif(UNIX AND NOT APPLE)
    include_directories("${ggml_SRC_DIR}/include")
else()
    message(FATAL_ERROR "Failed to include directories")
endif()

# Set /bigobj for MSVC
if (${CMAKE_GENERATOR} MATCHES "Visual Studio")
	# C4819: The file contains a character that cannot be represented in the current code page
	# C4503: identifier' : decorated name length exceeded, name was truncated
	set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /wd4819 /wd4503 /bigobj")

	# C4146: unary minus operator applied to unsigned type, result still unsigned
	if ("${CMAKE_SYSTEM_NAME}" STREQUAL "WindowsStore")
		set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /wd4146 /wd4996 /ZW /EHsc")
	endif()
endif()

# set parallel build
if (${CMAKE_GENERATOR} MATCHES "Visual Studio")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /MP")
endif()

# create config file
configure_file(${PROJECT_SOURCE_DIR}/ggml/config.hpp.in
               ${PROJECT_SOURCE_DIR}/ggml/config.hpp
               @ONLY)

FILE(GLOB_RECURSE HEADERS ${PROJECT_SOURCE_DIR}/ggml/*.hpp)
FILE(GLOB_RECURSE SOURCES ${PROJECT_SOURCE_DIR}/ggml/*.cpp)

configure_file(${CMAKE_CURRENT_SOURCE_DIR}/version.rc.in
               ${CMAKE_CURRENT_BINARY_DIR}/version.rc
               @ONLY)

# STATIC_LIBRARIES
if (MSVC OR MSYS OR MINGW)
    if("${CMAKE_SYSTEM_NAME}" STREQUAL "WindowsStore")
        set(STATIC_LIBRARIES
            ${ggml_DIR}/lib/static/ggml.lib
        )
    else()
        set(STATIC_LIBRARIES
            ${ggml_DIR}/lib/static/ggml.lib
        )
    endif()
elseif("${CMAKE_SYSTEM_NAME}" STREQUAL "iOS")
    set(STATIC_LIBRARIES
        ${ggml_DIR}/lib/static/libggml.a
        "-framework Accelerate"
    )
elseif(APPLE)
    set(STATIC_LIBRARIES
        ${ggml_DIR}/lib/static/libggml.a
        "-framework Accelerate"
    )
elseif("${CMAKE_SYSTEM_NAME}" STREQUAL "Android")
    set(STATIC_LIBRARIES
        ${ggml_DIR}/lib/static/libggml.a
        z
        log
        android
    )
elseif(UNIX AND NOT APPLE)
    # centos7 uses glibc-2.17-292.el7.x86_64 but build dev uses latest GCC
    # Therefore, link libmvec as static
    # https://sourceware.org/glibc/wiki/libmvec
    # But it is not linked. why?
    set(STATIC_LIBRARIES
        ${ggml_DIR}/lib/static/libggml.a
        pthread
        gcc_s
        gcc
    )
else()
    message(FATAL_ERROR "Failed to link library")
endif()

if ("${CMAKE_SYSTEM_NAME}" STREQUAL "iOS")
    add_library(${PROJ_NAME} ${HEADERS}
                             ${SOURCES}
                             ${CMAKE_CURRENT_BINARY_DIR}/version.rc)
else ()
    add_library(${PROJ_NAME} SHARED ${HEADERS}
                                    ${SOURCES}
                                    ${CMAKE_CURRENT_BINARY_DIR}/version.rc)
endif()

target_link_libraries(${PROJ_NAME} ${STATIC_LIBRARIES})

set(CompilerFlags
    CMAKE_CXX_FLAGS
    CMAKE_CXX_FLAGS_DEBUG
    CMAKE_CXX_FLAGS_RELEASE
    CMAKE_C_FLAGS
    CMAKE_C_FLAGS_DEBUG
    CMAKE_C_FLAGS_RELEASE
)

# install
target_include_directories(${PROJ_NAME} INTERFACE
                           $<BUILD_INTERFACE:${CMAKE_CURRENT_SOURCE_DIR}/include>
                           $<INSTALL_INTERFACE:include>)
install(TARGETS ${PROJ_NAME}
        EXPORT ${PROJ_NAME}Config
        LIBRARY DESTINATION lib
)
install(DIRECTORY ${CMAKE_CURRENT_SOURCE_DIR}/ggml/
        DESTINATION include/ggmldotnet
)
install(EXPORT ${PROJ_NAME}Config
        NAMESPACE ${PROJ_NAME}::
        DESTINATION lib/cmake/ggmldotnet
)

set(CPACK_PROJECT_NAME ${PROJECT_NAME})
set(CPACK_PROJECT_VERSION ${PROJECT_VERSION})
include(CPack)