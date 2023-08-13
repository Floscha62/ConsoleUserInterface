using LoggingConsole;

namespace ConsoleUserInterface.Core.Dom;

internal class Dom {
    readonly static ILogger logger = LoggingFactory.Create(typeof(Dom));
    internal IComponent FocusedComponent => mountContexts[focusedElement!].Component;
    internal IDomNode FocusedNode => mountContexts[focusedElement!].Node;
    internal bool HasChanged { get { var tmp = hasChanged; hasChanged = false; return tmp; } }
    bool hasChanged = true;

    internal readonly IDomNode.RootNode rootNode;

    readonly Dictionary<string, MountContext> mountContexts;
    readonly Dictionary<string, Action> updates;
    string? focusedElement;

    private record MountContext(object? Props, object? State, IDomNode Node, IComponent Component);

    internal (IDomNode node, object? props, object? state, bool focused) this[string key] {
        get {
            var ctx = mountContexts[key];

            return (ctx.Node, ctx.Props, ctx.State, Equals(focusedElement, key));
        }
    }

    internal Dom(IComponent component) {
        mountContexts = new();
        updates = new();

        rootNode = new(Expand("", new(), 0, component));
    }

    internal bool ReceiveKey(ConsoleKeyInfo info) {
        if (info.Key == ConsoleKey.Tab && info.Modifiers == 0) {
            FocusNext();
            return true;
        }
        if (info.Key == ConsoleKey.Tab && info.Modifiers == ConsoleModifiers.Control) {
            FocusPrevious();
            return true;
        }
        var focusedComponent = FocusedComponent;
        return focusedComponent.ReceiveKey(info);
    }

    internal void Traverse(Action<IDomNode, object?, object?, bool, int> forEach) {
        TraverseRecurse(forEach, mountContexts[rootNode.EntryKey], 0);
    }

    internal IEnumerable<IDomNode> ChildNodesOf(IDomNode node) => node switch {
        IDomNode.StructureNode s => s.Children.Select(k => mountContexts[k].Node),
        IDomNode.RootNode r => new[] { mountContexts[r.EntryKey].Node },
        _ => Enumerable.Empty<IDomNode>()
    };

    void FocusPrevious() {
        var focusedNode = mountContexts[focusedElement!].Node;
            var chain = Previous(focusedNode.IndexChain, rootNode);
            if (Enumerable.SequenceEqual(chain, focusedNode.IndexChain)) chain = Last(rootNode);
            var newFocus = AtIndexChain(chain, rootNode);

        if (newFocus != focusedElement && mountContexts[newFocus].Node.SelfFocusable) {
            focusedElement = newFocus;
            hasChanged = true;
        } else if (newFocus != focusedElement) {
            focusedElement = newFocus;
            hasChanged = true;
            FocusPrevious();
        }
    }

    void FocusNext() {
        var focusedNode = mountContexts[focusedElement!].Node;
        var chain = Next(focusedNode.IndexChain, rootNode);
        if (Enumerable.SequenceEqual(chain, focusedNode.IndexChain)) chain = new() { 0 };
        var newFocus = AtIndexChain(chain, rootNode);

        if (newFocus != focusedElement && mountContexts[newFocus].Node.SelfFocusable) {
            focusedElement = newFocus;
            hasChanged = true;
        } else if (newFocus != focusedElement) {
            focusedElement = newFocus;
            hasChanged = true;
            FocusNext();
        }
    }

    void TraverseRecurse(Action<IDomNode, object?, object?, bool, int> forEach, MountContext current, int depth) {
        forEach(current.Node, current.Props, current.State, focusedElement == current.Node.Key, depth);
        switch (current.Node) {
            case IDomNode.StructureNode s:
                s.Children.ForEach(s => TraverseRecurse(forEach, mountContexts[s], depth + 1));
                break;
        }
    }

    string AtIndexChain(List<int> indexChain, IDomNode node) => node switch {
        _ when indexChain.Count == 0 => node.Key,
        IDomNode.RootNode n => AtIndexChain(indexChain.Skip(1).ToList(), mountContexts[n.EntryKey].Node),
        IDomNode.StructureNode n => AtIndexChain(indexChain.Skip(1).ToList(), mountContexts[n.Children[indexChain[0]]].Node),
        _ => throw new Exception(),
    };

    List<int> Previous(List<int> currentElement, IDomNode recurseNode) {
        switch (recurseNode) {
            case IDomNode.RootNode n when currentElement[0] == 0: return Previous(currentElement.Skip(1).ToList(), mountContexts[n.EntryKey].Node).Prepend(0).ToList();
            case var _ when currentElement.Count == 0: return currentElement;

            case IDomNode.StructureNode n: {
                    var previousOfChild = Previous(currentElement.Skip(1).ToList(), mountContexts[n.Children[currentElement[0]]].Node);
                    if (Enumerable.SequenceEqual(currentElement.Skip(1), previousOfChild)) {
                        if (currentElement[0] > 0 && mountContexts[n.Children[currentElement[0] - 1]].Node is IDomNode.StructureNode child && child.Children.Count > 0) {
                            return new List<int>() { currentElement[0] - 1 }.Concat(Last(child)).ToList();
                        } else if (currentElement[0] > 0) {
                            return new List<int>() { currentElement[0] - 1 };
                        } else {
                            return currentElement.Take(currentElement.Count - 1).ToList();
                        }
                    } else {
                        return previousOfChild.Prepend(currentElement[0]).ToList();
                    }
                }
            default: throw new NotImplementedException("Unexpected case reached");
        }
    }

    List<int> Last(IDomNode recurseNode) => recurseNode switch {
        IDomNode.RootNode r => Last(mountContexts[r.EntryKey].Node).Prepend(0).ToList(),
        IDomNode.StructureNode s when s.Children.Count == 0 => new(),
        IDomNode.StructureNode s => Last(mountContexts[s.Children[^1]].Node).Prepend(s.Children.Count - 1).ToList(),
        IDomNode.TextNode _ => new(),
        _ => throw new NotImplementedException("Unexpected case reached")
    };

    List<int> Next(List<int> currentElement, IDomNode recurseNode) {
        switch (recurseNode) {
            case IDomNode.RootNode n when currentElement[0] == 0: return Next(currentElement.Skip(1).ToList(), mountContexts[n.EntryKey].Node).Prepend(0).ToList();

            case IDomNode.StructureNode n when currentElement.Count == 0 && (n.Children.Count == 0 || !n.ChildrenFocusable):
            case IDomNode.TextNode _ when currentElement.Count == 0: return currentElement;

            case IDomNode.RootNode _ when currentElement.Count == 0:
            case IDomNode.StructureNode _ when currentElement.Count == 0:
                return new() { 0 };

            case IDomNode.StructureNode n: {
                    var nextOfChild = Next(currentElement.Skip(1).ToList(), mountContexts[n.Children[currentElement[0]]].Node);
                    if (Enumerable.SequenceEqual(currentElement.Skip(1), nextOfChild)) {
                        if (currentElement[0] < n.Children.Count - 1) {
                            return new List<int>() { currentElement[0] + 1 };
                        } else {
                            return currentElement;
                        }
                    } else {
                        return nextOfChild.Prepend(currentElement[0]).ToList();
                    }
                }
            default: throw new NotImplementedException("Unexpected case reached");
        }
    }

    string Expand(string parentKey, List<int> indexChain, int index, IComponent component) {
        _ = component ?? throw new ArgumentException("Cannot render null component", nameof(component));

        var key = CalculateChildKey(parentKey, component, index);

        if (mountContexts.ContainsKey(key) && Equals(component.ComponentProps, mountContexts[key].Props)) {
            if(!Equals(component.ComponentState, mountContexts[key].State)) {
                hasChanged = true;
            }

            var newChildren = Remount(key);
            switch (mountContexts[key].Node) {
                case IDomNode.StructureNode s:
                    var oldChildren = s.Children;
                    var removedChildren = oldChildren.Except(newChildren);
                    foreach (var rem in removedChildren) {
                        Unmount(rem);
                    }
                    break;
            };

            if (!mountContexts.ContainsKey(focusedElement!)) focusedElement = key;
            return key;
        }

        hasChanged = true;

        if (mountContexts.ContainsKey(key)) Unmount(key);

        logger.Debug($"Expanding {key}");
        var expandedChain = indexChain.Append(index).ToList();

        var node = Render(component, parentKey, expandedChain, key);

        mountContexts[key] = new(component.ComponentProps, component.ComponentState, node, component);
        component.OnStateChanged += updates[key] = () => Expand(parentKey, indexChain, index, component);
        component.OnMounted();
        return key;
    }

    IDomNode Render(IComponent component, string parentKey, List<int> expandedChain, string key) {
        switch (component) {
            case IBaseComponent c: {
                    var result = c.Render();
                    focusedElement ??= key;
                    return new IDomNode.TextNode(parentKey, expandedChain, key, result.Text, result.Underlined, component.Transform);
                }
            case ICompoundComponent c: {
                    var result = c.Render();
                    if (result.SelfFocusable) focusedElement ??= key;

                    var childKeys = result.Components.Select((c, i) => Expand(key, expandedChain, i, c)).ToList();
                    return new IDomNode.StructureNode(parentKey, expandedChain, key, result.SelfFocusable, result.ComponentsFocusable, childKeys, component.Transform, result.Layout, result.ZOffset);
                }
            default: throw new NotImplementedException("Unhandled component type");
        }
    }

    List<string> Remount(string key) {
        logger.Debug($"Remounting {key}");
        var ctx = mountContexts[key];

        switch ((ctx.Component, ctx.Node)) {
            case (IBaseComponent c, IDomNode.TextNode node): {
                    var result = c.Render();
                    mountContexts[key] = ctx with { Node = node with { Content = result.Text, Transform = ctx.Component.Transform } };
                    return new();
                }
            case (ICompoundComponent c, IDomNode.StructureNode node): {
                    var result = c.Render();
                    var childKeys = new List<string>();
                    var children = result.Components.ToList();
                    for (var i = 0; i < children.Count; i++) {
                        var child = children[i];
                        var k = CalculateChildKey(key, child, i);
                        Expand(key, node.IndexChain, i, child);
                        childKeys.Add(k);
                    }
                    mountContexts[key] = ctx with { Node = node with { Children = childKeys, Transform = ctx.Component.Transform, Layout = result.Layout } };
                    return childKeys;
                }
        }
        return new();
    }

    static string CalculateChildKey(string parentKey, IComponent component, int index) =>
        index == -1 ? $"{parentKey}.{component.TypeName}" : $"{parentKey}[{index} - {component.TypeName}]";

    void Unmount(string key) {
        logger.Debug($"Unmounting {key}");
        var component = mountContexts[key].Component;
        mountContexts.Remove(key);
        hasChanged = true;

        if (updates.Remove(key, out var update)) {
            component.OnStateChanged -= update;
        }
        component.OnUnmounted();
    }
}
