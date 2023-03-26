namespace ConsoleUserInterfaceWithDOM.Core.Components {
    internal class Container : CompoundComponent<Container.Props, Container.State> {
        public Container(Props props, ITransform transform) : base(props, transform) {
        }

        public override bool ReceiveKey(ConsoleKeyInfo keyInfo) => false;

        public override CompoundRenderResult Render() => new(props.Layout, props.Children, false, props.ChildrenFocusable, props.ZOffset);

        internal record Props(Layout Layout, List<IComponent> Children, bool ChildrenFocusable = true, int ZOffset = 0);
        internal record State();
    }
}
