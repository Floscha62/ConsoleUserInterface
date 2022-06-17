using ConsoleTests.Core.TestUtility;
using ConsoleUserInterface.Core;
using NUnit.Framework;
using Comps = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleTests.Core.Components {
    public class LabelTest {

        [Test]
        public void Label_Displays_Text() {
            var labelText = "Dies sollte ein beliebiger Text sein";
            var label = Comps.Label(labelText);
            var console = new TestConsole(labelText.Length, 1);
            var renderer = new Renderer(label, console: console);
            renderer.RenderFrame(true);
            console.ShouldDisplay(labelText);
        }

        [Test]
        public void Label_Underlines_Text() {
            var labelText = "Dies sollte ein beliebiger Text sein";
            var label = Comps.Label(labelText, true);
            var console = new TestConsole(labelText.Length, 1);
            var renderer = new Renderer(label, console: console);
            renderer.RenderFrame(true);
            console.ShouldDisplay($"\x1b[4m{labelText}");
        }

        [Test]
        public void Label_Stops_Underlining() {
            var labelText = "Abc";
            var label = Comps.Label(labelText, true);
            var console = new TestConsole(labelText.Length + 3, 1);
            var renderer = new Renderer(label, console: console);
            renderer.RenderFrame(true);
            console.ShouldDisplay($"\x1b[4m{labelText}\x1b[0m   ");
        }

        [Test]
        public void Label_Underlines_In_Background() {
            var labelText = "Abcdef";
            var label = Comps.Group(Comps.Layout.VERTICAL, Comps.Label(labelText, true), Comps.Modal(Comps.Label("ffff"), 1));
            var console = new TestConsole(labelText.Length + 3, 1);
            var renderer = new Renderer(label, console: console);
            renderer.RenderFrame(true);
            console.ShouldDisplay($"\x1b[4mAb\x1b[0mffff   ");
        }
    }
}
