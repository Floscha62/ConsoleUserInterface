using ConsoleUserInterface.Core;
using FsCheck;
using FsCheck.Fluent;
using NUnit.Framework;
using C = ConsoleUserInterface.Core.Components.Components;

namespace ConsoleUserInterface.Tests.Components;

[TestFixture]
public class Container {

    [Test]
    public void Container_Renders_Children_Directly() {
        var comp = C.Container(ITransform.Create(), Core.Layout.Vertical, true, 
            C.Label(ITransform.Create(1), "Label 1", false),    
            C.Label(ITransform.Create(1), "Label 2", false),    
            C.Label(ITransform.Create(1), "Label 3", false),    
            C.Label(ITransform.Create(1), "Label 4", false),    
            C.Label(ITransform.Create(1), "Label 5", false)    
        );

        var renderer = new TestRenderer(comp);

        renderer.DomHas.RootNode
            .ThatIsStructured()
            .WithChildrenMatching(new() {
                a => a.ThatIsText().WithText("Label 1"),
                a => a.ThatIsText().WithText("Label 2"),
                a => a.ThatIsText().WithText("Label 3"),
                a => a.ThatIsText().WithText("Label 4"),
                a => a.ThatIsText().WithText("Label 5")
            });
    }

    [FsCheck.NUnit.Property]
    public void Container_Uses_Provided_Layout(Core.Layout layout) {
        var comp = C.Container(ITransform.Create(), layout, true);

        var renderer = new TestRenderer(comp);

        renderer.DomHas.RootNode
            .ThatIsStructured()
            .WithLayout(layout);
    }

    [Test]
    public void Container_Is_Not_Focusable() {
        var comp = C.Container(ITransform.Create(), Core.Layout.Vertical, true);

        var renderer = new TestRenderer(comp);

        renderer.DomHas.RootNode
            .ThatIsStructured()
            .ThatIsNotFocusable();
    }

    [Test]
    public void Container_Has_Focusable_Children_If_Provided() {
        var comp = C.Container(ITransform.Create(), Core.Layout.Vertical, true);

        var renderer = new TestRenderer(comp);

        renderer.DomHas.RootNode
            .ThatIsStructured()
            .WithFocusableChildren();

        comp = C.Container(ITransform.Create(), Core.Layout.Vertical, false);

        renderer = new TestRenderer(comp);

        renderer.DomHas.RootNode
            .ThatIsStructured()
            .WithUnfocusableChildren();
    }

    [FsCheck.NUnit.Property]
    public Property Container_Does_Not_Handle_Keys(ConsoleKey key) {
        var comp = C.Container(ITransform.Create(), Core.Layout.Vertical, true, C.Label(ITransform.Create(1), "abc", false));

        var renderer = new TestRenderer(comp);

        return (key == ConsoleKey.Tab || !renderer.ReceiveKey(key)).ToProperty();
    }
}
