namespace ConsoleUserInterface.Core.Extensions {

    /// <summary>
    /// Provides useful deconstruct methods, for implementing components.
    /// </summary>
    public static class Deconstructors {

        /// <summary>
        /// Deconstructs the <see cref="ConsoleKeyInfo"/>.
        /// </summary>
        /// <param name="info"> The info to be constructed. </param>
        /// <param name="key"> The key pressed. </param>
        /// <param name="character"> The character corresponding the key press. </param>
        /// <param name="flags"> The flags describing use of control, alt and shift. </param>
        public static void Deconstruct(this ConsoleKeyInfo info, out ConsoleKey key, out char character, out ConsoleModifiers flags) =>
            (key, character, flags) = (info.Key, info.KeyChar, info.Modifiers);
    }
}
