namespace ConsoleUserInterfaceWithDOM.Core {
    public abstract class BaseComponent<Props, State> : Component<Props, State>, IBaseComponent where State : new() {

        public BaseComponent(Props props, ITransform transform) : base(props, transform) {
        }

        public abstract BaseRenderResult Render();
    }
}
