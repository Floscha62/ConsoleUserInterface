using NUnit.Framework;
using System;
using System.Linq;
using Comps = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleTests.Core.Components {
    public class TextFieldTest {

        [Test]
        public void TextField_Starts_With_StartLabel() {
            var startText = "Dies kann ein beliebiger Text sein";
            var textField = Comps.TextField(startText, s => { });
            var context = new TestUtility.TestContext(textField, 35, 1);
            context.ShouldDisplay($"{startText}|");
        }

        [Test]
        public void TextField_Can_Navigate_With_Left_And_Right_Arrow() {
            var startText = "Beliebiger Text";
            var textField = Comps.TextField(startText, s => { });
            var context = new TestUtility.TestContext(textField, startText.Length + 1, 1);
            context.InputKey(ConsoleKey.LeftArrow);
            context.InputKey(ConsoleKey.LeftArrow);
            context.InputKey(ConsoleKey.LeftArrow);
            context.ShouldDisplay("Beliebiger T|ext");
            context.InputKey(ConsoleKey.RightArrow);
            context.InputKey(ConsoleKey.RightArrow);
            context.InputKey(ConsoleKey.RightArrow);
            context.ShouldDisplay("Beliebiger Text|");
        }

        [Test]
        public void TextField_Cannot_Navigate_Outside_String() {
            var startText = "abc";
            var textField = Comps.TextField(startText, s => { });
            var context = new TestUtility.TestContext(textField, startText.Length + 1, 1);
            foreach (var key in Enumerable.Repeat(ConsoleKey.LeftArrow, 4)) {
                context.InputKey(key);
            }
            context.ShouldDisplay("|abc");
            context.InputKey(ConsoleKey.RightArrow);
            context.ShouldDisplay("a|bc");

            foreach (var key in Enumerable.Repeat(ConsoleKey.RightArrow, 3)) {
                context.InputKey(key);
            }
            context.ShouldDisplay("abc|");
            context.InputKey(ConsoleKey.LeftArrow);
            context.ShouldDisplay("ab|c");
        }

        [Test]
        public void TextField_Updates_On_Char() {
            var startText = "";
            var updateCount = 0;
            var textField = Comps.TextField(startText, s => { updateCount++; });
            var context = new TestUtility.TestContext(textField, 7, 1);
            context.InputText("abcdef");
            Assert.That(updateCount, Is.EqualTo(6));
            context.ShouldDisplay("abcdef|");
        }

        [Test]
        public void TextField_Updates_On_BackSpace() {
            var startText = "abcdef";
            var updateCount = 0;
            var textField = Comps.TextField(startText, s => { updateCount++; });
            var context = new TestUtility.TestContext(textField, startText.Length + 1, 1);
            for (int i = 0; i < 7; i++) {
                context.InputKey(ConsoleKey.Backspace);
            }

            Assert.That(updateCount, Is.EqualTo(6));
            context.ShouldDisplay("|      ");
        }
    }
}
