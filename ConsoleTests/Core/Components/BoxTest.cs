using ConsoleTests.Core.TestUtility;
using ConsoleUserInterface.Core;
using NUnit.Framework;
using Comps = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleTests.Core.Components {
    public class BoxTest {

        [Test]
        public void Box_With_Default_Border() {
            var box = Comps.Box(Comps.Label("abc"));
            var console = new TestConsole(7, 5);
            var renderer = new Renderer(box, console: console);
            renderer.RenderFrame(true);
            console.ShouldDisplay(
                "+-----+",
                "|     |",
                "| abc |",
                "|     |",
                "+-----+"
            );
        }

        [Test]
        public void Box_With_Customized_Border() {
            var box = Comps.Box(Comps.Label("abc"), '#', '#', '#');
            var console = new TestConsole(7, 5);
            var renderer = new Renderer(box, console: console);
            renderer.RenderFrame(true);
            console.ShouldDisplay(
                "#######",
                "#     #",
                "# abc #",
                "#     #",
                "#######"
            );
        }

        [Test]
        public void Box_Cascades_Input() {
            var box = Comps.Box(Comps.TextField("", _ => { }));
            var console = new TestConsole(8, 5);
            var renderer = new Renderer(box, console: console);
            console.AddStringInput("abc");
            renderer.Receive();
            renderer.Receive();
            renderer.Receive();
            renderer.RenderFrame(true);
            console.ShouldDisplay(
                "+------+",
                "|      |",
                "| abc| |",
                "|      |",
                "+------+"
            );
        }
    }
}
