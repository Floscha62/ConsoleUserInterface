﻿namespace ConsoleUserInterfaceWithDOM.Core.Components {
    internal class Label : BaseComponent<Label.Props, Label.State> {
        public Label(Props props, ITransform transform) : base(props, transform) {
        }

        public override bool ReceiveKey(ConsoleKeyInfo keyInfo) => false;

        public override BaseRenderResult Render() => new(props.Text);

        internal record Props(string Text);
        internal record State();
    }
}
