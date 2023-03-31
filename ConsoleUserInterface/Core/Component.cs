namespace ConsoleUserInterface.Core {
    /// <summary>
    /// The base class for any component to be rendered.
    /// </summary>
    /// <typeparam name="Props">The static props of this component.</typeparam>
    /// <typeparam name="State">The dynamic state of this component.</typeparam>
    public abstract class Component<Props, State> : IComponent where State : new() {
        ITransform IComponent.Transform => transform;
        object? IComponent.ComponentProps => props;
        object? IComponent.ComponentState => state;

        string IComponent.TypeName => this.TypeName;
        internal virtual string TypeName => GetType().Name;

        /// <summary> The transform, with which the component is lain out. </summary>
        protected readonly ITransform transform;

        /// <summary> The static state provided during component creation. </summary>
        protected readonly Props props;

        event Action? IComponent.OnStateChanged { add { onStateChanged += value; } remove { onStateChanged -= value; } }
        private Action? onStateChanged;

        /// <summary> The current dynamic state of the component. Setting this will rerender the component. </summary>
        protected State CurrentState { get => state; set { state = value; onStateChanged?.Invoke(); } }
        private State state;

        internal Component(Props props, ITransform transform) {
            this.transform = transform;
            this.props = props;
            this.state = new();
        }

        /// <inheritdoc/>
        public abstract bool ReceiveKey(ConsoleKeyInfo keyInfo);
    }
}
