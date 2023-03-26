using ConsoleUserInterfaceWithDOM.Core.Dom;

namespace ConsoleUserInterfaceWithDOM.Core {
    public interface IComponent {
        internal ITransform Transform { get; }
        internal IPropsAndState PropsAndState { get; }

        internal event Action OnStateChanged;

        public bool ReceiveKey(ConsoleKeyInfo keyInfo);

        public void OnMounted() { }
        public void OnUnmounted() { }
    }
}
