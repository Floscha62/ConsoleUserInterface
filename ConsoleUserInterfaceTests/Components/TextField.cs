using NUnit.Framework;
using C = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleUserInterface.Tests.Components;

[TestFixture]
public class TextField {

    [Test]
    public void Text_Field_Is_Rendered_With_Start_Text() {
        static void Action(string s) { }
        var renderer = new TestRenderer(C.TextField(Core.ITransform.Create(), "This is a starting text", Action));

        renderer.DomHas.RootNode
            .ThatIsText()
            .WithProps(new Core.Components.TextField.Props("This is a starting text", Action))
            .WithText("This is a starting text|");
    }

    [Test]
    public void Text_Field_Reacts_To_Typed_Text() {
        static void Action(string s) { }
        var renderer = new TestRenderer(C.TextField(Core.ITransform.Create(), "", Action));

        renderer.DomHas.RootNode
            .ThatIsText()
            .WithProps(new Core.Components.TextField.Props("", Action))
            .WithText("|");

        renderer.ReceiveText("This is a typed text");

        renderer.DomHas.RootNode
            .ThatIsText()
            .WithProps(new Core.Components.TextField.Props("", Action))
            .WithText("This is a typed text|");
    }

    [Test]
    public void Text_Field_Reacts_To_Backspaces() {
        static void Action(string s) { }
        var renderer = new TestRenderer(C.TextField(Core.ITransform.Create(), "Abcdef", Action));

        renderer.ReceiveKey(ConsoleKey.Backspace);
        renderer.ReceiveKey(ConsoleKey.Backspace);
        renderer.ReceiveKey(ConsoleKey.Backspace);

        renderer.DomHas.RootNode
            .ThatIsText()
            .WithProps(new Core.Components.TextField.Props("Abcdef", Action))
            .WithText("Abc|");
    }

    [Test]
    public void Text_Field_Reacts_To_Arrow_Keys() {
        static void Action(string s) { }
        var renderer = new TestRenderer(C.TextField(Core.ITransform.Create(), "", Action));

        renderer.ReceiveText("Abcdef");
        renderer.DomHas.RootNode
            .ThatIsText()
            .WithProps(new Core.Components.TextField.Props("", Action))
            .WithText("Abcdef|");

        renderer.ReceiveKey(ConsoleKey.LeftArrow);
        renderer.DomHas.RootNode
            .ThatIsText()
            .WithProps(new Core.Components.TextField.Props("", Action))
            .WithText("Abcde|f");

        renderer.ReceiveKey(ConsoleKey.RightArrow);
        renderer.DomHas.RootNode
            .ThatIsText()
            .WithProps(new Core.Components.TextField.Props("", Action))
            .WithText("Abcdef|");
    }

    [Test]
    public void Text_Field_Inserts_Text_At_Cursor() {
        static void Action(string s) { }
        var renderer = new TestRenderer(C.TextField(Core.ITransform.Create(), "", Action));

        renderer.ReceiveText("Abcdef");
        renderer.ReceiveKey(ConsoleKey.LeftArrow);
        renderer.ReceiveKey(ConsoleKey.LeftArrow);
        renderer.DomHas.RootNode
            .ThatIsText()
            .WithProps(new Core.Components.TextField.Props("", Action))
            .WithText("Abcd|ef");

        renderer.ReceiveText("de");
        renderer.DomHas.RootNode
            .ThatIsText()
            .WithProps(new Core.Components.TextField.Props("", Action))
            .WithText("Abcdde|ef");

        renderer.ReceiveKey(ConsoleKey.RightArrow);
        renderer.ReceiveKey(ConsoleKey.RightArrow);
        renderer.DomHas.RootNode
            .ThatIsText()
            .WithProps(new Core.Components.TextField.Props("", Action))
            .WithText("Abcddeef|");
    }

    [Test]
    public void Text_Field_Triggers_Callback_On_Text() {
        var calls = 0;
        void Action(string s) => calls++;
        var renderer = new TestRenderer(C.TextField(Core.ITransform.Create(), "", Action));

        Assert.AreEqual(calls, 0);
        renderer.ReceiveText("Abcdefghij");

        Assert.AreEqual(calls, 10);
    }

    [Test]
    public void Text_Field_Triggers_Callback_On_Backspace() {
        var calls = 0;
        void Action(string s) => calls++;
        var renderer = new TestRenderer(C.TextField(Core.ITransform.Create(), "Abcdefghij", Action));

        Assert.AreEqual(calls, 0);
        renderer.ReceiveKey(ConsoleKey.Backspace);

        Assert.AreEqual(calls, 1);
    }

    [Test]
    public void Text_Field_Does_Not_Trigger_Callback_On_Arrow_Keys() {
        var calls = 0;
        void Action(string s) => calls++;
        var renderer = new TestRenderer(C.TextField(Core.ITransform.Create(), "Abcdefghij", Action));

        renderer.ReceiveKey(ConsoleKey.LeftArrow);
        renderer.ReceiveKey(ConsoleKey.LeftArrow);
        renderer.ReceiveKey(ConsoleKey.LeftArrow);
        renderer.ReceiveKey(ConsoleKey.RightArrow);
        renderer.ReceiveKey(ConsoleKey.RightArrow);
        renderer.ReceiveKey(ConsoleKey.RightArrow);

        Assert.AreEqual(calls, 0);
    }

    [Test]
    public void Text_Field_Does_Trigger_Callback_With_The_Text_After_Key() {
        var lastCall = "";
        void Action(string s) => lastCall = s;
        var renderer = new TestRenderer(C.TextField(Core.ITransform.Create(), "Abc", Action));

        renderer.ReceiveText("def");

        Assert.AreEqual(lastCall, "Abcdef");

        renderer.ReceiveKey(ConsoleKey.Backspace);
        renderer.ReceiveKey(ConsoleKey.Backspace);

        Assert.AreEqual(lastCall, "Abcd");
    }
}
