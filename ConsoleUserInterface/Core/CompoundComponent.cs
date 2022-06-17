namespace ConsoleUserInterface.Core {
    internal abstract class CompoundComponent<Props, State> : Component<Props, State> {

        public CompoundComponent(Props props, ITransform transform) : base(props, transform) { }

        public abstract CompoundRenderResult Render(int width, int height);
    }
}
