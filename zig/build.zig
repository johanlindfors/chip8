const std = @import("std");

pub fn build(b: *std.build.Builder) void {
    const mode = b.standardReleaseOptions();
    const exe = b.addExecutable("hello-gamedev", "src/main.zig");
    exe.setBuildMode(mode);

    exe.addIncludePath("/usr/local/include/SDL2");
    exe.addLibraryPath("/usr/local/lib");
    exe.linkSystemLibrary("sdl2");
    exe.linkLibC();
    exe.install();
}