using System;

namespace ConsoleUserInterface.Core
{
    internal class DefaultConsole : IConsole
    {
        public bool CursorVisible { get => Console.CursorVisible; set => Console.CursorVisible = value; }

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
