namespace ConsoleUserInterface.Core {

    /// <summary>
    /// Defines the layout of a compound component.
    /// </summary>
    public enum Layout {
        /// <summary> Inherit the layout from the parent component. </summary>
        Inherit = 0,
        /// <summary> 
        /// All positioning and sizing of the child components are in relation to the window.
        /// </summary>
        Absolute,
        /// <summary> 
        /// All positioning and sizing of the child components are in relation to parent component.
        /// </summary>
        Relative,
        /// <summary>
        /// The components are lain out vertically. The components are rendered lain out with the width of the parent component
        /// and with a height proportional to the transforms weight. 
        /// All child components need to have a transform with a weight. <see cref="ITransform.Create(double)"/>
        /// </summary>
        Vertical,
        /// <summary>
        /// The components are lain out vertically. The components are rendered lain out with the width of the parent component
        /// and with the height provided by the transform.
        /// All child components need to have a transform with a size.
        /// </summary>
        VerticalPreserveHeight,
        /// <summary>
        /// The components are lain out horizontally. The components are rendered lain out with the height of the parent component
        /// and with a width proportional to the transforms weight. 
        /// All child components need to have a transform with a weight. <see cref="ITransform.Create(double)"/>
        /// </summary>
        Horizontal,
        /// <summary>
        /// The components are lain out horizontally. The components are rendered lain out with the height of the parent component
        /// and with the width provided by the transform.
        /// All child components need to have a transform with a size.
        /// </summary>
        HorizontalPreserveHeight,
    }
}
