namespace ConsoleUserInterface.Core {

    /// <summary>
    /// The base interface of any component. <br/>
    /// <br/>
    /// To create components, see <see cref="Components.Components"/> for most basic elements.
    /// <list>
    /// <listheader>To add custom components, see </listheader>
    /// <item><see cref="BaseComponent{Props, State}"/></item>
    /// <item><see cref="CompoundComponent{Props, State}"/></item>
    /// <item>The <c>FunctionalComponent</c> method in <see cref="Components.Components"/></item>
    /// </list>
    /// </summary>
    public interface IComponent {
        internal ITransform Transform { get; }
        internal object? ComponentProps { get; }
        internal object? ComponentState { get; }

        internal string TypeName => GetType().Name;

        internal event Action OnStateChanged;

        /// <summary>
        /// Handle key events for this component.
        /// </summary>
        /// <param name="keyInfo"> The key received. </param>
        /// <returns> <c>true</c>, if the key was used by this component; <c>false</c> otherwise.</returns>
        public bool ReceiveKey(ConsoleKeyInfo keyInfo);

        /// <summary>
        /// This is called when the component is added to the dom.
        /// 
        /// Use this to trigger effects as needed.
        /// </summary>
        public void OnMounted() { }

        /// <summary>
        /// This is called when the component is removed from the dom.
        /// 
        /// Use this to cleanup effects as needed.
        /// </summary>
        public void OnUnmounted() { }
    }
}
