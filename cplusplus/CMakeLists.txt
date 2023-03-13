cmake_minimum_required(VERSION 3.10)
project(chip8 LANGUAGES CXX)

include_directories(
    "${CMAKE_CURRENT_SOURCE_DIR}/include"
)

file(GLOB_RECURSE SOURCES 
    "${CMAKE_CURRENT_SOURCE_DIR}/source/chip8/*.cpp"
)

file(GLOB_RECURSE HEADERS
    "${CMAKE_CURRENT_SOURCE_DIR}/include/*.h"
)

# library
add_library(${PROJECT_NAME}_lib ${SOURCES} ${HEADERS})

set_property(TARGET ${PROJECT_NAME}_lib PROPERTY CXX_STANDARD 17)
set_property(TARGET ${PROJECT_NAME}_lib PROPERTY CXX_STANDARD_REQUIRED ON)

# main executable
add_executable(${PROJECT_NAME} 
    "${CMAKE_CURRENT_SOURCE_DIR}/source/main.cpp"
)
target_link_libraries(${PROJECT_NAME} PRIVATE ${PROJECT_NAME}_lib SDL2)

set_property(TARGET ${PROJECT_NAME} PROPERTY CXX_STANDARD 17)
set_property(TARGET ${PROJECT_NAME} PROPERTY CXX_STANDARD_REQUIRED ON)

# enable testing functionality
enable_testing()

# add tests
add_subdirectory(tests)
