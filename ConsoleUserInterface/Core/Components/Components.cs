using System;
using System.Collections.Generic;

namespace ConsoleUserInterface.Core.Components {
    public static class Components {
        public enum Layout {
            VERTICAL,
            HORIZONTAL,
            NONE
        }

        public static IComponent Group(Layout direction, IEnumerable<IComponent> components) => direction switch {
            Layout.VERTICAL => new VerticalGroup(new VerticalGroup.Props(components)),
            Layout.HORIZONTAL => new HorizontalGroup(new HorizontalGroup.Props(components)),
            Layout.NONE => new NoLayoutGroup(new NoLayoutGroup.Props(components)),
            _ => throw new ArgumentException("Only Layout.VERTICAL, Layout.HORIZONTAL and Layout.None are valid")
        };

        public static IComponent Group(Layout direction, params IComponent[] components) => direction switch {
            Layout.VERTICAL => new VerticalGroup(new VerticalGroup.Props(components)),
            Layout.HORIZONTAL => new HorizontalGroup(new HorizontalGroup.Props(components)),
            Layout.NONE => new NoLayoutGroup(new NoLayoutGroup.Props(components)),
            _ => throw new ArgumentException("Only Layout.VERTICAL, Layout.HORIZONTAL and Layout.NONE are valid")
        };

        public static IComponent Modal(IComponent component, int layerOffset = 0) =>
            new CenteredComponent(new CenteredComponent.Props(component), layerOffset);

        public static IComponent Box(
            IComponent component, 
            char verticalBorder = '|', 
            char corner = '+', 
            char horizontalBorder = '-'
        ) => new Box(new Box.Props(component, new Box.BorderSet(verticalBorder, corner, horizontalBorder)));

        public static IComponent Label(string label, bool underlined = false, int maxWidth = -1) =>
            new Label(new Label.Props(label, underlined, maxWidth));

        public static IComponent TextField(string startingText, Action<string> onChange, int maxWidth = -1) =>
            new TextField(new TextField.Props(startingText, onChange, maxWidth));

        public static IComponent TextArea(string startingText, int showLength, int boxWidth, Action<string> onChange, string ellipsis = "...") =>
            new TextArea(new TextArea.Props(startingText, showLength, boxWidth, onChange, ellipsis));

        public static IComponent TreeView<T>(T root, Action<T> onSelectionChanged) where T : TreeElement<T> =>
            new TreeView<T>(new TreeView<T>.Props(root, onSelectionChanged));

        public static IComponent TreeElementEditor<T>(T root, Func<T, IComponent> elementView) where T : TreeElement<T> =>
            new TreeElementEditor<T>(new TreeElementEditor<T>.Props(root, elementView));

        public static IComponent Form(params (string label, IComponent component)[] components) =>
            new Form(new Form.Props(components));
    }
}
