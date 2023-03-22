namespace Chip8;

public interface IKeyboard {
    void OnKeyPressed(int keyCode);
    void OnKeyReleased(int keyCode);

    bool this[int index] { get; }
    int Length { get; }
}