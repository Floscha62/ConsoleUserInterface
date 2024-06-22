using ConsoleUserInterface.Core.Dom;
using ConsoleUserInterface.Core;
using NUnit.Framework;

namespace ConsoleUserInterface.Tests;

internal class TestRenderer {

    private readonly Dom dom;

    internal DomAssert DomHas => new(dom);

    internal TestRenderer(IComponent component) {
        dom = new(component);
    }

    internal bool ReceiveText(string text) => 
        text.Select(c => new ConsoleKeyInfo(c, 
            char.IsLetter(c) ?
                Enum.Parse<ConsoleKey>(c.ToString(), true) :
                ConsoleKey.Spacebar,
            false, false, false))
            .Aggregate(true, (a, c) => dom.ReceiveKey(c) && a);

    internal bool ReceiveKey(ConsoleKey key) => dom.ReceiveKey(new ConsoleKeyInfo('\0', key, false, false, false));
}

internal class DomAssert {

    private readonly Dom dom;

    internal DomAssert(Dom dom) {
        this.dom = dom;
    }

    internal DomNodeAssert FocusedNode => new(dom, dom.FocusedNode);

    internal DomNodeAssert RootNode => new(dom, dom[dom.rootNode.EntryKey].node);

}

internal class DomNodeAssert : NodeAssert<DomNodeAssert> {
    internal DomNodeAssert(Dom dom, IDomNode node) : base(dom, node) {
    }
}

internal class NodeAssert<N> where N : NodeAssert<N> {

    protected readonly Dom dom;
    private readonly IDomNode node;

    internal NodeAssert(Dom dom, IDomNode node) {
        this.dom = dom;
        this.node = node;
    }

    internal N WithKey(string key) {
        Assert.AreEqual(key, node.Key);
        return (N)this;
    }

    internal N WithProps(object? props) {
        var nodeProps = dom[node.Key].props; 
        Assert.AreEqual(props, nodeProps); 
        return (N)this;
    }

    internal N WithState(object? state) {
        var nodeState = dom[node.Key].state;
        Assert.AreEqual(state, nodeState);
        return (N)this;
    }

    internal N WithPropsMatching(Action<object?> matcher) {
        matcher(dom[node.Key].props);
        return (N)this;
    }

    internal N WithStateMatching(Action<object?> matcher) {
        matcher(dom[node.Key].state);
        return (N)this;
    }

    internal TextNodeAssert ThatIsText() {
        Assert.IsInstanceOf<IDomNode.TextNode>(node);
        return new TextNodeAssert(dom, (node as IDomNode.TextNode)!);
    }

    internal StructureNodeAssert ThatIsStructured() {
        Assert.IsInstanceOf<IDomNode.StructureNode>(node);
        return new StructureNodeAssert(dom, (node as  IDomNode.StructureNode)!);
    }

    internal N WithParentMatching(Action<DomNodeAssert> parentMatcher) {
        Assert.NotNull(node.ParentKey, "Tried to match parent on root node");
        parentMatcher(new(dom, dom[node.ParentKey!].node));
        return (N)this;
    }
}

internal class TextNodeAssert : NodeAssert<TextNodeAssert> {

    private readonly IDomNode.TextNode node;

    internal TextNodeAssert(Dom dom, IDomNode.TextNode node) : base(dom, node) {
        this.node = node;
    }

    internal TextNodeAssert WithText(string text) {
        Assert.AreEqual(node.Content, text);
        return this;
    }

    internal TextNodeAssert WithUnderline() {
        Assert.IsTrue(node.Underlined);
        return this;
    }

    internal TextNodeAssert IsNotUnderlined() {
        Assert.IsFalse(node.Underlined);
        return this;
    }
}

internal class StructureNodeAssert : NodeAssert<StructureNodeAssert> {

    private readonly IDomNode.StructureNode node;

    internal StructureNodeAssert(Dom dom, IDomNode.StructureNode node) : base(dom, node) {
        this.node = node;
    }

    internal StructureNodeAssert WithChildrenMatching(List<Action<DomNodeAssert>> matchers) {
        Assert.AreEqual(node.Children.Count, matchers.Count);
        for (int i = 0; i < matchers.Count; i++) {
            matchers[i](new DomNodeAssert(dom, dom[node.Children[i]].node));
        }
        return this;
    }

    internal StructureNodeAssert WithLayout(Core.Layout layout) {
        Assert.AreEqual(node.Layout, layout);
        return this;
    }

    internal StructureNodeAssert WithZOffset(int zOffset) {
        Assert.AreEqual(node.ZOffset, zOffset);
        return this;
    }

    internal StructureNodeAssert ThatIsFocusable() {
        Assert.IsTrue(node.SelfFocusable); return this;
    }

    internal StructureNodeAssert ThatIsNotFocusable() {
        Assert.IsFalse(node.SelfFocusable); return this;
    }

    internal StructureNodeAssert WithFocusableChildren() {
        Assert.IsTrue(node.ChildrenFocusable); return this;
    }

    internal StructureNodeAssert WithUnfocusableChildren() {
        Assert.IsFalse(node.ChildrenFocusable); return this;
    }
}
