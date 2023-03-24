using System;

namespace ConsoleUserInterface.Core {

    public static class LoggingFactory {

        private static readonly string path = "Test.log";
        private static readonly int level = 0;

        static LoggingFactory() {
            System.IO.File.WriteAllText(path, null);
        }

        public static ILogger Create(Type type) => File(type, path, level);

        private static ILogger File(Type type, string path, int level) => new ILogger.FileLogger(type, path, level);

    }

    public interface ILogger : IDisposable {

        void Log(string @string, int level);
        void Debug(string @string) {
            Log(@string, 0);
        }
        void Info(string @string) {
            Log(@string, 1);
        }
        void Warn(string @string) {
            Log(@string, 2);
        }
        void Error(string @string) {
            Log(@string, 3);
        }


        class FileLogger : ILogger {

            readonly Type type;
            readonly string path;
            readonly int level;

            public FileLogger(Type type, string path, int level) {
                this.type = type;
                this.path = path;
                this.level = level;
            }

            public void Dispose() {
                System.IO.File.Delete(path);
            }

            public void Log(string @string, int level) {
                if (level < this.level) return;
                System.IO.File.AppendAllText(path, $"[{LevelIndicator(level)}][{type.Name}][{DateTime.Now:dd-MM-yyyy; HH:mm:ss.ffff}] {@string}\n");
            }

            private static string LevelIndicator(int level) => level switch {
                0 => "\x1b[38;2;150;150;150mDEBUG\x1b[0m",
                1 => "\x1b[38;2;50;50;200mINFO\x1b[0m",
                2 => "\x1b[38;2;200;200;50mWARN\x1b[0m",
                _ => "\x1b[38;2;200;50;50mERROR\x1b[0m"
            };
        }
    }
}
