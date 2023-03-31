﻿using System.Diagnostics;
using System.IO.Pipes;

namespace LoggingConsole {

    public static class LoggingFactory {

        private static readonly string path = "Test.log";
#if DEBUG
        private static readonly int level = 0;
#else
        private static readonly int level = 1;
#endif

        static LoggingFactory() {
            System.IO.File.WriteAllText(path, null);
        }

        public static ILogger Create(Type type) => Console(type, path, level);

        private static ILogger File(Type type, string path, int level) => new ILogger.FileLogger(type, path, level);

        private static ILogger Console(Type type, string window, int level) => new ILogger.ConsoleLogger(type, window, level);
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
                File.Delete(path);
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

        class ConsoleLogger : ILogger {

            readonly static Dictionary<string, NamedPipeClientStream> consoles = new();
            readonly static Dictionary<NamedPipeClientStream, int> references = new();

            readonly Type type;
            readonly string consoleKey;
            readonly int level;

            public ConsoleLogger(Type type, string consoleKey, int level) {
                this.type = type;
                this.consoleKey = consoleKey;
                this.level = level;

                if (consoles.TryGetValue(consoleKey, out var console)) {
                    references[console]++;
                } else {
                    var newConsole = new Process() {
                        StartInfo = new() {
                            FileName = Path.Combine("LoggingConsole.exe"),
                            UseShellExecute = false,
                            WindowStyle = ProcessWindowStyle.Normal,
                            Arguments = consoleKey
                        }
                    };
                    newConsole.Start();
                    var pipe = new NamedPipeClientStream(".",$"logging\\{consoleKey}", PipeDirection.Out);
                    pipe.Connect();
                    consoles[consoleKey] = pipe;
                    references[pipe] = 1;
                }
            }

            public void Dispose() {
                var console = consoles[consoleKey];
                var refCount = --references[console];
                if (refCount > 0) return;

                references.Remove(console);
                consoles.Remove(consoleKey);
                console.Dispose();
            }

            public void Log(string @string, int level) {
                if (level < this.level) return;
                LogAsync(@string, level);
            }

            private Task LogAsync(string @string, int level) => Task.Run(() => {
                lock (consoleKey) {
                    using var writer = new BinaryWriter(consoles[consoleKey], System.Text.Encoding.Unicode, true);

                    var loggingPackage = new LoggingPackage(level, type.Name, @string, DateTime.Now.Ticks);

                    loggingPackage.Write(writer);
                }
            });
        }
    }
}
