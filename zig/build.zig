const std = @import("std");

pub fn build(b: *std.Build) void {
    const optimize = b.standardOptimizeOption(.{});
    const target = b.standardTargetOptions(.{});
    
    const exe = b.addExecutable(.{
        .name = "chip8",
        .root_source_file = .{ .path = "src/main.zig" },
        .optimize = optimize,
        .target = target
    });

    exe.addIncludePath(.{ .path = "/usr/local/include/SDL2"});
    exe.addLibraryPath(.{ .path = "/usr/local/lib"});
    exe.linkSystemLibrary("sdl2");
    exe.linkLibC();
    
    const install_exe = b.addInstallArtifact(exe, .{});
    b.getInstallStep().dependOn(&install_exe.step); 
}