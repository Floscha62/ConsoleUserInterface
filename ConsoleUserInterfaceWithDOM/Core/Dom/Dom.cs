using LoggingConsole;

namespace ConsoleUserInterfaceWithDOM.Core.Dom {

    internal class Dom {
        private readonly static ILogger logger = LoggingFactory.Create(typeof(Dom));
        internal IComponent FocusedComponent => mountedComponents[focusedElement!];
        internal IDomNode FocusedNode => nodes[focusedElement!];
        internal IDomNode RootNode => nodes[rootNode.EntryKey];

        readonly Dictionary<string, IPropsAndState> propsAndStates;
        readonly Dictionary<string, IDomNode> nodes;
        readonly Dictionary<string, IComponent> mountedComponents;
        readonly RootNode rootNode;
        string? focusedElement;

        internal bool HasChanged { get { var tmp = hasChanged; hasChanged = false; return tmp; } }
        private bool hasChanged = true;

        internal Dom(IComponent component) {
            propsAndStates = new();
            nodes = new();
            mountedComponents = new();

            rootNode = new(Expand("", new(), 0, component));
        }

        internal void FocusPrevious() {
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

        internal void FocusNext() {
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

        internal string AtIndexChain(List<int> indexChain, IDomNode node) => node switch {
            _ when indexChain.Count == 0 => node.Key,
            RootNode n => AtIndexChain(indexChain.Skip(1).ToList(), nodes[n.EntryKey]),
            StructureNode n => AtIndexChain(indexChain.Skip(1).ToList(), nodes[n.Children[indexChain[0]]]),
            _ => throw new Exception(),
        };

        internal List<int> Previous(List<int> currentElement, IDomNode recurseNode) {
            switch (recurseNode) {
                case RootNode n when currentElement[0] == 0: return Previous(currentElement.Skip(1).ToList(), nodes[n.EntryKey]).Prepend(0).ToList();
                case var _ when currentElement.Count == 0: return currentElement;

                case StructureNode n: {
                        var previousOfChild = Previous(currentElement.Skip(1).ToList(), nodes[n.Children[currentElement[0]]]);
                        if (Enumerable.SequenceEqual(currentElement.Skip(1), previousOfChild)) {
                            if (currentElement[0] > 0 && nodes[n.Children[currentElement[0] - 1]] is StructureNode child && child.Children.Count > 0) {
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

        internal List<int> Last(IDomNode recurseNode) => recurseNode switch {
            RootNode r => Last(nodes[r.EntryKey]).Prepend(0).ToList(),
            StructureNode s when s.Children.Count == 0 => new(),
            StructureNode s => Last(nodes[s.Children[^1]]).Prepend(s.Children.Count - 1).ToList(),
            TextNode _ => new(),
            _ => throw new NotImplementedException("Unexpected case reached")
        };

        internal List<int> Next(List<int> currentElement, IDomNode recurseNode) {
            switch (recurseNode) {
                case RootNode n when currentElement[0] == 0: return Next(currentElement.Skip(1).ToList(), nodes[n.EntryKey]).Prepend(0).ToList();

                case StructureNode n when currentElement.Count == 0 && (n.Children.Count == 0 || !n.ChildrenFocusable):
                case TextNode _ when currentElement.Count == 0: return currentElement;

                case RootNode _ when currentElement.Count == 0:
                case StructureNode _ when currentElement.Count == 0:
                    return new() { 0 };

                case StructureNode n: {
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

        private string Expand(string parentKey, List<int> indexChain, int index, IComponent component) {
            _ = component ?? throw new ArgumentException("Cannot render null component", nameof(component));


            var type = component.GetType();
            var key = index == -1 ? $"{parentKey}.{type.Name}" : $"{parentKey}[{index} - {type.Name}]";
            logger.Debug($"Expanding {key}");
            var expandedChain = indexChain.Append(index).ToList();

            switch (component) {
                case IBaseComponent c: {
                        var result = c.Render();
                        nodes[key] = new TextNode(parentKey, expandedChain, key, result.Text, component.Transform);
                        break;
                    }
                case ICompoundComponent c: {
                        var result = c.Render();
                        var childKeys = result.Components.Select((c, i) => Expand(key, expandedChain, i, c)).ToList();
                        nodes[key] = new StructureNode(parentKey, expandedChain, key, result.SelfFocusable, result.ComponentsFocusable, childKeys, component.Transform, result.Layout, result.ZOffset);
                        break;
                    }
                default: throw new NotImplementedException("Unhandled component type");
            }

            focusedElement ??= key;
            propsAndStates[key] = component.PropsAndState;
            mountedComponents[key] = component;
            component.OnStateChanged += () => Update(key, component.PropsAndState);

            return key;
        }

        private void Remount(string key) {
            logger.Debug($"Remounting {key}");
            var component = mountedComponents[key];

            switch ((component, nodes[key])) {
                case (IBaseComponent c, TextNode node): {
                        var result = c.Render();
                        nodes[key] = node with { Content = result.Text, Transform = component.Transform };
                        break;
                    }
                case (ICompoundComponent c, StructureNode node): {
                        var result = c.Render();
                        var childKeys = result.Components.Select((c, i) => Expand(key, node.IndexChain, i, c)).ToList();
                        nodes[key] = node with { Children = childKeys, Transform = component.Transform, Layout = result.Layout };
                        break;
                    }
            }
        }

        internal bool Update(string key, IPropsAndState propsAndState) { // TODO: Only unmount removed children, remount changed children
            if (Equals(propsAndState, propsAndStates[key])) return false;

            hasChanged = true;

            propsAndStates[key] = propsAndState;


            switch (nodes[key]) {
                case StructureNode s:
                    s.Children.ForEach(Unmount);
                    break;
            };

            Remount(key);

            if (!nodes.ContainsKey(focusedElement!)) focusedElement = key;

            return true;
        }

        internal void Unmount(string key) { // TODO: Unmount only as needed
            logger.Debug($"Unmounting {key}");
            propsAndStates.Remove(key);

            switch (nodes[key]) {
                case StructureNode s:
                    s.Children.ForEach(Unmount);
                    break;
            };
            nodes.Remove(key);

            var component = mountedComponents[key];
            mountedComponents.Remove(key);

            component.OnStateChanged -= () => Update(key, component.PropsAndState);
            component.OnUnmounted();
        }

        internal IEnumerable<IDomNode> ChildNodesOf(IDomNode node) => node switch {
            StructureNode s => s.Children.Select(k => nodes[k]),
            RootNode r => new[] { nodes[r.EntryKey] },
            _ => Enumerable.Empty<IDomNode>()
        };
    }

    public interface IPropsAndState {

    }

    internal record PropsAndState<P, S>(P Props, S State) : IPropsAndState {

    }

    internal interface IDomNode {
        string? ParentKey { get; }
        string Key { get; }
        ITransform Transform { get; }
        Layout Layout { get; }
        bool SelfFocusable { get; }
        List<int> IndexChain { get; }
    }

    internal record RootNode(string EntryKey) : IDomNode {
        public string? ParentKey => null;

        public string Key => "";

        public ITransform Transform => ITransform.Create();

        public Layout Layout => Layout.ABSOLUTE;

        public List<int> IndexChain => new();

        public bool SelfFocusable => false;
    }

    internal record TextNode(string ParentKey, List<int> IndexChain, string Key, string Content, ITransform Transform) : IDomNode {
        public Layout Layout => Layout.INHERIT;
        public bool SelfFocusable => true;
    }

    internal record StructureNode(string ParentKey, List<int> IndexChain, string Key, bool SelfFocusable, bool ChildrenFocusable, List<string> Children, ITransform Transform, Layout Layout, int ZOffset) : IDomNode {

    }
}
