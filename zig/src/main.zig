const Emulator = @import("emulator.zig").Emulator;

pub fn main() anyerror!void {
    var emulator = Emulator.init();
    emulator.run();
}
