namespace ConsoleUserInterface.Core {
    /// <summary>
    /// The container for the relevant information for rendering a <see cref="BaseComponent{Props, State}"/>.
    /// </summary>
    /// <param name="Text">The text to be rendered for the component.</param>
    /// <param name="Underlined">The text should be rendered underlined.</param>
    public record BaseRenderResult(string Text, bool Underlined = false);
}
