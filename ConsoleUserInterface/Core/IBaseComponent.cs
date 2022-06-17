namespace ConsoleUserInterface.Core {
    internal interface IBaseComponent {
        public ITransform Transform { get; }

        public abstract BaseRenderResult Render(int width, int height);
    }
}
