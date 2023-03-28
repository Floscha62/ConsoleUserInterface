using ConsoleUserInterface.Core;
using ConsoleUserInterface.Core.Components;
using LoggingConsole;

namespace TestInterface {
    public class Program {

        public static void Main() {
            using ILogger logger = LoggingFactory.Create(typeof(Program));

            var pages = Enumerable.Range(0, 3).Select(i =>
                Components.TextField(ITransform.Create(10), $"Page {i}", s => logger?.Debug($"Textfield updated: '{s}'"))
            ).ToArray();

            var helloWorldModal = Components.Modal(ITransform.Create(0), true, Components.Label(ITransform.Create(.8, .8), "Hello World"));

            var application = Components.Container(ITransform.Create(), Layout.Vertical, true,
                Components.Label(ITransform.Create(1), "UP"),
                Components.Container(ITransform.Create(1), Layout.Horizontal, true,
                    Components.FunctionComponent(ITransform.Create(2), pages, 0, PagedTextEditor),
                    Components.FunctionComponent(ITransform.Create(1), helloWorldModal, false, ModalButton)
                ),
                Components.Label(ITransform.Create(1), "DOWN")
            );

            var renderer = new Renderer("Test Application", application, 3);
            renderer.Start();
        }

        static CompoundRenderResult ModalButton(IComponent modal, bool open, Action<bool> setOpen) => new(
            open
                ? (new[] { Components.Button(ITransform.Create(1), "Click Here", () => setOpen(!open)), modal })
                : (IEnumerable<IComponent>)(new[] { Components.Button(ITransform.Create(1), "Click Here", () => setOpen(!open)) }), 
            SelfFocusable: false
        );

        static CompoundRenderResult PagedTextEditor(IComponent[] props, int state, Action<int> setState) => new(
            new[] { 
                Components.ListSelection(
                    ITransform.Create(1),
                    Enumerable.Range(0, props.Length).ToList(),
                    setState,
                    i => i.ToString(),
                    state
                ), 
                props[state] 
            },
            SelfFocusable: false
        );
    }
}