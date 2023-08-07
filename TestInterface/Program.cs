// See https://aka.ms/new-console-template for more information
using ConsoleUserInterface.Core;
using ConsoleUserInterface.Core.Components;

//LoggingConsole.LoggingFactory.EnableConsole = true;

var comp = Components.FunctionComponent<object?, State>(ITransform.Create(1.0, 1.0), null, Mirror);

var rend = new Renderer("__", comp);
rend.Start();

static CompoundRenderResult Mirror(object? _, State state, Action<State> setState) =>
    new(new[] {
        Components.TextField(ITransform.Create(1), state.Val, s => setState(new(s))),
        Components.Label(ITransform.Create(1), state.Val, false)
    }, Layout.Horizontal);

record State(string Val) {
    public State() : this("") { }
}