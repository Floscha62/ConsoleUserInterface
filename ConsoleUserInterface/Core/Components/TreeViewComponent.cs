using ConsoleUserInterface.Core.Extensions;
using static ConsoleUserInterface.Core.Components.TreeUtility;

namespace ConsoleUserInterface.Core.Components;

internal static class TreeViewComponent {

    internal record Props<T>(T RootElement, Action<T> OnSelectElement) where T : ITreeElement<T> {
        public override string ToString() => $"Props = {{ Root = {RootElement} }}";
    }

    internal record State<T>(HashSet<T> Opened, int[] SelectedElement, int[] HoveredElement) where T : ITreeElement<T> {
        public State() : this(new(), Array.Empty<int>(), Array.Empty<int>()) { }

        internal bool IsOpen(T t) => this.Opened.Contains(t);

        public override string ToString() => $"State = {{ Selected = [{string.Join(", ", SelectedElement)}], " +
            $"Hovered = [{string.Join(", ", HoveredElement)}]}}";
    }

    internal static IComponent TreeView<T>(ITransform transform, T rootElement, Action<T> onSelect) where T : ITreeElement<T> =>
        Components.FunctionComponent<Props<T>, State<T>>(transform, new(rootElement, onSelect), TreeView, handleKeys: HandleKey);

    static bool HandleKey<T>(ConsoleKeyInfo info, Props<T> props, State<T> state, Action<State<T>> updateState) where T : ITreeElement<T> {
        switch (info) {
            case ConsoleKeyInfo(ConsoleKey.RightArrow, _, _) when !state.IsOpen(props.RootElement[state.HoveredElement]): {
                    var updatedSet = new HashSet<T> { props.RootElement[state.HoveredElement] };
                    updatedSet.UnionWith(state.Opened);
                    updateState(state with { Opened = updatedSet });
                    return true;
                }
            case ConsoleKeyInfo(ConsoleKey.LeftArrow, _, _) when state.IsOpen(props.RootElement[state.HoveredElement]): {
                    var updatedSet = new HashSet<T>(state.Opened);
                    updatedSet.Remove(props.RootElement[state.HoveredElement]);
                    updateState(state with { Opened = updatedSet });
                    return true;
                }
            case ConsoleKeyInfo(ConsoleKey.DownArrow, _, _):
                updateState(state with { HoveredElement = Next(props.RootElement, state.HoveredElement, state.Opened) });
                return true;
            case ConsoleKeyInfo(ConsoleKey.UpArrow, _, _):
                updateState(state with { HoveredElement = Previous(props.RootElement, state.HoveredElement, state.Opened) });
                return true;
            case ConsoleKeyInfo(ConsoleKey.Enter, _, _) when !Enumerable.SequenceEqual(state.HoveredElement, state.SelectedElement):
                props.OnSelectElement(props.RootElement[state.HoveredElement]);
                updateState(state with { SelectedElement = state.HoveredElement });
                return true;
        }
        return false;
    }

    static CompoundRenderResult TreeView<T>(Props<T> props, State<T> state, Action<State<T>> _, Callbacks callbacks) where T : ITreeElement<T> =>
        new(
            FlattenTree(props.RootElement, state.Opened)
                .Select(ValueLabel(props.RootElement[state.SelectedElement], props.RootElement[state.HoveredElement])),
            Layout: Layout.VerticalPreserveHeight,
            ComponentsFocusable: false
        );

    static Func<(T element, int depth, bool open), int, IComponent> ValueLabel<T>(T selected, T hovered) where T : ITreeElement<T> => (item, index) =>
        Components.Label(
            ITransform.Create(0, 1), 
            $"{new string(' ', item.depth)}{(Equals(item.element, hovered) ? "→" : " ")}  {NodeMarker(item.element, item.open)}{item.element.Label}",
            underlined: Equals(item.element, selected)
        );

    static char NodeMarker<T>(T t, bool isOpen) where T : ITreeElement<T> =>
        t.Leaf ? ' ' : isOpen ? '\\' : '-';
}

internal static class TreeUtility {

    internal static int[] Previous<T>(T t, int[] currentElement, HashSet<T> openNodes) where T : ITreeElement<T> {
        if (currentElement.Length == 0) {
            return currentElement;
        }

        var children = t.GetChildren().ToArray();
        var previousOfChild = Previous(children[currentElement[0]], currentElement[1..], openNodes);

        if (Enumerable.SequenceEqual(currentElement.Skip(1), previousOfChild)) {
            var previousIndex = currentElement[0] - 1;

            if (previousIndex >= 0 && openNodes.Contains(children[previousIndex])) {
                var lastOfPrevious = Last(children[previousIndex], openNodes);
                return lastOfPrevious.Prepend(previousIndex).ToArray();
            } else if (currentElement[0] > 0) {
                return new[] { previousIndex };
            } else {
                return currentElement[..^1];
            }
        } else {
            return previousOfChild.Prepend(currentElement[0]).ToArray();
        }
    }

    internal static int[] Next<T>(T t, int[] currentElement, HashSet<T> openNodes) where T : ITreeElement<T> {
        if (currentElement.Length == 0 && (!openNodes.Contains(t) || t.Leaf)) {
            return currentElement;
        }

        if (currentElement.Length == 0) {
            return new[] { 0 };
        }
        var children = t.GetChildren().ToArray();
        var nextOfChild = Next(children[currentElement[0]], currentElement[1..], openNodes);
        if (Enumerable.SequenceEqual(currentElement.Skip(1), nextOfChild)) {
            if (currentElement[0] < children.Length - 1) {
                return new[] { currentElement[0] + 1 };
            } else {
                return currentElement;
            }
        } else {
            return nextOfChild.Prepend(currentElement[0]).ToArray();
        }
    }

    internal static int[] Last<T>(T t, HashSet<T> openNodes) where T : ITreeElement<T> {
        if (t.Leaf || !openNodes.Contains(t)) {
            return Array.Empty<int>();
        }

        var children = t.GetChildren();
        return Last(children.Last(), openNodes).Prepend(children.Count - 1).ToArray();
    }

    internal static IEnumerable<(T, int, bool)> FlattenTree<T>(T t, HashSet<T> openNodes, int depth = 0) where T : ITreeElement<T> =>
        t.Leaf || !openNodes.Contains(t) ?
        new[] { (t, depth, openNodes.Contains(t)) } :
        t.GetChildren().SelectMany(c => FlattenTree(c, openNodes, depth + 1)).Prepend((t, depth, openNodes.Contains(t)));
}

public interface ITreeElement<T> where T : ITreeElement<T> {

    internal bool Leaf => !GetChildren().Any();

    internal T this[int[] selectedElement] {
        get {
            if (selectedElement.Length == 0) {
                return (T)this;
            }
            return GetChildren()[selectedElement[0]][selectedElement[1..]];
        }
    }

    internal IEnumerable<T> GetChildrenRecursive() =>
        GetChildren().SelectMany(c => c.GetChildrenRecursive()).Prepend((T)this);

    public abstract string Label { get; }
    public List<T> GetChildren();
}

