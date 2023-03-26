using LoggingConsole;

namespace ConsoleUserInterfaceWithDOM.Core {

    /// <summary>
    /// This controls the rendering of the components to the console terminal.
    /// </summary>
    public class Renderer {

        private static readonly ILogger logger = LoggingFactory.Create(typeof(Renderer));
        private readonly Dom.Dom dom;
        private readonly IConsole console;
        private readonly int layerCount;
        private Layer buffer;

        /// <summary>
        /// Create a new renderer to render the component.
        /// </summary>
        /// <param name="component">The component to render.</param>
        /// <param name="layerCount">The number of layers required for the components.</param>
        /// <param name="console">The console to render to. null to indicate default console. </param>
        public Renderer(IComponent component, int layerCount = 2, IConsole? console = null) {
            this.console = console ?? new DefaultConsole();
            this.console.Write("Test");
            this.layerCount = layerCount;
            this.dom = new(component);
        }

        /// <summary>
        /// Start the rendering of the component. This method blocks this thread for the purpose of rendering.
        /// </summary>
        public void Start() {
            var frame = 0;
            var escaped = false;
            while (!escaped) {
                if (dom.HasChanged) {
                    logger?.Debug($"Frame: {frame} (focused node: {dom.FocusedNode.Key})");
                    RenderFrame(true);
                    logger?.Debug($"Frame finished rendering: {frame}");
                    frame++;
                }
                escaped = Receive();
                console.CursorVisible = false;
            }
        }

        public void RenderFrame(bool force) {
            Render(force);
        }

        public bool Receive() {
            var info = console.ReadKey(true);
            logger?.Debug($"Read key {info.Key}");
            if (info.Key == ConsoleKey.Tab && info.Modifiers == 0) {
                dom.FocusNext();
                return false;
            }
            if (info.Key == ConsoleKey.Tab && info.Modifiers == ConsoleModifiers.Control) {
                dom.FocusPrevious();
                return false;
            }
            var focusedComponent = dom.FocusedComponent;
            var received = focusedComponent.ReceiveKey(info);

            return !received && info.Key == ConsoleKey.Escape;
        }

        private void Render(bool force) {
            var canvas = new Layer[layerCount];
            var blank = new Layer(console.WindowWidth, console.WindowHeight, console);
            blank.ApplyFormatting(0, 0, new List<FormattingRange>() { IFormatting.Blank((0, 0), (console.WindowWidth - 1, console.WindowHeight - 1)) });
            for (int i = 0; i < layerCount; i++) {
                canvas[i] = new Layer(console.WindowWidth, console.WindowHeight, console);
            }

            Render(dom.RootNode, canvas, console.WindowWidth, console.WindowHeight, dom.RootNode.Layout, 0, 0, 0);

            var buf = canvas.Aggregate(blank, (l1, l2) => l1.MergeUp(l2));
            buf.PrintToConsole(buffer, force);
            buffer = buf;
        }

        private void Render(
            Dom.IDomNode domNode,
            Layer[] canvas,
            int parentWidth,
            int parentHeight,
            Layout layout,
            int xOffset,
            int yOffset,
            int zOffset
        ) {
            var (w, h, x, y) = (layout, domNode.Transform) switch {
                (Core.Layout.HORIZONTAL, _) => (parentWidth, parentHeight, xOffset, yOffset),
                (Core.Layout.VERTICAL, _) => (parentWidth, parentHeight, xOffset, yOffset),

                (Core.Layout.ABSOLUTE, ITransform.PositionTransform pos) => (pos.Width, pos.Height, pos.X, pos.Y),
                (Core.Layout.ABSOLUTE, ITransform.CenteredTransform c) => (c.Width, c.Height, (console.WindowWidth - c.Width) / 2, (console.WindowHeight - c.Height) / 2),
                (Core.Layout.ABSOLUTE, ITransform.CenteredFullsizeTransform) => (parentWidth, parentHeight, (console.WindowWidth - parentWidth) / 2, (console.WindowHeight - parentHeight) / 2),
                (Core.Layout.ABSOLUTE, _) =>
                    throw new ArgumentException("Component in an absolute position layout component needs to have a position specified"),

                (Core.Layout.RELATIVE, ITransform.PositionTransform pos) => (pos.Width, pos.Height, xOffset + pos.X, yOffset + pos.Y),
                (Core.Layout.RELATIVE, ITransform.CenteredTransform c) => (c.Width, c.Height, xOffset + (parentWidth - c.Width) / 2, yOffset + (parentHeight - c.Height) / 2),
                (Core.Layout.RELATIVE, ITransform.CenteredFullsizeTransform) => (parentWidth, parentHeight, xOffset, yOffset),
                (Core.Layout.RELATIVE, _) =>
                    throw new ArgumentException("Component in an relative position layout component needs to have a position specified"),
                (var l, _) =>
                    throw new ArgumentException($"Unknown layout {l}")
            };


            var focusFormat = ConstructFocusFormat(domNode == dom.FocusedNode, zOffset, w, h);

            switch (domNode) {
                case Dom.TextNode node:
                    logger.Debug($"Rendering '{domNode.Key}' {(w, h, x, y)} (Text Content '{node.Content}')");
                    canvas[zOffset].Write(node.Content, x, y, w, h);
                    canvas[zOffset].ApplyFormatting(x, y, focusFormat);
                    break;
                case Dom.StructureNode node:
                    logger.Debug($"Rendering '{domNode.Key}' {(w, h, x, y)} (Children '{node.Children.Count}')");
                    foreach (var comp in Layout(w, h, layout, dom.ChildNodesOf(node))) {
                        Render(comp.domNode, canvas, comp.width, comp.height, comp.layout, x + comp.xOffset, y + comp.yOffset, node.ZOffset + zOffset);
                    }
                    canvas[zOffset].ApplyFormatting(x, y, focusFormat);

                    break;
            }
        }

        static readonly Dictionary<int, IFormatting> FocusByZ = new();
        static IFormatting ConstructFocusForZ(int z) => IFormatting.Background(20, 20, 40 + 20 * z);

        static IEnumerable<FormattingRange> ConstructFocusFormat(bool inFocus, int z, int w, int h) {
            if (!inFocus) return Enumerable.Empty<FormattingRange>();
            if (!FocusByZ.TryGetValue(z, out var format))
                FocusByZ[z] = format = ConstructFocusForZ(z);

            return Enumerable.Range(0, h).Select(row => new FormattingRange((0, row), (w - 1, row), format));
        }

        static IEnumerable<(Dom.IDomNode domNode, int xOffset, int yOffset, int width, int height, Layout layout)> Layout(int width, int height, Layout layout, IEnumerable<Dom.IDomNode> children) =>
            layout switch {
                Core.Layout.ABSOLUTE or Core.Layout.RELATIVE => children.Select(c => (c, 0, 0, width, height, c.Layout == 0 ? layout : c.Layout)),
                Core.Layout.VERTICAL => VerticalLayout(width, height, children),
                Core.Layout.HORIZONTAL => HorizontalLayout(width, height, children),
                _ => throw new ArgumentException("Layout may only be one of the defined values")
            };

        static IEnumerable<(Dom.IDomNode domNode, int xOffset, int yOffset, int width, int height, Layout layout)> VerticalLayout(int width, int height, IEnumerable<Dom.IDomNode> children) {
            var weighedChildren = from child in children
                                    let transform = (child.Transform as ITransform.WeightedTransform) ?? throw new ArgumentException("Component in vertical layout group needs to have a weighed transform")
                                    select (transform.Weight, child);
            var totalWeight = weighedChildren.Sum(t => t.Weight);
            var yOffset = 0;
            foreach (var (weight, childNode) in weighedChildren) {
                var componentHeight = (int)Math.Floor(weight / totalWeight * height);
                yield return (childNode, 0, yOffset, width, componentHeight, childNode.Layout == 0 ? Core.Layout.VERTICAL : childNode.Layout);
                yOffset += componentHeight;
            }
        }

        static IEnumerable<(Dom.IDomNode domNode, int xOffset, int yOffset, int width, int height, Layout layout)> HorizontalLayout(int width, int height, IEnumerable<Dom.IDomNode> children) {
            var weighedChildren = from child in children
                                    let transform = (child.Transform as ITransform.WeightedTransform) ?? throw new ArgumentException("Component in vertical layout group needs to have a weighed transform")
                                    select (transform.Weight, child);
            var totalWeight = weighedChildren.Sum(t => t.Weight);
            var xOffset = 0;
            foreach (var (weight, childNode) in weighedChildren) {
                var componentWidth = (int)Math.Floor(weight / totalWeight * width);
                yield return (childNode, xOffset, 0, componentWidth, height, childNode.Layout == 0 ? Core.Layout.HORIZONTAL : childNode.Layout);
                xOffset += componentWidth;
            }
        }
    }
}
