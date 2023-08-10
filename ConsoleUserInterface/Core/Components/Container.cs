namespace ConsoleUserInterface.Core.Components {
    internal class Container : CompoundComponent<Container.Props, Container.State> {
        private readonly List<IComponent> children;

        internal Container(Props props, List<IComponent> children, ITransform transform) : base(props, transform) {
            this.children = children;
        }

        public override bool ReceiveKey(ConsoleKeyInfo keyInfo) => false;

        public override CompoundRenderResult Render() => new(children, props.Layout, false, props.ChildrenFocusable, props.ZOffset);

        internal record Props(Layout Layout, bool ChildrenFocusable = true, int ZOffset = 0) {
            public override string ToString() => $"Props {{ Layout = {Layout}, ChildrenFocusable = {ChildrenFocusable}, ZOffset = {ZOffset} }}";
        }
        internal record State();
    }
}
