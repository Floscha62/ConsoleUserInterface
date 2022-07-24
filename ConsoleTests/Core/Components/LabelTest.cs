using ConsoleTests.Core.TestUtility;
using ConsoleUserInterface.Core;
using NUnit.Framework;
using Comps = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleTests.Core.Components {
    public class LabelTest {

        const string labelText = "Dies ist ein beliebiges label";

        [Test]
        public void Label_Can_Be_Rendered_Top_Level() {
            var label = Comps.Label(ITransform.Create(0, 0, labelText.Length, 1), labelText);
            var context = new TestUtility.TestContext(label, labelText.Length, 1);
            context.ShouldDisplay("Dies ist ein beliebiges label");
        }


        [Test]
        public void Label_Can_Be_Rendered_Top_Level_With_Position() {
            var label = Comps.Label(ITransform.Create(15, 4, labelText.Length, 1), labelText);
            var context = new TestUtility.TestContext(label, 50, 10);
            context.ShouldDisplay(
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "               Dies ist ein beliebiges label      ",
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "                                                  ",
                "                                                  "
                );
        }

        [Test]
        public void Label_Can_Be_Underlined() {
            var label = Comps.Label(ITransform.Create(0, 0, labelText.Length, 1), labelText, underlined: true);
            var context = new TestUtility.TestContext(label, labelText.Length, 1);
            context.ShouldDisplay("\x1b[4mDies ist ein beliebiges label");
        }
    }
}
