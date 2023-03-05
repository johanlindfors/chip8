cmake_minimum_required(VERSION 3.10)
project(chip8)

include_directories(
    "${CMAKE_CURRENT_SOURCE_DIR}/include"
)

file(GLOB SOURCES 
    "${CMAKE_CURRENT_SOURCE_DIR}/source/*.cpp"
    "${CMAKE_CURRENT_SOURCE_DIR}/source/chip8/*.cpp"
)

file(GLOB HEADERS
    "${CMAKE_CURRENT_SOURCE_DIR}/include/*.h"
)

find_package(PkgConfig REQUIRED)
pkg_search_module(GLFW REQUIRED glfw3)

add_executable(${PROJECT_NAME} ${SOURCES} ${HEADERS})
target_link_libraries(${PROJECT_NAME} SDL2)

set_property(TARGET ${project_name} PROPERTY CXX_STANDARD 17)
set_property(TARGET ${project_name} PROPERTY CXX_STANDARD_REQUIRED ON)
