using System;

namespace ConsoleUserInterface.Core {
    public abstract class Component<Props, State> : IComponent {
        public ITransform Transform { get; }
        protected readonly Props props;
        protected State state;

        public Component(Props props, ITransform transform) {
            this.Transform = transform;
            this.props = props;
            this.state = StartingState;
        }

        protected abstract State StartingState { get; }

        public abstract bool ReceiveKey(ConsoleKeyInfo keyInfo);
    }
}
