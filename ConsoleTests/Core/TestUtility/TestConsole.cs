using ConsoleUserInterface.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTests.Core.TestUtility {
    public class TestConsole : IConsole {
        public bool CursorVisible { get => false; set { } }

        public int WindowWidth { get; }

        public int WindowHeight { get; }

        public int BufferHeight { get; set; } = 0;

        private List<ConsoleKeyInfo> keys;
        private List<string> lines;

        public TestConsole(int windowWidth, int windowHeight) {
            this.WindowWidth = windowWidth;
            this.WindowHeight = windowHeight;
            this.keys = new List<ConsoleKeyInfo>();
            this.lines = new List<string>();
        }

        public void AddKeyInput(ConsoleKeyInfo key) {
            keys.Add(key);
        }

        public void ResetLines() {
            this.lines = new List<string>();
        }

        public void ShouldDisplay(params string[] display) {
            var actual = string.Join("", lines);
            var expected = string.Join("", display.Select(s => $"\x1b[0m{s}"));
            Assert.AreEqual(expected, actual);
            ResetLines();
        }

        public ConsoleKeyInfo ReadKey(bool v) {
            var k = keys[0];
            keys.RemoveAt(0);
            return k;
        }

        public void SetCursorPosition(int column, int row) { }

        public void Write(string v) {
            lines.Add(v);
        }

        public void AddStringInput(string v) {
            keys.AddRange(v.Select(c =>
            new ConsoleKeyInfo(c,
                char.IsLower(c) ?
                Enum.Parse<ConsoleKey>(c.ToString(), true) :
                ConsoleKey.Spacebar, false, false, false)));
        }
    }
}
