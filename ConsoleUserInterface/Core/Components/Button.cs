namespace ConsoleUserInterface.Core.Components;

internal class Button : BaseComponent<Button.Props, Button.State> {
    internal Button(Props props, ITransform transform) : base(props, transform) {
    }

    public override bool ReceiveKey(ConsoleKeyInfo keyInfo) {
        if (keyInfo.Key == ConsoleKey.Enter) {
            props.OnTrigger?.Invoke();
            return true;
        }
        return false;
    }

    public override BaseRenderResult Render() => new(props.Label);

    internal record Props(string Label, Action OnTrigger) {
        public override string ToString() => $"Props {{ Label = { Label } }}";
    }
    internal record State();
}
