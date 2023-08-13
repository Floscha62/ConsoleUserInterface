using FsCheck;
using FsCheck.Fluent;
using NUnit.Framework;
using C = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleUserInterface.Tests.Components;

[TestFixture]
public class ListSelection {

    [Test]
    public void List_Selection_Starts_On_Index_0() {
        var list = new List<int>() { 1, 2, 3, 4, 5 };
        static void Action(int i) { }
        var comp = C.ListSelection(Core.ITransform.Create(), list, Action);
        var renderer = new TestRenderer(comp);

        renderer.DomHas.RootNode        
            .ThatIsText()
            .WithPropsMatching(p => {
                Assert.IsInstanceOf<Core.Components.ListSelection<int>.Props>(p);
                var props = (p as Core.Components.ListSelection<int>.Props)!;
                Assert.AreEqual(props.StartIndex, 0);
                Assert.AreEqual(props.OnSelectionChanged, (Action<int>)Action);
                Assert.AreEqual(props.Values, list);
            })
            .WithState(new Core.Components.ListSelection<int>.State(0))
            .WithText("> 1\n  2\n  3\n  4\n  5");
    }

    [Test]
    public void List_Selection_Starts_On_Provided_Start_Index() {
        var list = new List<int>() { 1, 2, 3, 4, 5 };
        static void Action(int i) { }
        var comp = C.ListSelection(Core.ITransform.Create(), list, Action, startIndex: 3);
        var renderer = new TestRenderer(comp);

        renderer.DomHas.RootNode
            .ThatIsText()
            .WithPropsMatching(p => {
                Assert.IsInstanceOf<Core.Components.ListSelection<int>.Props>(p);
                var props = (p as Core.Components.ListSelection<int>.Props)!;
                Assert.AreEqual(props.StartIndex, 3);
                Assert.AreEqual(props.OnSelectionChanged, (Action<int>)Action);
                Assert.AreEqual(props.Values, list);
            })
            .WithState(new Core.Components.ListSelection<int>.State(3))
            .WithText("  1\n  2\n  3\n> 4\n  5");
    }

    [Test]
    public void List_Selection_Uses_Provided_Label_Function() {
        var list = new List<int>() { 1, 2, 3, 4, 5 };
        static void Action(int i) { }
        static string ToString(int i) => $"Value {i}";
        var comp = C.ListSelection(Core.ITransform.Create(), list, Action, ToString);
        var renderer = new TestRenderer(comp);

        renderer.DomHas.RootNode
            .ThatIsText()
            .WithPropsMatching(p => {
                Assert.IsInstanceOf<Core.Components.ListSelection<int>.Props>(p);
                var props = (p as Core.Components.ListSelection<int>.Props)!;
                Assert.AreEqual(props.StartIndex, 0);
                Assert.AreEqual(props.OnSelectionChanged, (Action<int>)Action);
                Assert.AreEqual(props.Values, list);
            })
            .WithState(new Core.Components.ListSelection<int>.State(0))
            .WithText("> Value 1\n  Value 2\n  Value 3\n  Value 4\n  Value 5");
    }

    [Test]
    public void List_Selection_Uses_Arrow_Keys_To_Change_Index() {
        var list = new List<int>() { 1, 2, 3, 4, 5 };
        static void Action(int i) { }
        var comp = C.ListSelection(Core.ITransform.Create(), list, Action, startIndex: 1);
        var renderer = new TestRenderer(comp);

        Assert.IsTrue(renderer.ReceiveKey(ConsoleKey.DownArrow));
        Assert.IsTrue(renderer.ReceiveKey(ConsoleKey.DownArrow));

        renderer.DomHas.RootNode
            .ThatIsText()
            .WithPropsMatching(p => {
                Assert.IsInstanceOf<Core.Components.ListSelection<int>.Props>(p);
                var props = (p as Core.Components.ListSelection<int>.Props)!;
                Assert.AreEqual(props.StartIndex, 1);
                Assert.AreEqual(props.OnSelectionChanged, (Action<int>)Action);
                Assert.AreEqual(props.Values, list);
            })
            .WithState(new Core.Components.ListSelection<int>.State(3))
            .WithText("  1\n  2\n  3\n> 4\n  5");

        Assert.IsTrue(renderer.ReceiveKey(ConsoleKey.UpArrow));
        Assert.IsTrue(renderer.ReceiveKey(ConsoleKey.UpArrow));

        renderer.DomHas.RootNode
            .ThatIsText()
            .WithPropsMatching(p => {
                Assert.IsInstanceOf<Core.Components.ListSelection<int>.Props>(p);
                var props = (p as Core.Components.ListSelection<int>.Props)!;
                Assert.AreEqual(props.StartIndex, 1);
                Assert.AreEqual(props.OnSelectionChanged, (Action<int>)Action);
                Assert.AreEqual(props.Values, list);
            })
            .WithState(new Core.Components.ListSelection<int>.State(1))
            .WithText("  1\n> 2\n  3\n  4\n  5");


    }

    [Test]
    public void List_Selection_Calls_Callback_When_Changing_Selection() {
        var count = 0;
        int? last = null;

        var list = new List<int>() { 1, 2, 3, 4, 5 };
        void Action(int i) {
            count++;
            last = i;
        }
        var comp = C.ListSelection(Core.ITransform.Create(), list, Action, startIndex: 1);
        var renderer = new TestRenderer(comp);

        Assert.IsNull(last);
        Assert.AreEqual(count, 0);

        Assert.IsTrue(renderer.ReceiveKey(ConsoleKey.DownArrow));
        Assert.AreEqual(last, 3);
        Assert.AreEqual(count, 1);
        Assert.IsTrue(renderer.ReceiveKey(ConsoleKey.DownArrow));
        Assert.AreEqual(last, 4);
        Assert.AreEqual(count, 2);

        renderer.DomHas.RootNode
            .ThatIsText()
            .WithPropsMatching(p => {
                Assert.IsInstanceOf<Core.Components.ListSelection<int>.Props>(p);
                var props = (p as Core.Components.ListSelection<int>.Props)!;
                Assert.AreEqual(props.StartIndex, 1);
                Assert.AreEqual(props.OnSelectionChanged, (Action<int>)Action);
                Assert.AreEqual(props.Values, list);
            })
            .WithState(new Core.Components.ListSelection<int>.State(3))
            .WithText("  1\n  2\n  3\n> 4\n  5");

        Assert.IsTrue(renderer.ReceiveKey(ConsoleKey.UpArrow));
        Assert.AreEqual(last, 3);
        Assert.AreEqual(count, 3);
        Assert.IsTrue(renderer.ReceiveKey(ConsoleKey.UpArrow));
        Assert.AreEqual(last, 2);
        Assert.AreEqual(count, 4);

        renderer.DomHas.RootNode
            .ThatIsText()
            .WithPropsMatching(p => {
                Assert.IsInstanceOf<Core.Components.ListSelection<int>.Props>(p);
                var props = (p as Core.Components.ListSelection<int>.Props)!;
                Assert.AreEqual(props.StartIndex, 1);
                Assert.AreEqual(props.OnSelectionChanged, (Action<int>)Action);
                Assert.AreEqual(props.Values, list);
            })
            .WithState(new Core.Components.ListSelection<int>.State(1))
            .WithText("  1\n> 2\n  3\n  4\n  5");


    }

    [Test]
    public void List_Selection_Does_Not_Consume_Arrow_Keys_On_List_End() {
        var list = new List<int>() { 1, 2, 3, 4, 5 };
        static void Action(int i) { }
        var comp = C.ListSelection(Core.ITransform.Create(), list, Action);
        var renderer = new TestRenderer(comp);

        renderer.DomHas.RootNode
            .ThatIsText()
            .WithPropsMatching(p => {
                Assert.IsInstanceOf<Core.Components.ListSelection<int>.Props>(p);
                var props = (p as Core.Components.ListSelection<int>.Props)!;
                Assert.AreEqual(props.StartIndex, 0);
                Assert.AreEqual(props.OnSelectionChanged, (Action<int>)Action);
                Assert.AreEqual(props.Values, list);
            })
            .WithState(new Core.Components.ListSelection<int>.State(0))
            .WithText("> 1\n  2\n  3\n  4\n  5");

        Assert.IsFalse(renderer.ReceiveKey(ConsoleKey.UpArrow));

        renderer.ReceiveKey(ConsoleKey.DownArrow);
        renderer.ReceiveKey(ConsoleKey.DownArrow);
        renderer.ReceiveKey(ConsoleKey.DownArrow);
        renderer.ReceiveKey(ConsoleKey.DownArrow);

        renderer.DomHas.RootNode
            .ThatIsText()
            .WithPropsMatching(p => {
                Assert.IsInstanceOf<Core.Components.ListSelection<int>.Props>(p);
                var props = (p as Core.Components.ListSelection<int>.Props)!;
                Assert.AreEqual(props.StartIndex, 0);
                Assert.AreEqual(props.OnSelectionChanged, (Action<int>)Action);
                Assert.AreEqual(props.Values, list);
            })
            .WithState(new Core.Components.ListSelection<int>.State(4))
            .WithText("  1\n  2\n  3\n  4\n> 5");

        Assert.IsFalse(renderer.ReceiveKey(ConsoleKey.DownArrow));
    }

    [FsCheck.NUnit.Property]
    public Property List_Selection_Does_Not_Consume_Other_Keys(ConsoleKey key) {
        var list = new List<int>() { 1, 2, 3, 4, 5 };
        static void Action(int i) { }
        var comp = C.ListSelection(Core.ITransform.Create(), list, Action);
        var renderer = new TestRenderer(comp);

        return (key == ConsoleKey.Tab || key == ConsoleKey.UpArrow || key == ConsoleKey.DownArrow || !renderer.ReceiveKey(key)).ToProperty();
    }
}
