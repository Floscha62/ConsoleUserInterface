namespace ConsoleUserInterface.Core {
    internal class DefaultConsole : IConsole {
        internal DefaultConsole() {
            ConsoleUtil.AllocateANSIConsole();
        }

        public bool CursorVisible { set => Console.CursorVisible = value; }

        public int WindowWidth => Console.WindowWidth;

        public int WindowHeight => Console.WindowHeight;

        public int BufferHeight { get => Console.BufferHeight; set => Console.BufferHeight = value; }

        public string Title { set => Console.Title = value; }

        public ConsoleKeyInfo ReadKey(bool intercept) => Console.ReadKey(intercept);

        public void SetCursorPosition(int column, int row) {
            Console.SetCursorPosition(column, row);
        }

        public void Write(string v) {
            Console.Write(v);
        }
    }
}
