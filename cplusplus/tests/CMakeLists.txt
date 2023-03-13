function(add_tests)
    foreach(test IN LISTS ARGN)
        set(TEST_NAME "${PROJECT_NAME}_${test}")
        message(STATUS "Adding ${TEST_NAME}")
        # testing binary
        add_executable(${TEST_NAME}
            "${CMAKE_CURRENT_SOURCE_DIR}/${test}.cpp"
        )
        target_link_libraries(${TEST_NAME} PRIVATE ${PROJECT_NAME}_lib SDL2)

        set_property(TARGET ${TEST_NAME} PROPERTY CXX_STANDARD 17)
        set_property(TARGET ${TEST_NAME} PROPERTY CXX_STANDARD_REQUIRED ON)

        # define tests
        add_test(
            NAME ${TEST_NAME}
            COMMAND $<TARGET_FILE:${TEST_NAME}>
        )
    endforeach()
endfunction()

# enable testing functionality
enable_testing()

add_tests(
    should_test_opcode_0x1xxx
    should_test_opcode_0x2xxx
)