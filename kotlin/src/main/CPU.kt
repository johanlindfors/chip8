package se.programmeramera.chip8

import java.util.Stack

class CPU {
    val stack: Stack<Short> = Stack()

    fun doWork() {
        stack.push(0x0001);
        stack.push(0x0002);
        stack.push(0x0003);

        for (num in stack) {
            println(num)
        }
    }
}
