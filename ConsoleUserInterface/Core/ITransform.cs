namespace ConsoleUserInterface.Core {
    public interface ITransform {
        public static ITransform Create(double weight) => new WeightedTransform(weight);
        public static ITransform Create(int x, int y, int width, int height) => new PositionTransform(x, y, width, height);
        public static ITransform Create(int width, int height) => new CenteredTransform(width, height);

        internal record WeightedTransform(double Weight) : ITransform { }
        internal record PositionTransform(int X, int Y, int Width, int Height) : ITransform { }
        internal record CenteredTransform(int Width, int Height) : ITransform { }
    }
}
