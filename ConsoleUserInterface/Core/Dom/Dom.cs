using LoggingConsole;

namespace ConsoleUserInterface.Core.Dom {

    internal class Dom {
        readonly static ILogger logger = LoggingFactory.Create(typeof(Dom));
        internal IComponent FocusedComponent => mountedComponents[focusedElement!];
        internal IDomNode FocusedNode => nodes[focusedElement!];
        internal IDomNode RootNode => nodes[rootNode.EntryKey];
        internal bool HasChanged { get { var tmp = hasChanged; hasChanged = false; return tmp; } }
        bool hasChanged = true;

        readonly Dictionary<string, object?> props;
        readonly Dictionary<string, object?> states;
        readonly Dictionary<string, IDomNode> nodes;
        readonly Dictionary<string, IComponent> mountedComponents;
        readonly IDomNode.RootNode rootNode;
        string? focusedElement;


        internal Dom(IComponent component) {
            props = new();
            states = new();
            nodes = new();
            mountedComponents = new();

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
            TraverseRecurse(forEach, RootNode, 0);
        }

        internal IEnumerable<IDomNode> ChildNodesOf(IDomNode node) => node switch {
            IDomNode.StructureNode s => s.Children.Select(k => nodes[k]),
            IDomNode.RootNode r => new[] { nodes[r.EntryKey] },
            _ => Enumerable.Empty<IDomNode>()
        };

        void FocusPrevious() {
            var focusedNode = nodes[focusedElement!];
            var chain = Previous(focusedNode.IndexChain, rootNode);
            if (Enumerable.SequenceEqual(chain, focusedNode.IndexChain)) chain = Last(rootNode);
            var newFocus = AtIndexChain(chain, rootNode);

            if (newFocus != focusedElement && nodes[newFocus].SelfFocusable) {
                focusedElement = newFocus;
                hasChanged = true;
            } else if (newFocus != focusedElement) {
                focusedElement = newFocus;
                hasChanged = true;
                FocusPrevious();
            }
        }

        void FocusNext() {
            var focusedNode = nodes[focusedElement!];
            var chain = Next(focusedNode.IndexChain, rootNode);
            if (Enumerable.SequenceEqual(chain, focusedNode.IndexChain)) chain = new() { 0 };
            var newFocus = AtIndexChain(chain, rootNode);

            if (newFocus != focusedElement && nodes[newFocus].SelfFocusable) {
                focusedElement = newFocus;
                hasChanged = true;
            } else if (newFocus != focusedElement) {
                focusedElement = newFocus;
                hasChanged = true;
                FocusNext();
            }
        }


        void TraverseRecurse(Action<IDomNode, object?, object?, bool, int> forEach, IDomNode current, int depth) {
            forEach(current, props[current.Key], states[current.Key], focusedElement == current.Key, depth);
            switch(current) {
                case IDomNode.StructureNode s:
                    s.Children.ForEach(s => TraverseRecurse(forEach, nodes[s], depth + 1));
                    break;
            }
        }

        string AtIndexChain(List<int> indexChain, IDomNode node) => node switch {
            _ when indexChain.Count == 0 => node.Key,
            IDomNode.RootNode n => AtIndexChain(indexChain.Skip(1).ToList(), nodes[n.EntryKey]),
            IDomNode.StructureNode n => AtIndexChain(indexChain.Skip(1).ToList(), nodes[n.Children[indexChain[0]]]),
            _ => throw new Exception(),
        };

        List<int> Previous(List<int> currentElement, IDomNode recurseNode) {
            switch (recurseNode) {
                case IDomNode.RootNode n when currentElement[0] == 0: return Previous(currentElement.Skip(1).ToList(), nodes[n.EntryKey]).Prepend(0).ToList();
                case var _ when currentElement.Count == 0: return currentElement;

                case IDomNode.StructureNode n: {
                        var previousOfChild = Previous(currentElement.Skip(1).ToList(), nodes[n.Children[currentElement[0]]]);
                        if (Enumerable.SequenceEqual(currentElement.Skip(1), previousOfChild)) {
                            if (currentElement[0] > 0 && nodes[n.Children[currentElement[0] - 1]] is IDomNode.StructureNode child && child.Children.Count > 0) {
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
            IDomNode.RootNode r => Last(nodes[r.EntryKey]).Prepend(0).ToList(),
            IDomNode.StructureNode s when s.Children.Count == 0 => new(),
            IDomNode.StructureNode s => Last(nodes[s.Children[^1]]).Prepend(s.Children.Count - 1).ToList(),
            IDomNode.TextNode _ => new(),
            _ => throw new NotImplementedException("Unexpected case reached")
        };

        List<int> Next(List<int> currentElement, IDomNode recurseNode) {
            switch (recurseNode) {
                case IDomNode.RootNode n when currentElement[0] == 0: return Next(currentElement.Skip(1).ToList(), nodes[n.EntryKey]).Prepend(0).ToList();

                case IDomNode.StructureNode n when currentElement.Count == 0 && (n.Children.Count == 0 || !n.ChildrenFocusable):
                case IDomNode.TextNode _ when currentElement.Count == 0: return currentElement;

                case IDomNode.RootNode _ when currentElement.Count == 0:
                case IDomNode.StructureNode _ when currentElement.Count == 0:
                    return new() { 0 };

                case IDomNode.StructureNode n: {
                        var nextOfChild = Next(currentElement.Skip(1).ToList(), nodes[n.Children[currentElement[0]]]);
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
            logger.Debug($"Expanding {key}");
            var expandedChain = indexChain.Append(index).ToList();

            switch (component) {
                case IBaseComponent c: {
                        var result = c.Render();
                        nodes[key] = new IDomNode.TextNode(parentKey, expandedChain, key, result.Text, component.Transform);
                        break;
                    }
                case ICompoundComponent c: {
                        var result = c.Render();
                        var childKeys = result.Components.Select((c, i) => Expand(key, expandedChain, i, c)).ToList();
                        nodes[key] = new IDomNode.StructureNode(parentKey, expandedChain, key, result.SelfFocusable, result.ComponentsFocusable, childKeys, component.Transform, result.Layout, result.ZOffset);
                        break;
                    }
                default: throw new NotImplementedException("Unhandled component type");
            }

            focusedElement ??= key;
            props[key] = component.ComponentProps;
            states[key] = component.ComponentState;
            mountedComponents[key] = component;
            component.OnStateChanged += () => Update(key, component.ComponentProps, component.ComponentState);
            component.OnMounted();
            return key;
        }

        List<string> Remount(string key) {
            logger.Debug($"Remounting {key}");
            var component = mountedComponents[key];

            switch ((component, nodes[key])) {
                case (IBaseComponent c, IDomNode.TextNode node): {
                        var result = c.Render();
                        nodes[key] = node with { Content = result.Text, Transform = component.Transform };
                        mountedComponents[key] = component;
                        return new();
                    }
                case (ICompoundComponent c, IDomNode.StructureNode node): {
                        var result = c.Render();
                        var childKeys = new List<string>();
                        var children = result.Components.ToList();
                        for (var i = 0; i < children.Count; i++) {
                            var child = children[i];
                            var k = CalculateChildKey(key, child, i);
                            if (nodes.ContainsKey(k) && Equals(child.ComponentProps, props[k])) {
                                Update(k, child.ComponentProps, child.ComponentState);    
                            } else {
                                Expand(key, node.IndexChain, i, child);
                            }
                            childKeys.Add(k);
                        }
                        nodes[key] = node with { Children = childKeys, Transform = component.Transform, Layout = result.Layout };
                        mountedComponents[key] = component;
                        return childKeys;
                    }
            }
            return new();
        }

        static string CalculateChildKey(string parentKey, IComponent component, int index) =>
            index == -1 ? $"{parentKey}.{component.TypeName}" : $"{parentKey}[{index} - {component.TypeName}]";

        bool Update(string key, object? p, object? state) {
            if (Equals(p, props[key]) && Equals(state, states[key])) return false;

            hasChanged = true;
            props[key] = p;
            states[key] = state;

            var newChildren = Remount(key);
            switch (nodes[key]) {
                case IDomNode.StructureNode s:
                    var oldChildren = s.Children;
                    var removedChildren = oldChildren.Except(newChildren);
                    foreach(var rem in removedChildren) {
                        Unmount(rem);
                    }
                    break;
            };

            if (!nodes.ContainsKey(focusedElement!)) focusedElement = key;

            return true;
        }

        void Unmount(string key) {
            logger.Debug($"Unmounting {key}");
            var component = mountedComponents[key];
            props.Remove(key);
            states.Remove(key);
            nodes.Remove(key);
            mountedComponents.Remove(key);

            component.OnStateChanged -= () => Update(key, component.ComponentProps, component.ComponentState);
            component.OnUnmounted();
        }
    }
}
