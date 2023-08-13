using ConsoleUserInterface.Core.Extensions;

namespace ConsoleUserInterface.Core.Components;

internal class ListSelection<T> : BaseComponent<ListSelection<T>.Props, ListSelection<T>.State> {
    internal ListSelection(ListSelection<T>.Props props, ITransform transform) : base(props, transform) {
        CurrentState = new(props.StartIndex);
    }

    public override bool ReceiveKey(ConsoleKeyInfo keyInfo) {
        switch (keyInfo) {
            case ConsoleKeyInfo(ConsoleKey.UpArrow, _, _) when CurrentState.SelectedIndex > 0:
                CurrentState = CurrentState with { SelectedIndex = CurrentState.SelectedIndex - 1 };
                props.OnSelectionChanged?.Invoke(props.Values[CurrentState.SelectedIndex]);
                return true;
            case ConsoleKeyInfo(ConsoleKey.DownArrow, _, _) when CurrentState.SelectedIndex < props.Values.Count - 1:
                CurrentState = CurrentState with { SelectedIndex = CurrentState.SelectedIndex + 1 };
                props.OnSelectionChanged?.Invoke(props.Values[CurrentState.SelectedIndex]);
                return true;
        }
        return false;
    }

    public override BaseRenderResult Render() => new(string.Join('\n', props.Values
        .Select(props.LabelFunc)
        .Select((label, i) => (i == CurrentState.SelectedIndex ? $"> {label}" : $"  {label}"))
    ));

    internal record Props(List<T> Values, Func<T, string> LabelFunc, Action<T> OnSelectionChanged, int StartIndex = 0) {
        public override string ToString() => $"Props {{ Values = [{string.Join(", ", Values.Select(t => t?.ToString()??""))}], StartIndex = {StartIndex} }}";
    }
    internal record State(int SelectedIndex) { public State() : this(0) { } }
}
