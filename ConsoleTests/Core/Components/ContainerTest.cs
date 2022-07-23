using ConsoleTests.Core.TestUtility;
using ConsoleUserInterface.Core;
using NUnit.Framework;
using Comps = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleTests.Core.Components {
    internal class ContainerTest {

        [Test]
        public void Relative_Container_Position_Children() {
            var label1 = Comps.Label("abc", 0, 0);
            var label2 = Comps.Label("def", 10, 3);
            var container = Comps.Container(Layout.RELATIVE, 5, 2, 13, 5, label1, label2);
            var context = new TestUtility.TestContext(container, 50, 10);
            context.ShouldDisplay(
                "                                                  ",
                "                                                  ",
                "     abc                                          ",
                "                                                  ",
                "                                                  ",
                "               def                                ",
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "                                                  "
            );
        }

        [Test]
        public void Absolute_Container_Can_Return_To_Absolute_Position() {
            var label1 = Comps.Label("abc", 0, 0);
            var label2 = Comps.Label("def", 10, 3);
            var inner = Comps.Container(Layout.ABSOLUTE, 0, 0, 50, 10, label1, label2);
            var container = Comps.Container(Layout.RELATIVE, 5, 2, 13, 5, inner);
            var context = new TestUtility.TestContext(container, 50, 10);
            context.ShouldDisplay(
                "abc                                               ",
                "                                                  ",
                "                                                  ",
                "          def                                     ",
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "                                                  "
            );
        }

        [Test]
        public void Vertical_Container_Divides_Correctly() {
            var label1 = Comps.Label("abc", 3);
            var label2 = Comps.Label("def", 7);
            var container = Comps.Container(Layout.VERTICAL, 0, 0, 50, 10, label1, label2);

            var context = new TestUtility.TestContext(container, 50, 10);
            context.ShouldDisplay(
                "abc                                               ",
                "                                                  ",
                "                                                  ",
                "def                                               ",
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "                                                  "
            );
        }

        [Test]
        public void Horizontal_Container_Divides_Correctly() {
            var label1 = Comps.Label("abc", 3);
            var label2 = Comps.Label("def", 7);
            var container = Comps.Container(Layout.HORIZONTAL, 0, 0, 50, 10, label1, label2);

            var context = new TestUtility.TestContext(container, 50, 10);
            context.ShouldDisplay(
                "abc            def                                ",
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "                                                  "
            );
        }

        [Test]
        public void Vertical_And_Horizontal_Can_Be_Nested() {
            var label1 = Comps.Label("abc", 3);
            var label2 = Comps.Label("def", 7);
            var upper = Comps.Container(Layout.HORIZONTAL, 3, label1, label2);
            var lower = Comps.Container(Layout.HORIZONTAL, 7, label1, label2);
            var container = Comps.Container(Layout.VERTICAL, 0, 0, 50, 10, upper, lower);

            var context = new TestUtility.TestContext(container, 50, 10);
            context.ShouldDisplay(
                "abc            def                                ",
                "                                                  ",
                "                                                  ",
                "abc            def                                ",
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "                                                  "
            );
        }
        [Test]
        public void Horizontal_And_Vertical_Can_Be_Nested() {
            var label1_1 = Comps.Label("abc", 3);
            var label2_1 = Comps.Label("def", 7);
            var label1_2 = Comps.Label("abc", 6);
            var label2_2 = Comps.Label("def", 4);
            var left = Comps.Container(Layout.VERTICAL, 3, label1_1, label2_1);
            var right = Comps.Container(Layout.VERTICAL, 7, label1_2, label2_2);
            var container = Comps.Container(Layout.HORIZONTAL, 0, 0, 50, 10, left, right);

            var context = new TestUtility.TestContext(container, 50, 10);
            context.ShouldDisplay(
                "abc            abc                                ",
                "                                                  ",
                "                                                  ",
                "def                                               ",
                "                                                  ",
                "                                                  ",
                "               def                                ",
                "                                                  ",
                "                                                  ",
                "                                                  "
            );
        }
    }
}
