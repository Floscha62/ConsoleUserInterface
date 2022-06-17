namespace ConsoleUserInterface.Core {
    public abstract class BaseComponent<Props, State> : Component<Props, State>, IBaseComponent {

        public BaseComponent(Props props, ITransform transform) : base(props, transform) {
        }

        public abstract BaseRenderResult Render(int width, int height);
    }
}
