using ConsoleUserInterface.Core.Components;
using ConsoleUserInterface.Core;
using LoggingConsole;

var comp = Components.FunctionComponent<object?, State>(ITransform.Create(1.0, 1.0), null, initialState: null, Mirror);

LoggingFactory.EnableConsole = true;

var rend = new Renderer("__", comp);
rend.Start();

static CompoundRenderResult Mirror(object? _, State state, Action<State> setState, Callbacks callbacks) {
    var updateState = callbacks.Create<string>(s => setState(new(s)), new());

    return new(new[] {
        Components.Container(ITransform.Create(1), Layout.Vertical, true, Components.TextField(ITransform.Create(1), "", updateState)),
        Components.Container(ITransform.Create(1), Layout.Vertical, true,
            Components.Label(ITransform.Create(1), state.Val, false),
            Components.Label(ITransform.Create(1), state.Val, false)
        ),
        Components.Container(ITransform.Create(1), Layout.Vertical, true,
            Components.Label(ITransform.Create(1), state.Val, false),
            Components.Label(ITransform.Create(1), state.Val, false)
        ),
    }, Layout.Horizontal, false);
}

record State(string Val) {
    public State() : this("") { }

    public static State Create(string Val) { return new State(Val); }
}