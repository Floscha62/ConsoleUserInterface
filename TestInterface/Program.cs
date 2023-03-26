using ConsoleUserInterfaceWithDOM.Core;
using ConsoleUserInterfaceWithDOM.Core.Components;
using LoggingConsole;

namespace TestInterface {
    public class Program {


        public static void Main() {
            using ILogger logger = LoggingFactory.Create(typeof(Program));

            var pages = Enumerable.Range(0, 3).Select(i =>
                Components.TextField(ITransform.Create(10), $"Page {i}", s => logger?.Debug($"Textfield updated: '{s}'"))
            ).ToArray();

            var application = Components.Container(ITransform.Create(), Layout.VERTICAL, true,
                Components.Label(ITransform.Create(1), "UP"),
                Components.Container(ITransform.Create(1), Layout.HORIZONTAL, true,
                    Components.FunctionComponent(ITransform.Create(2), pages, 0, (props, state, setState) => {
                        var list = Components.ListSelection(
                            ITransform.Create(1),
                            Enumerable.Range(0, props.Length).ToList(),
                            i => i.ToString(),
                            setState,
                            state
                        );

                        return ((_, _) => false, new(Layout.INHERIT, new[] { list, props[state] }, false, true, 0));
                    }),
                    Components.Button(ITransform.Create(1), "Click Here", () => logger?.Debug($"Button pressed"))
                ),
                Components.Label(ITransform.Create(1), "DOWN")
            );

            var renderer = new Renderer(application, 3);
            renderer.Start();
        }
    }
}