using ConsoleTests.Core.TestUtility;
using ConsoleUserInterface.Core;
using ConsoleUserInterface.Core.Components;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Comps = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleTests.Core.Components {
    public class TreeViewTest {

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
        public void TreeView_Can_Be_Rendered_Top_Level() {
            var treeView = Comps.TreeView(tree, _ => { }, 0, 0, 10, 8);
            var context = new TestUtility.TestContext(treeView, 10, 8);
            context.ShouldDisplay(
                "→  -\u001b[0m\u001b[4m*\u001b[0m     ",
                "          ",
                "          ",
                "          ",
                "          ",
                "          ",
                "          ",
                "          "
            );
        }

        [Test]
        public void TreeView_Can_Be_Rendered_Top_Level_With_Position() {
            var treeView = Comps.TreeView(tree, _ => { }, 3, 3, 7, 5);
            var context = new TestUtility.TestContext(treeView, 10, 8);
            context.ShouldDisplay(
                "          ",
                "          ",
                "          ",
                "   →  -\u001b[0m\u001b[4m*\u001b[0m  ",
                "          ",
                "          ",
                "          ",
                "          "
            );
        }

        [Test]
        public void TreeView_Group_Can_Be_Opened() {
            var treeView = Comps.TreeView(tree, _ => { }, 0, 0, 10, 8);
            var context = new TestUtility.TestContext(treeView, 10, 8);
            context.InputKey(ConsoleKey.RightArrow);

            context.ShouldDisplay(
                "→  \\\u001b[0m\u001b[4m*\u001b[0m     ",
                "    -A    ",
                "     B    ",
                "          ",
                "          ",
                "          ",
                "          ",
                "          "
            );
        }

        [Test]
        public void TreeView_Can_Navigate() {
            var treeView = Comps.TreeView(tree, _ => { }, 0, 0, 10, 8);
            var context = new TestUtility.TestContext(treeView, 10, 8);
            context.InputKey(ConsoleKey.RightArrow);
            context.InputKey(ConsoleKey.DownArrow);

            context.ShouldDisplay(
                "   \\\u001b[0m\u001b[4m*\u001b[0m     ",
                " →  -A    ",
                "     B    ",
                "          ",
                "          ",
                "          ",
                "          ",
                "          "
            );

            context.InputKey(ConsoleKey.DownArrow);

            context.ShouldDisplay(
                "   \\\u001b[0m\u001b[4m*\u001b[0m     ",
                "    -A    ",
                " →   B    ",
                "          ",
                "          ",
                "          ",
                "          ",
                "          "
            );

            context.InputKey(ConsoleKey.DownArrow);

            context.ShouldDisplay(
                "   \\\u001b[0m\u001b[4m*\u001b[0m     ",
                "    -A    ",
                " →   B    ",
                "          ",
                "          ",
                "          ",
                "          ",
                "          "
            );

            context.InputKey(ConsoleKey.UpArrow);

            context.ShouldDisplay(
                "   \\\u001b[0m\u001b[4m*\u001b[0m     ",
                " →  -A    ",
                "     B    ",
                "          ",
                "          ",
                "          ",
                "          ",
                "          "
            );
        }
    }
}
