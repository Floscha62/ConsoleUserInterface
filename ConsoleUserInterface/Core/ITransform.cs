namespace ConsoleUserInterface.Core {

    /// <summary>
    /// The transform of a component defines together with its parents layout, how the component is positioned and sized.
    /// </summary>
    public interface ITransform {
        /// <summary>
        /// Creates a transform with the provided weight. Can only be used with a parent with <see cref="Layout.Horizontal"/> or <see cref="Layout.Vertical"/>.
        /// See <see cref="Layout"/> for more detail.
        /// </summary>
        /// <param name="weight"> The weight of the component for the layout. </param>
        /// <returns> The transform. </returns>
        public static ITransform Create(double weight) => new WeightedTransform(weight);

        /// <summary>
        /// Creates a transform with the provided position and size. Can only be used with a parent with <see cref="Layout.Absolute"/> or <see cref="Layout.Relative"/>.
        /// See <see cref="Layout"/> for more detail.
        /// </summary>
        /// <param name="x"> The x position for the layout. </param>
        /// <param name="y"> The y position for the layout. </param>
        /// <param name="width"> The width of the component in characters. </param>
        /// <param name="height"> The height of the component in lines. </param>
        /// <returns> The transform. </returns>
        public static ITransform Create(int x, int y, int width, int height) => new PositionTransform(x, y, width, height);

        /// <summary>
        /// Creates a transform with the provided size centered in its parent or the screen. Can only be used with a parent 
        /// with <see cref="Layout.Absolute"/> or <see cref="Layout.Relative"/>.
        /// See <see cref="Layout"/> for more detail.
        /// </summary>
        /// <param name="width"> The width of the component in characters. </param>
        /// <param name="height"> The height of the component in lines. </param>
        /// <returns> The transform. </returns>
        public static ITransform Create(int width, int height) => new CenteredTransform(width, height);

        /// <summary>
        /// Creates a transform with the provided size centered in its parent or the screen. Can only be used with a parent 
        /// with <see cref="Layout.Absolute"/> or <see cref="Layout.Relative"/>.
        /// See <see cref="Layout"/> for more detail.
        /// </summary>
        /// <param name="width"> The width of the component relative to its parent or screen size. </param>
        /// <param name="height"> The height of the component relative to its parent or screen size. </param>
        /// <returns> The transform. </returns>
        public static ITransform Create(double width = 1, double height = 1) => new CenteredRationalTransform(width, height);

        internal record WeightedTransform(double Weight) : ITransform { }
        internal record PositionTransform(int X, int Y, int Width, int Height) : ITransform { }
        internal record CenteredTransform(int Width, int Height) : ITransform { }
        internal record CenteredRationalTransform(double Width, double Height) : ITransform { }
    }
}
