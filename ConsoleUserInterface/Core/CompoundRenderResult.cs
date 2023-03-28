namespace ConsoleUserInterface.Core {

    /// <summary>
    /// The container for the relevant information of a compound component.
    /// </summary>
    /// <param name="Layout"> The layout used to lay out the child components. </param>
    /// <param name="Components"> The child components. </param>
    /// <param name="SelfFocusable"> Can the compound component be focused? </param>
    /// <param name="ComponentsFocusable"> Can the child components be focused? </param>
    /// <param name="ZOffset"> The z-offset of the component. </param>
    public record CompoundRenderResult(IEnumerable<IComponent> Components, Layout Layout = Layout.Inherit, bool SelfFocusable = true, bool ComponentsFocusable = true, int ZOffset = 0) {

    }
}
