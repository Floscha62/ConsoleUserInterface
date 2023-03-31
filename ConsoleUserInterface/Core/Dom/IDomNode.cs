namespace ConsoleUserInterface.Core.Dom {
    internal interface IDomNode {
        string? ParentKey { get; }
        string Key { get; }
        ITransform Transform { get; }
        Layout Layout { get; }
        bool SelfFocusable { get; }
        List<int> IndexChain { get; }


        internal record RootNode(string EntryKey) : IDomNode {
            public string? ParentKey => null;

            public string Key => "";

            public ITransform Transform => ITransform.Create();

            public Layout Layout => Layout.Absolute;

            public List<int> IndexChain => new();

            public bool SelfFocusable => false;
        }

        internal record TextNode(string ParentKey, List<int> IndexChain, string Key, string Content, bool Underlined, ITransform Transform) : IDomNode {
            public Layout Layout => Layout.Inherit;
            public bool SelfFocusable => true;
        }

        internal record StructureNode(string ParentKey, List<int> IndexChain, string Key, bool SelfFocusable, bool ChildrenFocusable, List<string> Children, ITransform Transform, Layout Layout, int ZOffset) : IDomNode {

        }
    }
}
