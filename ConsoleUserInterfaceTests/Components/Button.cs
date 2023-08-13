using FsCheck;
using FsCheck.Fluent;
using NUnit.Framework;
using C = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleUserInterface.Tests.Components;

[TestFixture]
public class Button {

    [Test]
    public void Button_Renders_Like_Label() {
        static void Action() { }

        var renderer = new TestRenderer(C.Button(Core.ITransform.Create(), "Button Text", Action));

        renderer.DomHas.RootNode
            .ThatIsText()
            .WithProps(new Core.Components.Button.Props("Button Text", Action))
            .WithText("Button Text")
            .IsNotUnderlined();
    }

    [FsCheck.NUnit.Property]
    public Property Button_Does_Not_Handle_Other_Keys(ConsoleKey key) {
        static void Action() { }

        var renderer = new TestRenderer(C.Button(Core.ITransform.Create(), "Button Text", Action));

        return (key == ConsoleKey.Tab || key == ConsoleKey.Enter || !renderer.ReceiveKey(key)).ToProperty();
    }

    [Test]
    public void Button_Triggers_Callback_On_Enter() {
        bool called = false;
        void Action() => called = true;

        var renderer = new TestRenderer(C.Button(Core.ITransform.Create(), "Button Text", Action));

        renderer.ReceiveKey(ConsoleKey.Enter);

        Assert.IsTrue(called);
    }

    [Test]
    public void Button_Triggers_Callback_Repeatedly_On_Repeated_Enter() {
        int called = 0;
        void Action() => called ++;

        var renderer = new TestRenderer(C.Button(Core.ITransform.Create(), "Button Text", Action));

        renderer.ReceiveKey(ConsoleKey.Enter);
        renderer.ReceiveKey(ConsoleKey.Enter);
        renderer.ReceiveKey(ConsoleKey.Enter);
        renderer.ReceiveKey(ConsoleKey.Enter);

        Assert.AreEqual(called, 4);
    }
}