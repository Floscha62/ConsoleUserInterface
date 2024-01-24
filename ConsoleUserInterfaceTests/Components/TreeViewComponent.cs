using ConsoleUserInterface.Core.Components;
using NUnit.Framework;
using C = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleUserInterface.Tests.Components;

[TestFixture]
internal class TreeViewComponent {

    private class Tree : ITreeElement<Tree> {

        public string Label { get; }
        private readonly List<Tree> children;

        public Tree(string label, List<Tree> children) {
            this.Label = label;
            this.children = children;
        }

        public List<Tree> GetChildren() => children;

        public override bool Equals(object? other) =>
            other is Tree o &&
            this.Label.Equals(o.Label) &&
            Enumerable.SequenceEqual(this.children, o.children);

    }

    private static Tree T(string label, params Tree[] children) => new(label, children.ToList());

    private static Tree TestTree =>
        T("Root",
            T("Child-1",
                T("GrandChild-1"),
                T("GrandChild-2"),
                T("GrandChild-3")),
            T("Child-2"),
            T("Child-3"));

    [Test]
    public void Tree_View_Is_Closed_By_Default() {
        static void OnSelect(Tree s) { }
        var renderer = new TestRenderer(C.TreeView(Core.ITransform.Create(), TestTree, OnSelect));

        renderer.DomHas.RootNode
            .ThatIsStructured()
            .ThatIsFocusable()
            .WithLayout(Core.Layout.VerticalPreserveHeight)
            .WithUnfocusableChildren()
            .WithChildrenMatching(new() {
                da => da.ThatIsText()
                .WithText("→  -Root")
                .WithUnderline()
            });
    }

    [Test]
    public void Tree_View_Node_Can_Be_Opened_By_Right_Arrow_Key() {
        static void OnSelect(Tree s) { }
        var renderer = new TestRenderer(C.TreeView(Core.ITransform.Create(), TestTree, OnSelect));

        renderer.ReceiveKey(ConsoleKey.RightArrow);

        renderer.DomHas.RootNode
            .ThatIsStructured()
            .ThatIsFocusable()
            .WithLayout(Core.Layout.VerticalPreserveHeight)
            .WithUnfocusableChildren()
            .WithChildrenMatching(new() {
                da => da.ThatIsText()
                .WithText("→  \\Root")
                .WithUnderline(),
                da => da.ThatIsText()
                .WithText("    -Child-1"),
                da => da.ThatIsText()
                .WithText("     Child-2"),
                da => da.ThatIsText()
                .WithText("     Child-3")
            });
    }

    [Test]
    public void Tree_View_Node_Can_Be_Closed_By_Left_Arrow_Key() {
        static void OnSelect(Tree s) { }
        var renderer = new TestRenderer(C.TreeView(Core.ITransform.Create(), TestTree, OnSelect));

        renderer.ReceiveKey(ConsoleKey.RightArrow);

        renderer.DomHas.RootNode
            .ThatIsStructured()
            .ThatIsFocusable()
            .WithLayout(Core.Layout.VerticalPreserveHeight)
            .WithUnfocusableChildren()
            .WithChildrenMatching(new() {
                da => da.ThatIsText()
                .WithText("→  \\Root"),
                da => da.ThatIsText()
                .WithText("    -Child-1"),
                da => da.ThatIsText()
                .WithText("     Child-2"),
                da => da.ThatIsText()
                .WithText("     Child-3")
            });

        renderer.ReceiveKey(ConsoleKey.LeftArrow);

        renderer.DomHas.RootNode
            .ThatIsStructured()
            .ThatIsFocusable()
            .WithLayout(Core.Layout.VerticalPreserveHeight)
            .WithUnfocusableChildren()
            .WithChildrenMatching(new() {
                da => da.ThatIsText()
                .WithText("→  -Root")
            });
    }

    [Test]
    public void Tree_View_Can_Navigate_By_Up_And_Down_Arrow_Keys() {
        static void OnSelect(Tree s) { }
        var renderer = new TestRenderer(C.TreeView(Core.ITransform.Create(), TestTree, OnSelect));

        renderer.ReceiveKey(ConsoleKey.RightArrow);
        renderer.ReceiveKey(ConsoleKey.DownArrow);
        renderer.ReceiveKey(ConsoleKey.DownArrow);

        renderer.DomHas.RootNode
            .ThatIsStructured()
            .ThatIsFocusable()
            .WithLayout(Core.Layout.VerticalPreserveHeight)
            .WithUnfocusableChildren()
            .WithChildrenMatching(new() {
                da => da.ThatIsText()
                .WithText("   \\Root")
                .WithUnderline(),
                da => da.ThatIsText()
                .WithText("    -Child-1"),
                da => da.ThatIsText()
                .WithText(" →   Child-2"),
                da => da.ThatIsText()
                .WithText("     Child-3")
            });

        renderer.ReceiveKey(ConsoleKey.UpArrow);

        renderer.DomHas.RootNode
            .ThatIsStructured()
            .ThatIsFocusable()
            .WithLayout(Core.Layout.VerticalPreserveHeight)
            .WithUnfocusableChildren()
            .WithChildrenMatching(new() {
                da => da.ThatIsText()
                .WithText("   \\Root"),
                da => da.ThatIsText()
                .WithText(" →  -Child-1"),
                da => da.ThatIsText()
                .WithText("     Child-2"),
                da => da.ThatIsText()
                .WithText("     Child-3")
            });
    }

    [Test]
    public void Tree_View_Child_Node_Can_Be_Opened_By_Right_Array_Key() {

        static void OnSelect(Tree s) { }
        var renderer = new TestRenderer(C.TreeView(Core.ITransform.Create(), TestTree, OnSelect));

        renderer.ReceiveKey(ConsoleKey.RightArrow);
        renderer.ReceiveKey(ConsoleKey.DownArrow);
        renderer.ReceiveKey(ConsoleKey.RightArrow);

        renderer.DomHas.RootNode
            .ThatIsStructured()
            .ThatIsFocusable()
            .WithLayout(Core.Layout.VerticalPreserveHeight)
            .WithUnfocusableChildren()
            .WithChildrenMatching(new() {
                da => da.ThatIsText()
                .WithText("   \\Root")
                .WithUnderline(),
                da => da.ThatIsText()
                .WithText(" →  \\Child-1"),
                da => da.ThatIsText()
                .WithText("      GrandChild-1"),
                da => da.ThatIsText()
                .WithText("      GrandChild-2"),
                da => da.ThatIsText()
                .WithText("      GrandChild-3"),
                da => da.ThatIsText()
                .WithText("     Child-2"),
                da => da.ThatIsText()
                .WithText("     Child-3")
            });
    }

    [Test]
    public void Tree_View_Child_Node_Can_Be_Closed_By_Left_Array_Key() {
        static void OnSelect(Tree s) { }
        var renderer = new TestRenderer(C.TreeView(Core.ITransform.Create(), TestTree, OnSelect));

        renderer.ReceiveKey(ConsoleKey.RightArrow);
        renderer.ReceiveKey(ConsoleKey.DownArrow);
        renderer.ReceiveKey(ConsoleKey.RightArrow);
        renderer.ReceiveKey(ConsoleKey.LeftArrow);

        renderer.DomHas.RootNode
            .ThatIsStructured()
            .ThatIsFocusable()
            .WithLayout(Core.Layout.VerticalPreserveHeight)
            .WithUnfocusableChildren()
            .WithChildrenMatching(new() {
                da => da.ThatIsText()
                .WithText("   \\Root")
                .WithUnderline(),
                da => da.ThatIsText()
                .WithText(" →  -Child-1"),
                da => da.ThatIsText()
                .WithText("     Child-2"),
                da => da.ThatIsText()
                .WithText("     Child-3")
            });
    }

    [Test]
    public void Tree_View_Can_Navigate_Through_Grand_Children_By_Up_And_Down_Arrow_Keys() {
        static void OnSelect(Tree s) { }
        var renderer = new TestRenderer(C.TreeView(Core.ITransform.Create(), TestTree, OnSelect));

        renderer.ReceiveKey(ConsoleKey.RightArrow);
        renderer.ReceiveKey(ConsoleKey.DownArrow);
        renderer.ReceiveKey(ConsoleKey.RightArrow);

        renderer.ReceiveKey(ConsoleKey.DownArrow);
        renderer.ReceiveKey(ConsoleKey.DownArrow);
        renderer.ReceiveKey(ConsoleKey.DownArrow);
        renderer.ReceiveKey(ConsoleKey.DownArrow);

        renderer.DomHas.RootNode
            .ThatIsStructured()
            .ThatIsFocusable()
            .WithLayout(Core.Layout.VerticalPreserveHeight)
            .WithUnfocusableChildren()
            .WithChildrenMatching(new() {
                da => da.ThatIsText()
                .WithText("   \\Root")
                .WithUnderline(),
                da => da.ThatIsText()
                .WithText("    \\Child-1"),
                da => da.ThatIsText()
                .WithText("      GrandChild-1"),
                da => da.ThatIsText()
                .WithText("      GrandChild-2"),
                da => da.ThatIsText()
                .WithText("      GrandChild-3"),
                da => da.ThatIsText()
                .WithText(" →   Child-2"),
                da => da.ThatIsText()
                .WithText("     Child-3")
            });

        renderer.ReceiveKey(ConsoleKey.UpArrow);

        renderer.DomHas.RootNode
            .ThatIsStructured()
            .ThatIsFocusable()
            .WithLayout(Core.Layout.VerticalPreserveHeight)
            .WithUnfocusableChildren()
            .WithChildrenMatching(new() {
                da => da.ThatIsText()
                .WithText("   \\Root")
                .WithUnderline(),
                da => da.ThatIsText()
                .WithText("    \\Child-1"),
                da => da.ThatIsText()
                .WithText("      GrandChild-1"),
                da => da.ThatIsText()
                .WithText("      GrandChild-2"),
                da => da.ThatIsText()
                .WithText("  →   GrandChild-3"),
                da => da.ThatIsText()
                .WithText("     Child-2"),
                da => da.ThatIsText()
                .WithText("     Child-3")
            });

        renderer.ReceiveKey(ConsoleKey.UpArrow);
        renderer.ReceiveKey(ConsoleKey.UpArrow);
        renderer.ReceiveKey(ConsoleKey.UpArrow);

        renderer.DomHas.RootNode
            .ThatIsStructured()
            .ThatIsFocusable()
            .WithLayout(Core.Layout.VerticalPreserveHeight)
            .WithUnfocusableChildren()
            .WithChildrenMatching(new() {
                da => da.ThatIsText()
                .WithText("   \\Root")
                .WithUnderline(),
                da => da.ThatIsText()
                .WithText(" →  \\Child-1"),
                da => da.ThatIsText()
                .WithText("      GrandChild-1"),
                da => da.ThatIsText()
                .WithText("      GrandChild-2"),
                da => da.ThatIsText()
                .WithText("      GrandChild-3"),
                da => da.ThatIsText()
                .WithText("     Child-2"),
                da => da.ThatIsText()
                .WithText("     Child-3")
            });
    }
}
