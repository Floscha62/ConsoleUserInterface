namespace ConsoleUserInterface.Core {
    internal interface ICompoundComponent {
        public ITransform Transform { get; }

        public abstract CompoundRenderResult Render(int width, int height);

    }
}
