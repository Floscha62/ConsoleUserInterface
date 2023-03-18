namespace ConsoleUserInterface.Core {
    public abstract class CompoundComponent<Props, State> : Component<Props, State>, ICompoundComponent {

        public CompoundComponent(Props props, ITransform transform) : base(props, transform) { }

        public abstract CompoundRenderResult Render(int width, int height, bool inFocus);
    }
}
