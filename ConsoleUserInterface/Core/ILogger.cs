

using System;

namespace ConsoleUserInterface.Core {
    public interface ILogger {
        void LogString(string @string);

        public static ILogger File(string path) => new FileLogger(path);

        class FileLogger : ILogger {

            readonly string path;

            public FileLogger(string path) {
                this.path = path;
            }

            public void LogString(string @string) {
                System.IO.File.AppendAllText(path, $"[{DateTime.Now:dd-MM-yyyy; HH:mm}] {@string}");
            }
        }
    }
}
