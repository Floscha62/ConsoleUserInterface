namespace ConsoleUserInterface.Core {

    /// <summary>
    /// This is the base class for a component, which renders multiple child components.
    /// </summary>
    /// <typeparam name="Props">The static state of the component.</typeparam>
    /// <typeparam name="State">The dynamic state of the component.</typeparam>
    public abstract class CompoundComponent<Props, State> : Component<Props, State>, ICompoundComponent where State : new() {

        /// <summary>
        /// Creates a new compound component.
        /// </summary>
        /// <param name="props"> The static state of this component. </param>
        /// <param name="transform"> The transform used for laying out this component. </param>
        public CompoundComponent(Props props, ITransform transform) : base(props, transform) { }

        /// <summary>
        /// This will render the component based on its props and its internal state.
        /// </summary>
        /// <returns>The render result containing the relevant information for this component.</returns>
        public abstract CompoundRenderResult Render();
    }
}
