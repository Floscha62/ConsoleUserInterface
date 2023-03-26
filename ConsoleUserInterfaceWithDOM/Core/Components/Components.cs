namespace ConsoleUserInterfaceWithDOM.Core.Components {
    public static class Components {
        public static IComponent Container(ITransform transform, Layout layout, bool childrenFocusable, params IComponent[] components) =>
            new Container(new(layout, components.ToList(), childrenFocusable, 0), transform);
        public static IComponent Container(ITransform transform, Layout layout, bool childrenFocusable, List<IComponent> components) =>
            new Container(new(layout, components.ToList(), childrenFocusable, 0), transform);
        public static IComponent TextField(ITransform transform, string startText, Action<string> onChange) =>
            new TextField(new(startText, onChange), transform);
        public static IComponent Label(ITransform transform, string label) =>
            new Label(new(label), transform);
        public static IComponent Button(ITransform transform, string label, Action action) =>
            new Button(new(label, action), transform);
        public static IComponent ListSelection<T>(ITransform transform, List<T> values, Func<T, string> labelFunc, Action<T> onSelect, int startIndex = 0) =>
            new ListSelection<T>(new(values, labelFunc, onSelect, startIndex), transform);

        public static IComponent FunctionComponent<P, S>(
            ITransform transform,
            P p,
            S? initialState,
            Func<P, S, Action<S>, (Func<ConsoleKeyInfo, Action<S>, bool>, CompoundRenderResult)> implementation
        ) where S : new() => new FunctionComponent<P, S>(new(p, initialState, implementation), transform);
    }
}
