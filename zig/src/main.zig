const std = @import("std");
const os = std.os;

const Emulator = @import("emulator.zig").Emulator;

pub fn main() anyerror!void {
    var gpalloc = std.heap.GeneralPurposeAllocator(.{}){};
    defer std.debug.assert(gpalloc.deinit() == .ok);
    const allocator = gpalloc.allocator();

    const args = try std.process.argsAlloc(allocator);
    defer std.process.argsFree(allocator, args);
    if (args.len != 2) {
        std.debug.print("Usage: {s} ROM_FILE\n", .{args[0]});
        return;
    }
    const fileName = args[1];

    const file = try std.fs.cwd().openFile(fileName, .{ .mode = .read_only });
    defer file.close();

    var buffer: [1024 * 4]u8 = undefined;
    const bytes_read = try file.read(buffer[0..buffer.len]);
    std.debug.print("Bytes read {}", .{bytes_read});

    var emulator = Emulator.init();
    emulator.run(&buffer, bytes_read);
}
