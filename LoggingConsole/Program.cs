using System.Runtime.InteropServices;

namespace LoggingConsole {
    public class Program {

        public static Task Main(string[] args) {
            var ansiEnabled = ConsoleUtil.TryEnableAnsiCodes();
            Console.Title = args[0];
            var server = new LoggingServer(ansiEnabled, args[0]);
            server.HandleInput();
            return server.StartServer();
        }

    }
    internal static class ConsoleUtil {

        private const uint StdOutputHandle = 0xFFFFFFF5;
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(uint nStdHandle);
        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr handle, out int bufferModes);
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr handle, int bufferModes);

        public static bool TryEnableAnsiCodes() {
            var stdOut = GetStdHandle(StdOutputHandle);
            if (GetConsoleMode(stdOut, out var modes) && (modes & 0x0004) == 0x0004) {
                return true;
            }
            modes |= 0x0004;
            return SetConsoleMode(stdOut, modes);
        }
    }

}