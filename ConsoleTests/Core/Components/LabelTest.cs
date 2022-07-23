using ConsoleTests.Core.TestUtility;
using ConsoleUserInterface.Core;
using NUnit.Framework;
using Comps = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleTests.Core.Components {
    public class LabelTest {

        [Test]
        public void Label_Can_Be_Rendered_Top_Level() {
            var label = Comps.Label("Dies ist ein beliebiges label", 0, 0);
            var context = new TestUtility.TestContext(label, 29, 1);
            context.ShouldDisplay("Dies ist ein beliebiges label");
        }


        [Test]
        public void Label_Can_Be_Rendered_Top_Level_With_Position() {
            var label = Comps.Label("Dies ist ein beliebiges label", 15, 4);
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
    }
}
