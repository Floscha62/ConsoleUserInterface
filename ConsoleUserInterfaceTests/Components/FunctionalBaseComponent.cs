using ConsoleUserInterface.Core;
using NUnit.Framework;
using System.Net.NetworkInformation;
using C = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleUserInterface.Tests.Components;

[TestFixture]
internal class FunctionalBaseComponent {

    [Test]
    public void Functional_Base_Component_Renders_Text() {
        static BaseRenderResult SimpleText(object? _, object? __, Action<object?> ___, Callbacks ____) => new(
                "This is the text to be rendered!"
                );

        var comp = C.FunctionComponent<object?, object?>(ITransform.Create(1), null, SimpleText);
        var renderer = new TestRenderer(comp);

        renderer.DomHas.RootNode
            .ThatIsText()
            .WithText("This is the text to be rendered!")
            .IsNotUnderlined();
    }

    [Test]
    public void Functional_Base_Component_Renders_Underlined() {
        static BaseRenderResult SimpleText(object? _, object? __, Action<object?> ___, Callbacks ____) => new(
                "This is the text to be rendered!",
                true
                );

        var comp = C.FunctionComponent<object?, object?>(ITransform.Create(1), null, SimpleText);
        var renderer = new TestRenderer(comp);

        renderer.DomHas.RootNode
            .ThatIsText()
            .WithText("This is the text to be rendered!")
            .WithUnderline();
    }

    [Test]
    public void Functional_Base_Component_Renders_With_Provided_Props() {
        static BaseRenderResult SimpleText((string text, bool underlined) props, object? _, Action<object?> __, Callbacks ___) => new(
                props.text,
                props.underlined
            );

        var comp = C.FunctionComponent<(string, bool), object?>(ITransform.Create(1), ("Text provided by props", false), SimpleText);
        var renderer = new TestRenderer(comp);

        renderer.DomHas.RootNode
            .ThatIsText()
            .WithText("Text provided by props")
            .IsNotUnderlined();
    }

    [Test]
    public void Functional_Base_Component_Renders_With_Provided_Initial_State() {
        static BaseRenderResult SimpleText(object? _, (string text, bool underlined) state, Action<(string, bool)> __, Callbacks ___) => new(
                state.text,
                state.underlined
            );

        var comp = C.FunctionComponent<object?, (string, bool)>(ITransform.Create(1), null, SimpleText, ("Text provided by initial state", false));
        var renderer = new TestRenderer(comp);
        renderer.DomHas.RootNode
            .ThatIsText()
            .WithText("Text provided by initial state")
            .IsNotUnderlined();
    }

    [Test]
    public void Function_Base_Component_Handles_Keys() {
        var pressed = 0;
        void Callback() { pressed++; }

        static BaseRenderResult SimpleButton(Action _, object? __, Action<object?> ___, Callbacks ____) => new("Button");
        static bool KeyHandler(ConsoleKeyInfo key, Action props, object? _, Action<object?> __) {
            if (key.Key == ConsoleKey.Enter) {
                props();
                return true;
            } else {
                return false;
            }
        }

        var comp = C.FunctionComponent<Action, object?>(ITransform.Create(1), Callback, SimpleButton, handleKeys: KeyHandler);
        var renderer = new TestRenderer(comp);

        renderer.DomHas.RootNode
            .ThatIsText()
            .WithText("Button");
        Assert.That(pressed, Is.EqualTo(0));

        Assert.That(renderer.ReceiveKey(ConsoleKey.Enter), Is.True);

        renderer.DomHas.RootNode
            .ThatIsText()
            .WithText("Button");
        Assert.That(pressed, Is.EqualTo(1));

        Assert.That(renderer.ReceiveKey(ConsoleKey.Spacebar), Is.False);
        renderer.DomHas.RootNode
            .ThatIsText()
            .WithText("Button");
        Assert.That(pressed, Is.EqualTo(1));
    }

    [Test]
    public void Function_Base_Component_Handles_State_Updates() {
        static BaseRenderResult SimpleCounter(object? _, int state, Action<int> __, Callbacks ___) => new(state.ToString());
        static bool KeyHandler(ConsoleKeyInfo key, object? _, int state, Action<int> updateState) {
            if (key.Key == ConsoleKey.OemPlus) {
                updateState(state + 1);
                return true;
            } else if (key.Key == ConsoleKey.OemMinus) {
                updateState(state - 1);
                return true;
            } else { return false; }
        }

        var comp = C.FunctionComponent<object?, int>(ITransform.Create(1), null, SimpleCounter, handleKeys: KeyHandler);
        var renderer = new TestRenderer(comp);

        renderer.DomHas.RootNode
            .ThatIsText()
            .WithState(0)
            .WithText("0");

        Assert.IsTrue(renderer.ReceiveKey(ConsoleKey.OemPlus));
        renderer.DomHas.RootNode
            .ThatIsText()
            .WithState(1)
            .WithText("1");

        Assert.IsTrue(renderer.ReceiveKey(ConsoleKey.OemMinus));
        renderer.DomHas.RootNode
            .ThatIsText()
            .WithState(0)
            .WithText("0");
    }

    [Test]
    public void Function_Base_Component_Handles_State_Updates_On_Previous() {
        static BaseRenderResult SimpleCounter(object? _, int state, Action<Func<int, int>> ___, Callbacks ____) => new(state.ToString());
        static bool KeyHandler(ConsoleKeyInfo key, object? _, int __, Action<Func<int, int>> updateState) {
            if (key.Key == ConsoleKey.OemPlus) {
                updateState(state => state + 1);
                return true;
            } else if (key.Key == ConsoleKey.OemMinus) {
                updateState(state => state - 1);
                return true;
            } else { return false; }
        }

        var comp = C.FunctionComponent<object?, int>(ITransform.Create(1), null, SimpleCounter, handleKeys: KeyHandler);
        var renderer = new TestRenderer(comp);


        renderer.DomHas.RootNode
            .ThatIsText()
            .WithState(0)
            .WithText("0");

        Assert.IsTrue(renderer.ReceiveKey(ConsoleKey.OemPlus));
        renderer.DomHas.RootNode
            .ThatIsText()
            .WithState(1)
            .WithText("1");

        Assert.IsTrue(renderer.ReceiveKey(ConsoleKey.OemMinus));
        renderer.DomHas.RootNode
            .ThatIsText()
            .WithState(0)
            .WithText("0");
    }
}
