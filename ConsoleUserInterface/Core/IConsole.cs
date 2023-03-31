namespace ConsoleUserInterface.Core {

    /// <summary>
    /// The interface to encapsulate console behaviour.
    /// </summary>
    public interface IConsole {
        /// <summary> Is the cursor visible? </summary>
        bool CursorVisible { set; }
        /// <summary> The window width. </summary>
        int WindowWidth { get; }
        /// <summary> The window height. </summary>
        int WindowHeight { get; }
        /// <summary> The buffer height. </summary>
        int BufferHeight { get; set; }
        /// <summary> The window title. </summary>
        string Title { set; }

        /// <summary>
        /// Sets the cursor position.
        /// </summary>
        /// <param name="column"> The column to place the cursor. </param>
        /// <param name="row"> The row to place the cursor. </param>
        void SetCursorPosition(int column, int row);

        /// <summary> Write the given string beginning at the current cursor position. </summary>
        /// <param name="v"> The value to write. </param>
        void Write(string v);

        /// <summary>
        /// Blocks the execution to get the next key.
        /// </summary>
        /// <param name="intercept"><c>true</c> to intercept the key press; false to display.</param>
        /// <returns>The info corresponding to the pressed key. </returns>
        ConsoleKeyInfo ReadKey(bool intercept);
    }
}
