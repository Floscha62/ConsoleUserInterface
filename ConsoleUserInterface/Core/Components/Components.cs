using System;
using System.Collections.Generic;
using static ConsoleUserInterface.Core.ITransform;

namespace ConsoleUserInterface.Core.Components {
    public static class Components {

        public static IComponent Label(ITransform transform, string label, string ellipsis = "...", bool underlined = false) =>
            new Label(new Label.Props(label, ellipsis, underlined), transform);

        public static IComponent TextField(ITransform transform, string startText, Action<string> onChange) =>
            new TextField(new TextField.Props(startText, onChange), transform);

        public static IComponent TextArea(ITransform transform, string startText, Action<string> onChange, int popUpWidth) =>
            new TextArea(new TextArea.Props(startText, popUpWidth, onChange), transform);

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

        public static IComponent Modal(ITransform transform, IEnumerable<IComponent> components) =>
            new Container(new Container.Props(Layout.ABSOLUTE, components), transform);

    }
}
