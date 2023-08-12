namespace ConsoleUserInterface.Core.Components;

internal class Label : BaseComponent<Label.Props, Label.State> {
    internal Label(Props props, ITransform transform) : base(props, transform) {
    }

    public override bool ReceiveKey(ConsoleKeyInfo keyInfo) => false;

    public override BaseRenderResult Render() => new(props.Text, props.Underlined);

    internal record Props(string Text, bool Underlined);
    internal record State();
}