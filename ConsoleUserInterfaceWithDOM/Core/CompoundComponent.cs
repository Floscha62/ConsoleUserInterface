namespace ConsoleUserInterfaceWithDOM.Core {
    public abstract class CompoundComponent<Props, State> : Component<Props, State>, ICompoundComponent where State : new() {

        public CompoundComponent(Props props, ITransform transform) : base(props, transform) { }

        public abstract CompoundRenderResult Render();
    }
}
