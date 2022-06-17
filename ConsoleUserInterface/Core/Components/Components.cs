namespace ConsoleUserInterface.Core.Components {
    public static class Components {

        public static IComponent Label(string label, int x, int y) =>
            new Label(new Label.Props(label), new PositionTransform(x, y, label.Length, 1));

        public static IComponent Label(string label, int x, int y, int width, int height) =>
            new Label(new Label.Props(label), new PositionTransform(x, y, width, height));

        public static IComponent Label(string label, double weight) =>
            new Label(new Label.Props(label), new WeightedTransform(weight));

        public static IComponent TextField(string startText, int x, int y, int width, int height) =>
            new TextField(new TextField.Props(startText, null, width), new PositionTransform(x, y, width, height));

        public static IComponent TextField(string startText, int maxWidth, double weight) =>
            new TextField(new TextField.Props(startText, null, maxWidth), new WeightedTransform(weight));

        public static IComponent Container(Layout layout, int x, int y, int width, int height, params IComponent[] components) =>
            new Container(new Container.Props(layout, components), new PositionTransform(x, y, width, height));

        public static IComponent Container(Layout layout, double weight, params IComponent[] components) =>
            new Container(new Container.Props(layout, components), new WeightedTransform(weight));
    }
}
