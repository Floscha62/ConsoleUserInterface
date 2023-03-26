using System;

namespace ConsoleUserInterfaceWithDOM.Core.Extensions {
    public static class Destructors {

        public static void Deconstruct(this ConsoleKeyInfo info, out ConsoleKey key, out char character, out ConsoleModifiers flags) =>
            (key, character, flags) = (info.Key, info.KeyChar, info.Modifiers);
    }
}
