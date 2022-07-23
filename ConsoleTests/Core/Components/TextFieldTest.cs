using NUnit.Framework;
using System;
using Comps = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleTests.Core.Components {
    internal class TextFieldTest {

        [Test]
        public void TextField_Can_Be_Rendered() {
            var label = Comps.TextField("", null, 0, 0, 30, 1);
            var context = new TestUtility.TestContext(label, 30, 1);
            context.ShouldDisplay("|                             ");
        }

        [Test]
        public void TextField_Receives_Keys() {
            var label = Comps.TextField("", null, 0, 0, 30, 1);
            var context = new TestUtility.TestContext(label, 30, 1);
            context.InputText("abcdef");
            context.ShouldDisplay("abcdef|                       ");
        }

        [Test]
        public void TextField_Receives_Backspace() {
            var label = Comps.TextField("", null, 0, 0, 30, 1);
            var context = new TestUtility.TestContext(label, 30, 1);
            context.InputText("abc");
            context.ShouldDisplay("abc|                          ");
            context.InputKey(ConsoleKey.Backspace);
            context.ShouldDisplay("ab|                           ");
        }

        [Test]
        public void TextField_Receives_Arrows() {
            var label = Comps.TextField("", null, 0, 0, 30, 1);
            var context = new TestUtility.TestContext(label, 30, 1);
            context.InputText("abc");
            context.ShouldDisplay("abc|                          ");
            context.InputKey(ConsoleKey.LeftArrow);
            context.ShouldDisplay("ab|c                          ");
            context.InputKey(ConsoleKey.RightArrow);
            context.ShouldDisplay("abc|                          ");
        }

        [Test]
        public void TextField_Ignores_Arrows_On_Text_End() {
            var label = Comps.TextField("", null, 0, 0, 30, 1);
            var context = new TestUtility.TestContext(label, 30, 1);
            context.InputText("abc");
            context.ShouldDisplay("abc|                          ");

            context.InputKey(ConsoleKey.LeftArrow);
            context.InputKey(ConsoleKey.LeftArrow);
            context.InputKey(ConsoleKey.LeftArrow);
            context.ShouldDisplay("|abc                          ");
            context.InputKey(ConsoleKey.LeftArrow);
            context.InputKey(ConsoleKey.LeftArrow);
            context.InputKey(ConsoleKey.LeftArrow);
            context.ShouldDisplay("|abc                          ");

            context.InputKey(ConsoleKey.RightArrow);
            context.InputKey(ConsoleKey.RightArrow);
            context.InputKey(ConsoleKey.RightArrow);
            context.ShouldDisplay("abc|                          ");
            context.InputKey(ConsoleKey.RightArrow);
            context.InputKey(ConsoleKey.RightArrow);
            context.InputKey(ConsoleKey.RightArrow);
            context.ShouldDisplay("abc|                          ");
        }
    }
}
