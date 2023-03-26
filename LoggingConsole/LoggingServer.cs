using System.IO.Pipes;

namespace LoggingConsole {
    internal class LoggingServer {
        readonly bool ansiEnabled;
        readonly string loggerKey;
        readonly List<LoggingPackage> packages;

        internal LoggingServer(bool ansiEnabled, string loggerKey) {
            this.ansiEnabled = ansiEnabled;
            this.loggerKey = loggerKey;
            this.packages = new();
        }


        internal Task StartServer() => Task.Factory.StartNew(() => {
            using var server = new NamedPipeServerStream($"logging\\{loggerKey}");
            server.WaitForConnection();
            HandleInput();
            StartRepaintLoop();

            using var reader = new BinaryReader(server, System.Text.Encoding.Unicode);
            LoggingPackage? line;
            while ((line = LoggingPackage.Read(reader)) != null) {
                packages.Add(line);
                QueueRepaint();
            }
        });

        private bool typeInputOpen = false;
        private string filteredType = "";
        internal Task HandleInput() => Task.Factory.StartNew(() => {
            while(true) {
                var key = Console.ReadKey(true);
                if (HandleTypeInput(key)) QueueRepaint();
            }
        });
        private bool HandleTypeInput(ConsoleKeyInfo info) {
            if(!typeInputOpen && info.Key == ConsoleKey.T) {
                typeInputOpen = true;
                return true;
            }
            if (!typeInputOpen) return false;

            if (info.Key == ConsoleKey.Escape) {
                typeInputOpen = false;
                filteredType = "";
                return true;
            }

            if(info.Key == ConsoleKey.Enter) {
                typeInputOpen = false;
                return true;
            }

            if(info.Key == ConsoleKey.Backspace && filteredType != "") {
                filteredType = filteredType[..^1];
                return true;
            }

            if (char.IsLetterOrDigit(info.KeyChar) || char.IsWhiteSpace(info.KeyChar) || char.IsPunctuation(info.KeyChar)) {
                filteredType = $"{filteredType}{info.KeyChar}";
                return true;
            }
            return false;
        }

        private Layer last;
        private readonly object queueLock = new();
        private RepaintState? queuedRepaint;
        private record RepaintState(List<LoggingPackage> Packages, bool TypeInputOpen, string FilteredType);

        private void QueueRepaint() {
            lock (queueLock) {
                queuedRepaint = new(new(packages), typeInputOpen, filteredType);
            }
        }

        private Task StartRepaintLoop() => Task.Factory.StartNew(() => {
            while (true) {
                RepaintState? state;
                lock (queueLock) {
                    state = queuedRepaint;
                    queuedRepaint = null;
                }
                if (state != null) {
                    Repaint(state);
                }
            }
        });

        private void Repaint(RepaintState state) {
            var loggingLayer = new Layer(Console.WindowWidth, Console.WindowHeight);
            var controlLayer = new Layer(Console.WindowWidth, Console.WindowHeight);

            var filtered = Filter(state);
            for (int i = 0; i < filtered.Length; i++) {
                var row = i - filtered.Length + Console.WindowHeight;
                loggingLayer.Write(CreateMessage(filtered[i]), 0, row, Console.WindowWidth, 1);
            }

            if(state.TypeInputOpen) {
                var length = Math.Max(state.FilteredType.Length, "Type Filter".Length) + 5;
                controlLayer.Write(new string('#', length), 5, 5, length, 1);
                controlLayer.Write($"# {new string(' ', length - 4)} #", 5, 6, length, 1);
                controlLayer.Write($"# {"Type Filter:".PadRight(length - 4)} #", 5, 7, length, 1);
                controlLayer.Write($"# {new string(' ', length - 4)} #", 5, 8, length, 1);
                controlLayer.Write($"# {$"{state.FilteredType}|".PadRight(length - 4)} #", 5, 9, length, 1);
                controlLayer.Write($"# {new string(' ', length - 4)} #", 5, 10, length, 1);
                controlLayer.Write(new string('#', length), 5, 11, length, 1);
            }

            var merged = loggingLayer.MergeUp(controlLayer);

            merged.PrintToConsole(last, true);
            last = merged;
        }

        private LoggingPackage[] Filter(RepaintState state) =>
            state.Packages.Where(p => p.Type.Contains(state.FilteredType)).ToArray();

        private string CreateMessage(LoggingPackage package) =>
            $"[{LevelIndicator(package.Level)}][{new DateTime(package.Timestamp):dd-MM-yyyy; HH:mm:ss.fff}][{package.Type}] {package.Message.Replace("\n", "")}";

        private string LevelIndicator(int level) => (level, ansiEnabled) switch {
            (0, true) => "\x1b[38;2;150;150;150mDEBUG\x1b[0m",
            (0, false) => "DEBUG",
            (1, true) => "\x1b[38;2;50;50;200mINFO\x1b[0m",
            (1, false) => "INFO",
            (2, true) => "\x1b[38;2;200;200;50mWARN\x1b[0m",
            (2, false) => "WARN",
            (_, true) => "\x1b[38;2;200;50;50mERROR\x1b[0m",
            (_, false) => "ERROR"
        };
    }
}
