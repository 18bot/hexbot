cmake_minimum_required(VERSION 3.1)
project(hexbot)

set(CMAKE_MACOSX_RPATH 1)
set(CMAKE_CXX_STANDARD 11)
set(JSONCPP_WITH_TESTS "OFF" CACHE STRING "Do not need that.")

file(GLOB HEXBOT_SRC
    "*.h"
    "*.cpp"
)

add_compile_options(-std=c++11)
include_directories(hexbot "../external/jsoncpp/include")
add_subdirectory("${CMAKE_CURRENT_SOURCE_DIR}/../external/jsoncpp" "jsoncpp")

if(APPLE)
	set(MACOSX_BUNDLE_BUNDLE_NAME hexbot)
	add_library(hexbot MODULE ${HEXBOT_SRC})
	
	set_target_properties(hexbot PROPERTIES 
		BUNDLE TRUE
		LIBRARY_OUTPUT_DIRECTORY "${CMAKE_CURRENT_SOURCE_DIR}/../../unity/builds/mac/hexbot.app/Contents/Plugins"
		LIBRARY_OUTPUT_DIRECTORY_DEBUG "${CMAKE_CURRENT_SOURCE_DIR}/../../unity/builds/mac/hexbot.app/Contents/Plugins"
		LIBRARY_OUTPUT_DIRECTORY_RELEASE "${CMAKE_CURRENT_SOURCE_DIR}/../../unity/builds/mac/hexbot.app/Contents/Plugins"
	)

else()
	add_library(hexbot SHARED ${HEXBOT_SRC})
endif()

target_link_libraries(hexbot jsoncpp_lib_static)


