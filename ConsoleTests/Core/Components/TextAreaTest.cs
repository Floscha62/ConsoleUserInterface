using ConsoleTests.Core.TestUtility;
using ConsoleUserInterface.Core;
using NUnit.Framework;
using System;
using Comps = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleTests.Core.Components {
    internal class TextAreaTest {
        [Test]
        public void TextArea_Can_Be_Rendered() {
            var label = Comps.TextArea(ITransform.Create(0,0,30,1), "", null, 30);
            var context = new TestUtility.TestContext(label, 30, 1);
            context.ShouldDisplay("\u001b[48;2;20;20;40m                              ");
        }

        [Test]
        public void TextArea_Receives_Keys() {
            var label = Comps.TextArea(ITransform.Create(0,0,30,1), "", null, 30);
            var context = new TestUtility.TestContext(label, 30, 1);
            context.InputKey(ConsoleKey.Enter);
            context.InputText("abcdef");
            context.InputKey(ConsoleKey.Enter);
            context.ShouldDisplay("\u001b[48;2;20;20;40mabcdef                        ");
        }

        [Test]
        public void TextArea_Receives_Backspace() {
            var label = Comps.TextArea(ITransform.Create(0, 0, 30, 1), "", null, 30);
            var context = new TestUtility.TestContext(label, 30, 1);
            context.InputKey(ConsoleKey.Enter);
            context.InputText("abc");
            context.InputKey(ConsoleKey.Enter);
            context.ShouldDisplay("\u001b[48;2;20;20;40mabc                           ");
            context.InputKey(ConsoleKey.Enter);
            context.InputKey(ConsoleKey.Backspace);
            context.InputKey(ConsoleKey.Enter);
            context.ShouldDisplay("\u001b[48;2;20;20;40mab                            ");
        }
    }
}
