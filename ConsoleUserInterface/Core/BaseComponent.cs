namespace ConsoleUserInterface.Core {

    /// <summary>
    /// This is the base class for a component, which renders text.
    /// </summary>
    /// <typeparam name="Props">The static state of the component.</typeparam>
    /// <typeparam name="State">The dynamic state of the component.</typeparam>
    public abstract class BaseComponent<Props, State> : Component<Props, State>, IBaseComponent where State : new() {

        /// <summary>
        /// Creates a new base component with the provides props and transform.
        /// </summary>
        /// <param name="props">The props of the component.</param>
        /// <param name="transform">The transform of the component.</param>
        public BaseComponent(Props props, ITransform transform) : base(props, transform) {
        }

        /// <summary>
        /// This will render the component based on its props and its internal state.
        /// </summary>
        /// <returns>The render result containing the relevant information for this component.</returns>
        public abstract BaseRenderResult Render();
    }
}
