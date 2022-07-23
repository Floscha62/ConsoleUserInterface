namespace ConsoleUserInterface.Core {
    public interface ITransform {
        public static ITransform Create(double weight) => new WeightedTransform(weight);
        public static ITransform Create(int x, int y, int width, int height) => new PositionTransform(x, y, width, height);
    }
}
