using System;
using System.Collections.Generic;

namespace ConsoleUserInterface.Core.Components {
    public static class Components {

        public static IComponent Label(ITransform transform, string label, string ellipsis = "...", bool underlined = false) =>
            new Label(new(label, ellipsis, underlined), transform);

        public static IComponent TextField(ITransform transform, string startText, Action<string> onChange) =>
            new TextField(new(startText, onChange), transform);

        public static IComponent TextArea(ITransform transform, string startText, Action<string> onChange, int popUpWidth) =>
            new TextArea(new(startText, popUpWidth, onChange), transform);

        public static IComponent Container(ITransform transform, Layout layout, params IComponent[] components) =>
            new Container(new(layout, components, 0), transform);

        public static IComponent Container(ITransform transform, Layout layout, IEnumerable<IComponent> components) =>
            new Container(new(layout, components, 0), transform);

        public static IComponent TreeView<T>(ITransform transform, T root, Action<T> onSelected) where T : TreeElement<T> =>
            new TreeView<T>(new(root, onSelected), transform);

        public static IComponent TreeElementEditor<T>(ITransform transform, T root, Func<T, IComponent> func) where T : TreeElement<T> {
            var view = func(root);
            var treeView = TreeView(ITransform.Create(1), root, SelectElement);

            void SelectElement(T element) => view = func(element);
            IEnumerable<IComponent> Components() {
                yield return treeView;
                yield return view;
            }

            return Container(transform, Layout.HORIZONTAL, Components());
        }

        public static IComponent Button(ITransform transform, string label, Action action, bool underlined) =>
            new Button(new(action, label, underlined), transform);

        public static IComponent Modal(ITransform transform, params IComponent[] components) =>
            new Container(new(Layout.ABSOLUTE, components, 1), transform);

        public static IComponent Modal(ITransform transform, IEnumerable<IComponent> components) =>
            new Container(new(Layout.ABSOLUTE, components, 1), transform);

        public static IComponent ListSelection<T>(ITransform transform, List<T> values, Func<T, string> labelFunc, Action<T> onSelect, int startIndex = 0, int? displayedSize = null) =>
            new ListSelection<T>(new(values, labelFunc, onSelect, startIndex, displayedSize ?? values.Count), transform);
    }
}
