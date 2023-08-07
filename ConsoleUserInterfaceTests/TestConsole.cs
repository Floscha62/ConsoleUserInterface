using ConsoleUserInterface.Core;

namespace ConsoleUserInterfaceTests {
    internal class TestConsole : IConsole {
        public bool CursorVisible { get; set; }
        public int WindowWidth { get; }
        public int WindowHeight { get; }
        public int BufferHeight { get; set; }
        public string Title { get; set; }

        public ConsoleKeyInfo? NextKey { get; set; }
        public ConsoleKeyInfo ReadKey(bool intercept) {
            if (NextKey == null) throw new Exception("Unexpected ReadKey Call");

            return NextKey.Value;
        }

        public TestConsole(int width, int height) {
            this.WindowHeight = height;
            this.WindowWidth = width;
            screen = new (new string('\0', WindowWidth * WindowHeight).ToCharArray());
        }

        Memory<char> screen;
        int cursorPosition = 0;

        public void SetCursorPosition(int column, int row) {
            cursorPosition = column + row * WindowWidth;
        }

        public void Clear() {
            screen = new(new string('\0', WindowWidth * WindowHeight).ToCharArray());
        }

        public void Write(string v) {
            if (v.StartsWith('\x1b')) return;

            v.AsMemory().CopyTo(screen.Slice(cursorPosition, v.Length));
        }
    }
}
