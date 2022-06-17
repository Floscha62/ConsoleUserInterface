using System;

namespace ConsoleUserInterface.Core {
    public interface IConsole {
        bool CursorVisible { get; set; }
        int WindowWidth { get; }
        int WindowHeight { get; }
        int BufferHeight { get; set; }
        void SetCursorPosition(int column, int row);
        void Write(string v);
        ConsoleKeyInfo ReadKey(bool v);
    }
}
