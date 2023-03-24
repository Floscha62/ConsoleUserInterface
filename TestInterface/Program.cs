using ConsoleUserInterface.Core;
using ConsoleUserInterface.Core.Components;

namespace TestInterface {
    public class Program {

        public static void Main() {
            using var logger = LoggingFactory.Create(typeof(Program));
            var application = Components.Button(ITransform.Create(), "Test", () => { }, true);

            var renderer = new Renderer(application, 3);
            renderer.Start();

        }
    }
}