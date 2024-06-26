﻿namespace ConsoleUserInterface.Core.Components;

internal class Container : CompoundComponent<Container.Props, Container.State> {
    internal Container(Props props, ITransform transform) : base(props, transform) {
    }

    public override bool ReceiveKey(ConsoleKeyInfo keyInfo) => false;

    public override CompoundRenderResult Render() => new(props.Children, props.Layout, false, props.ChildrenFocusable, props.ZOffset);

    internal record Props(Layout Layout, List<IComponent> Children, bool ChildrenFocusable = true, int ZOffset = 0) {
        public override string ToString() => $"Props {{ Layout = {Layout}, ChildrenFocusable = {ChildrenFocusable}, ZOffset = {ZOffset} }}";

        public virtual bool Equals(Props? props) =>
            props is not null &&
            Layout == props.Layout &&
            Enumerable.SequenceEqual(Children, props.Children) &&
            ChildrenFocusable == props.ChildrenFocusable &&
            ZOffset == props.ZOffset;

        public override int GetHashCode() =>
            HashCode.Combine(Layout, Children, ChildrenFocusable, ZOffset);
    }
    internal record State();
}
