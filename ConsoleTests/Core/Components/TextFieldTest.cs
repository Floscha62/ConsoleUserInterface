using ConsoleUserInterface.Core;
using NUnit.Framework;
using System;
using Comps = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleTests.Core.Components {
    internal class TextFieldTest {

        [Test]
        public void TextField_Can_Be_Rendered() {
            var label = Comps.TextField(ITransform.Create(0,0,30,1), "", null);
            var context = new TestUtility.TestContext(label, 30, 1);
            context.ShouldDisplay("\u001b[48;2;20;20;40m|                             ");
        }

        [Test]
        public void TextField_Receives_Keys() {
            var label = Comps.TextField(ITransform.Create(0, 0, 30, 1), "", null);
            var context = new TestUtility.TestContext(label, 30, 1);
            context.InputText("abcdef");
            context.ShouldDisplay("\u001b[48;2;20;20;40mabcdef|                       ");
        }

        [Test]
        public void TextField_Receives_Backspace() {
            var label = Comps.TextField(ITransform.Create(0, 0, 30, 1), "", null);
            var context = new TestUtility.TestContext(label, 30, 1);
            context.InputText("abc");
            context.ShouldDisplay("\u001b[48;2;20;20;40mabc|                          ");
            context.InputKey(ConsoleKey.Backspace);
            context.ShouldDisplay("\u001b[48;2;20;20;40mab|                           ");
        }

        [Test]
        public void TextField_Receives_Arrows() {
            var label = Comps.TextField(ITransform.Create(0, 0, 30, 1), "", null);
            var context = new TestUtility.TestContext(label, 30, 1);
            context.InputText("abc");
            context.ShouldDisplay("\u001b[48;2;20;20;40mabc|                          ");
            context.InputKey(ConsoleKey.LeftArrow);
            context.ShouldDisplay("\u001b[48;2;20;20;40mab|c                          ");
            context.InputKey(ConsoleKey.RightArrow);
            context.ShouldDisplay("\u001b[48;2;20;20;40mabc|                          ");
        }

        [Test]
        public void TextField_Ignores_Arrows_On_Text_End() {
            var label = Comps.TextField(ITransform.Create(0, 0, 30, 1), "", null);
            var context = new TestUtility.TestContext(label, 30, 1);
            context.InputText("abc");
            context.ShouldDisplay("\u001b[48;2;20;20;40mabc|                          ");

            context.InputKey(ConsoleKey.LeftArrow);
            context.InputKey(ConsoleKey.LeftArrow);
            context.InputKey(ConsoleKey.LeftArrow);
            context.ShouldDisplay("\u001b[48;2;20;20;40m|abc                          ");
            context.InputKey(ConsoleKey.LeftArrow);
            context.InputKey(ConsoleKey.LeftArrow);
            context.InputKey(ConsoleKey.LeftArrow);
            context.ShouldDisplay("\u001b[48;2;20;20;40m|abc                          ");

            context.InputKey(ConsoleKey.RightArrow);
            context.InputKey(ConsoleKey.RightArrow);
            context.InputKey(ConsoleKey.RightArrow);
            context.ShouldDisplay("\u001b[48;2;20;20;40mabc|                          ");
            context.InputKey(ConsoleKey.RightArrow);
            context.InputKey(ConsoleKey.RightArrow);
            context.InputKey(ConsoleKey.RightArrow);
            context.ShouldDisplay("\u001b[48;2;20;20;40mabc|                          ");
        }
    }
}
