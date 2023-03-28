namespace ConsoleUserInterface.Core {
    /// <summary>
    /// The container for the relevant information for rendering a <see cref="BaseComponent{Props, State}"/>.
    /// </summary>
    /// <param name="Text">The text to be rendered for the component.</param>
    public record BaseRenderResult(string Text);
}
