using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Comps = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleTests.Core.Components {
    public class TextAreaTest {

        [Test]
        public void Text_Box_Is_Rendered_In_Foreground() {
            var component = Comps.Group(
                Comps.Layout.VERTICAL,
                Enumerable.Repeat(Comps.Label(new string('#', 30)), 19)
                    .Prepend(Comps.TextArea("", 30, 20, s => { }))
            );
            var context = new TestUtility.TestContext(component, 30, 20);
            context.InputKey(ConsoleKey.Enter);
            context.ShouldDisplay(
                "                              ",
                "##############################",
                "##############################",
                "##############################",
                "##############################",
                "##############################",
                "##############################",
                "##############################",
                "###+----------------------+###",
                "###|                      |###",
                "###| |                    |###",
                "###|                      |###",
                "###+----------------------+###",
                "##############################",
                "##############################",
                "##############################",
                "##############################",
                "##############################",
                "##############################",
                "##############################"
            );
        }
    }
}
