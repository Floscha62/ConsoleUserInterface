using ConsoleUserInterface.Core;
using NUnit.Framework;
using Comps = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleTests.Core.Components {
    internal class ContainerTest {

        [Test]
        public void Relative_Container_Position_Children() {
            var label1 = Comps.Label(ITransform.Create(0, 0, 3, 1), "abc");
            var label2 = Comps.Label(ITransform.Create(10, 3, 3, 1), "def");
            var container = Comps.Container(ITransform.Create(5, 2, 13, 5), Layout.RELATIVE, label1, label2);
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
            var label1 = Comps.Label(ITransform.Create(0, 0, 3, 1), "abc");
            var label2 = Comps.Label(ITransform.Create(10, 3, 3, 1), "def");
            var inner = Comps.Container(ITransform.Create(0, 0, 50, 10), Layout.ABSOLUTE, label1, label2);
            var container = Comps.Container(ITransform.Create(5, 2, 13, 5), Layout.RELATIVE, inner);
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
            var label1 = Comps.Label(ITransform.Create(3), "abc");
            var label2 = Comps.Label(ITransform.Create(7), "def");
            var container = Comps.Container(ITransform.Create(0, 0, 50, 10), Layout.VERTICAL, label1, label2);

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
            var label1 = Comps.Label(ITransform.Create(3), "abc");
            var label2 = Comps.Label(ITransform.Create(7), "def");
            var container = Comps.Container(ITransform.Create(0, 0, 50, 10), Layout.HORIZONTAL, label1, label2);

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
            var label1 = Comps.Label(ITransform.Create(3), "abc");
            var label2 = Comps.Label(ITransform.Create(7), "def");
            var upper = Comps.Container(ITransform.Create(3), Layout.HORIZONTAL, label1, label2);
            var lower = Comps.Container(ITransform.Create(7), Layout.HORIZONTAL, label1, label2);
            var container = Comps.Container(ITransform.Create(0, 0, 50, 10), Layout.VERTICAL, upper, lower);

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
            var label1_1 = Comps.Label(ITransform.Create(3), "abc");
            var label2_1 = Comps.Label(ITransform.Create(7), "def");
            var label1_2 = Comps.Label(ITransform.Create(6), "abc");
            var label2_2 = Comps.Label(ITransform.Create(4), "def");
            var left = Comps.Container(ITransform.Create(3), Layout.VERTICAL, label1_1, label2_1);
            var right = Comps.Container(ITransform.Create(7), Layout.VERTICAL, label1_2, label2_2);
            var container = Comps.Container(ITransform.Create(0,0,50,10), Layout.HORIZONTAL, left, right);

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

        [Test]
        public void Container_Should_Give_Control_To_Child() {
            var component = Comps.Container(
                ITransform.Create(0,0,20,10),
                Layout.HORIZONTAL,
                Comps.Label(ITransform.Create(1), "Left"),
                Comps.Container(ITransform.Create(1), Layout.VERTICAL, 
                    Comps.Label(ITransform.Create(1), "Header"), 
                    Comps.TextField(ITransform.Create(9), "Text", s => { })
                )
            );

            var context = new TestUtility.TestContext(component, 20, 10);
            context.ShouldDisplay(
                "Left      Header    ",
                "          Text|     ",
                "                    ",
                "                    ",
                "                    ",
                "                    ",
                "                    ",
                "                    ",
                "                    ",
                "                    "
            );
            context.InputKey(System.ConsoleKey.Tab);
            context.InputKey(System.ConsoleKey.Tab);
            context.InputKey(System.ConsoleKey.Tab);
            context.InputKey(System.ConsoleKey.Tab);
            context.InputText(" 1234");

            context.ShouldDisplay(
                "Left      Header    ",
                "          \u001b[0m\u001b[48;2;20;20;40mText 1234|",
                "                    ",
                "                    ",
                "                    ",
                "                    ",
                "                    ",
                "                    ",
                "                    ",
                "                    "
            );
        }
    }
}
