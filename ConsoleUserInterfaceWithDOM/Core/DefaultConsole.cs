namespace ConsoleUserInterfaceWithDOM.Core {
    internal class DefaultConsole : IConsole
    {
        internal DefaultConsole() {
            ConsoleUtil.AllocateANSIConsole();
        }

        public bool CursorVisible { set => Console.CursorVisible = value; }

        public int WindowWidth => Console.WindowWidth;

        public int WindowHeight => Console.WindowHeight;

        public int BufferHeight { get => Console.BufferHeight; set => Console.BufferHeight = value; }


        public ConsoleKeyInfo ReadKey(bool v) => Console.ReadKey(v);

        public void SetCursorPosition(int column, int row)
        {
            Console.SetCursorPosition(column, row);
        }

        public void Write(string v)
        {
            Console.Write(v);
        }
    }
}
