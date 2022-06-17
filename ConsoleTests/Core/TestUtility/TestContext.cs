using ConsoleUserInterface.Core;
using System;

namespace ConsoleTests.Core.TestUtility {
    public class TestContext {

        private TestConsole console;
        private Renderer renderer;

        public TestContext(IComponent component, int width, int height) {
            this.console = new TestConsole(width, height);
            this.renderer = new Renderer(component, console: this.console);
        }

        public void InputText(string text) {
            console.AddStringInput(text);
            for (int i = 0; i < text.Length; i++) {
                renderer.Receive();
            }
        }

        public void InputKey(ConsoleKey key) {
            console.AddKeyInput(new ConsoleKeyInfo('\0', key, false, false, false));
            renderer.Receive();
        }

        public void ShouldDisplay(params string[] lines) {
            renderer.RenderFrame(true);
            console.ShouldDisplay(lines);
        }
    }
}
