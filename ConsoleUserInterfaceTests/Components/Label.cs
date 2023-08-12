using FsCheck;
using FsCheck.Fluent;
using NUnit.Framework;
using C = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleUserInterface.Tests.Components;

[TestFixture]
public class Label {

    [Test]
    public void Label_Renders_Text_From_Props() {
        var renderer = new TestRenderer(C.Label(Core.ITransform.Create(), "Label Text", false));

        renderer.DomHas.FocusedNode
            .ThatIsText()
            .WithProps(new Core.Components.Label.Props("Label Text", false))
            .WithText("Label Text")
            .IsNotUnderlined();
    }

    [Test]
    public void Label_Renders_With_Underlined() {
        var renderer = new TestRenderer(C.Label(Core.ITransform.Create(), "Label Text", true));

        renderer.DomHas.FocusedNode
            .ThatIsText()
            .WithProps(new Core.Components.Label.Props("Label Text", true))
            .WithText("Label Text")
            .WithUnderline();
    }

    [FsCheck.NUnit.Property]
    public Property Label_Does_Not_Consume_Keys(ConsoleKey key) {
        var renderer = new TestRenderer(C.Label(Core.ITransform.Create(), "Label Text", false));

        return (key == ConsoleKey.Tab || renderer.ReceiveKey(key) == false).ToProperty();
    }
}
