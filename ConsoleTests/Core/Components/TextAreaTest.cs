using ConsoleTests.Core.TestUtility;
using ConsoleUserInterface.Core;
using NUnit.Framework;
using System;
using Comps = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleTests.Core.Components {
    internal class TextAreaTest {
        [Test]
        public void TextArea_Can_Be_Rendered() {
            var label = Comps.TextArea("", null, 0, 0, 30, 1);
            var context = new TestUtility.TestContext(label, 30, 1);
            context.ShouldDisplay("                              ");
        }

        [Test]
        public void TextArea_Receives_Keys() {
            var label = Comps.TextArea("", null, 0, 0, 30, 1);
            var context = new TestUtility.TestContext(label, 30, 1);
            context.InputKey(ConsoleKey.Enter);
            context.InputText("abcdef");
            context.InputKey(ConsoleKey.Enter);
            context.ShouldDisplay("abcdef                        ");
        }

        [Test]
        public void TextArea_Receives_Backspace() {
            var label = Comps.TextArea("", null, 0, 0, 30, 1);
            var context = new TestUtility.TestContext(label, 30, 1);
            context.InputKey(ConsoleKey.Enter);
            context.InputText("abc");
            context.InputKey(ConsoleKey.Enter);
            context.ShouldDisplay("abc                           ");
            context.InputKey(ConsoleKey.Enter);
            context.InputKey(ConsoleKey.Backspace);
            context.InputKey(ConsoleKey.Enter);
            context.ShouldDisplay("ab                            ");
        }
    }
}
