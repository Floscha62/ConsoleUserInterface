using System.Runtime.InteropServices;

namespace ConsoleUserInterface.Core {
    internal static class ConsoleUtil {

        private const uint StdOutputHandle = 0xFFFFFFF5;
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole(int pIC);
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(uint nStdHandle);
        [DllImport("kernel32.dll")]
        private static extern void SetStdHandle(uint nStdHandle, IntPtr handle);
        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr handle, out int bufferModes);
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr handle, int bufferModes);

        public static void AllocateANSIConsole() {
            if (!AllocConsole(-1) || !TryEnableAnsiCodes()) {
                throw new Exception("Could not allocate with ANSI enabled");
            }
        }

        private static bool TryEnableAnsiCodes() {
            var stdOut = GetStdHandle(StdOutputHandle);
            if (GetConsoleMode(stdOut, out var modes) && (modes & 0x0004) == 0x0004) {
                return true;
            }
            modes |= 0x0004;
            return SetConsoleMode(stdOut, modes);
        }
    }
}
