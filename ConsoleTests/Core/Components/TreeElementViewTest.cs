using ConsoleTests.Core.TestUtility;
using ConsoleUserInterface.Core;
using ConsoleUserInterface.Core.Components;
using NUnit.Framework;
using System.Collections.Generic;
using Comps = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleTests.Core.Components {
    public class TreeElementViewTest {

        private class TestTree : TreeElement<TestTree> {
            public override string Label { get; }
            private readonly IEnumerable<TestTree> children;

            public TestTree(string label, IEnumerable<TestTree> children) =>
                (Label, this.children) = (label, children);

            public override IEnumerable<TestTree> GetChildren() => this.children;
        }

        private static TestTree T(string label, params TestTree[] children) =>
            new(label, children);

        private static readonly TestTree tree = T("*",
            T("A",
                T("AA"),
                T("AB")
            ),
            T("B")
        );

        [Test]
        public void TreeElementView_Can_Be_Rendered_Top_Level() {
            var treeView = Comps.TreeElementEditor(tree, el => Comps.Label(ITransform.Create(1), el.Label), 0, 0, 16, 10);
            var context = new TestUtility.TestContext(treeView, 16, 10); 
            context.ShouldDisplay(
                "→  -\u001b[0m\u001b[4m*\u001b[0m   *       ",
                "                ",
                "                ",
                "                ",
                "                ",
                "                ",
                "                ",
                "                ",
                "                ",
                "                "
            );
        }
    }
}
