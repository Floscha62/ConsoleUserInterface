using System;
using System.Collections.Generic;

namespace ConsoleUserInterface.Core.Components {
    public static class Components {

        public static IComponent Label(string label, int x, int y) =>
            new Label(new Label.Props(label), new PositionTransform(x, y, label.Length, 1));

        public static IComponent Label(string label, int x, int y, int width, int height) =>
            new Label(new Label.Props(label), new PositionTransform(x, y, width, height));

        public static IComponent Label(string label, double weight) =>
            new Label(new Label.Props(label), new WeightedTransform(weight));
        public static IComponent Label(string label, ITransform transform) =>
            new Label(new Label.Props(label), transform);

        public static IComponent TextField(string startText, Action<string> onChange, int x, int y, int width, int height) =>
            new TextField(new TextField.Props(startText, onChange, width), new PositionTransform(x, y, width, height));

        public static IComponent TextField(string startText, Action<string> onChange, int maxWidth, double weight) =>
            new TextField(new TextField.Props(startText, onChange, maxWidth), new WeightedTransform(weight));
        public static IComponent TextArea(string startText, Action<string> onChange, int x, int y, int width, int height) =>
            new TextArea(new TextArea.Props(startText, width, width, onChange), new PositionTransform(x, y, width, height));

        public static IComponent TextArea(string startText, Action<string> onChange, int maxWidth, double weight) =>
            new TextArea(new TextArea.Props(startText, maxWidth, maxWidth, onChange), new WeightedTransform(weight));

        public static IComponent Container(Layout layout, int x, int y, int width, int height, params IComponent[] components) =>
            new Container(new Container.Props(layout, components), new PositionTransform(x, y, width, height));

        public static IComponent Container(Layout layout, double weight, params IComponent[] components) =>
            new Container(new Container.Props(layout, components), new WeightedTransform(weight));

        public static IComponent Container(Layout layout, ITransform transform, params IComponent[] components) =>
            new Container(new Container.Props(layout, components), transform);

        public static IComponent Container(Layout layout, int x, int y, int width, int height, IEnumerable<IComponent> components) =>
            new Container(new Container.Props(layout, components), new PositionTransform(x, y, width, height));

        public static IComponent Container(Layout layout, double weight, IEnumerable<IComponent> components) =>
            new Container(new Container.Props(layout, components), new WeightedTransform(weight));

        public static IComponent Container(Layout layout, ITransform transform, IEnumerable<IComponent> components) =>
            new Container(new Container.Props(layout, components), transform);

        public static IComponent TreeView<T>(T root, Action<T> onSelected, int x, int y, int width, int height) where T : TreeElement<T> =>
            new TreeView<T>(new TreeView<T>.Props(root, onSelected), new PositionTransform(x, y, width, height));

        public static IComponent TreeView<T>(T root, Action<T> onSelected, double weight) where T : TreeElement<T> =>
            new TreeView<T>(new TreeView<T>.Props(root, onSelected), new WeightedTransform(weight));

        public static IComponent TreeElementEditor<T>(T root, Func<T, IComponent> func, int x, int y, int width, int height) where T : TreeElement<T> {
            var view = func(root);
            var treeView = TreeView(root, SelectElement, 1);

            void SelectElement(T element) => view = func(element);
            IEnumerable<IComponent> Components() {
                yield return treeView;
                yield return view;
            }

            return Container(Layout.HORIZONTAL, x, y, width, height, Components());
        }

        public static IComponent Modal(ITransform transform, params IComponent[] components) =>
            new Container(new Container.Props(Layout.ABSOLUTE, components), transform);
    }
}
