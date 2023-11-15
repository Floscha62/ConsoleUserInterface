using ConsoleUserInterface.Core.Dom;
using LoggingConsole;
using System.Data;
#if DEBUG
using System.Text.RegularExpressions;
#endif

namespace ConsoleUserInterface.Core {

    /// <summary>
    /// This controls the rendering of the components to the console terminal.
    /// </summary>
    public class Renderer {

        static readonly ILogger logger = LoggingFactory.Create(typeof(Renderer));
        readonly Dom.Dom dom;
        readonly IConsole console;
        readonly int layerCount;
#if DEBUG
        bool renderDom;
        Layer domBuffer;
#endif
        bool exited;
        record RenderRequest(bool Forced);

        readonly object requestLock = new();
        RenderRequest? request;

        Layer buffer;
        int lastWidth;
        int lastHeight;

        /// <summary>
        /// Create a new renderer to render the component.
        /// </summary>
        /// <param name="applicationName">The name of the application.</param>
        /// <param name="component">The component to render.</param>
        /// <param name="layerCount">The number of layers required for the components.</param>
        /// <param name="console">The console to render to. null to indicate default console. </param>
        public Renderer(string applicationName, IComponent component, int layerCount = 2, IConsole? console = null) {
            this.console = console ?? new DefaultConsole();
            this.console.Title = applicationName;
            this.layerCount = layerCount;
            this.dom = new(component);
        }

        /// <summary>
        /// Start the rendering of the component. This method blocks this thread for the purpose of rendering.
        /// </summary>
        public void Start() {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            RenderLoop(token);
            ReceiveLoop(token);
            SizeChangeDetection(token);

            while (!exited) { }
            tokenSource.Cancel();
        }

        Task RenderLoop(CancellationToken token) => Task.Factory.StartNew(() => {
            var frame = 0;
            while (true) {
                try {
                    token.ThrowIfCancellationRequested();

                    RenderRequest? req;
                    lock (requestLock) {
                        req = request;
                        request = null;
                    }
                    if (req != null) {
#if DEBUG
                        if (renderDom) {
                            logger?.Debug($"Render Dom for Frame: {frame}");
                            RenderDom();
                            logger?.Debug($"Dom rendered: {frame}");
                        } else {
                            logger?.Debug($"Frame: {frame} (focused node: {dom.FocusedNode.Key})");
                            RenderFrame(req.Forced);
                            logger?.Debug($"Frame finished rendering: {frame}");
                        }
#else
                        if (req.Forced || dom.HasChanged) {
                            RenderFrame(req.Forced);
                        }
#endif
                        frame++;
                        console.CursorVisible = false;
                    }
                } catch (Exception ex) {
                    logger?.Error($"{ex.Message}: {ex.StackTrace}");
                }
            }
        }, token);

        Task ReceiveLoop(CancellationToken token) => Task.Factory.StartNew(() => {
            var escaped = false;
            while (!escaped) {
                token.ThrowIfCancellationRequested();
#if DEBUG
                (escaped, var debugToggled) = Receive();
                lock(requestLock) {
                    request = new(true);
                }
#else
                escaped = Receive();
                lock(requestLock) {
                    if (request == null) {
                        request = new(false);
                    }
                }
#endif
            }
            exited = true;
        }, token);

        Task SizeChangeDetection(CancellationToken token) => Task.Factory.StartNew(() => {
            while (true) {
                token.ThrowIfCancellationRequested();

                var newWidth = console.WindowWidth;
                var newHeight = console.WindowHeight;

                if ((newWidth, newHeight) != (lastWidth, lastHeight)) {
                    lock (requestLock) {
                        request = new(true);
                    }
                    (lastWidth, lastHeight) = (newWidth, newHeight);
                }
            }
        }, token);
#if DEBUG
        void RenderDom() {
            var canvas = new Layer(console.WindowWidth, console.WindowHeight, console);
            canvas.ApplyFormatting(0, 0, new[] { IFormatting.Blank((0, 0), (console.WindowWidth - 1, console.WindowHeight - 1)) });
            RenderDomNode(dom.rootNode, -1, canvas, 0, console.WindowWidth, console.WindowHeight, Core.Layout.Absolute, 0, 0, 0);

            canvas.PrintToConsole(domBuffer, true);
            domBuffer = canvas;
        }

        int RenderDomNode(
            IDomNode domNode,
            int depth,
            Layer canvas,
            int row,
            int width,
            int height,
            Layout layout,
            int xOffset,
            int yOffset,
            int zOffset
        ) {
            var selfKey = domNode.Key[(domNode.ParentKey?.Length ?? 0)..];
            var typeMatch = Regex.Match(selfKey, "\\[\\d* - (.*)\\]");
            var type = typeMatch.Success ? typeMatch.Groups[1].Value : selfKey;

            switch (domNode) {
                case IDomNode.RootNode root: {
                        var rows = 0;
                        foreach (var comp in Layout(width, height, xOffset, yOffset, layout, dom.ChildNodesOf(root))) {
                            rows += RenderDomNode(comp.DomNode, depth + 1, canvas, row + rows, comp.Width, comp.Height, comp.Layout, comp.XOffset, comp.YOffset, zOffset);
                        }
                        return rows;
                    }
                case IDomNode.TextNode _: {
                        var (_, props, state, focused) = dom[domNode.Key];
                        var nodeString = $"{"".PadRight(depth * 2)}<{type} {props ?? ""} {state ?? ""} " +
                            $"{{ size = {(width, height)}, offset = {(xOffset, yOffset, zOffset)}, layout = {layout} }}/>";
                        canvas.Write(nodeString, 0, row, console.WindowWidth, 1, focused);
                        return 1;
                    }
                case IDomNode.StructureNode structure: {
                        var (_, props, state, focused) = dom[domNode.Key];
                        var nodeString = $"{"".PadRight(depth * 2)}<{type} {props ?? ""} {state ?? ""} " +
                            $"{{ size = {(width, height)}, offset = {(xOffset, yOffset, zOffset)}, layout = {layout} }}/>";
                        canvas.Write(nodeString, 0, row, console.WindowWidth, 1, focused);
                        var rows = 1;
                        foreach (var comp in Layout(width, height, xOffset, yOffset, layout, dom.ChildNodesOf(structure))) {
                            rows += RenderDomNode(comp.DomNode, depth + 1, canvas, row + rows, comp.Width, comp.Height, comp.Layout, comp.XOffset, comp.YOffset, zOffset);
                        }
                        return rows;
                    }
            };
            return 0;
        }
#endif

        void RenderFrame(bool force) {
            Render(force);
        }

#if DEBUG
        (bool escaped, bool debugToggle) Receive() {
            var info = console.ReadKey(true);
            logger?.Debug($"Read key {info.Key}");
            if (info.Key == ConsoleKey.F12) {
                renderDom = !renderDom;
                return (false, true);
            }

            return (!dom.ReceiveKey(info) && info.Key == ConsoleKey.Escape, false);
        }
#else
        bool Receive() {
            var info = console.ReadKey(true);
            return !dom.ReceiveKey(info) && info.Key == ConsoleKey.Escape;
        }
#endif

        void Render(bool force) {
            var canvas = new Layer[layerCount];
            var blank = new Layer(console.WindowWidth, console.WindowHeight, console);
            blank.ApplyFormatting(0, 0, new List<FormattingRange>() { IFormatting.Blank((0, 0), (console.WindowWidth - 1, console.WindowHeight - 1)) });
            for (int i = 0; i < layerCount; i++) {
                canvas[i] = new Layer(console.WindowWidth, console.WindowHeight, console);
            }

            Render(dom.rootNode, canvas, console.WindowWidth, console.WindowHeight, Core.Layout.Absolute, 0, 0, 0);

            var buf = canvas.Aggregate(blank, (l1, l2) => l1.MergeUp(l2));
            buf.PrintToConsole(buffer, force);
            buffer = buf;
        }

        void Render(
            IDomNode domNode,
            Layer[] canvas,
            int width,
            int height,
            Layout layout,
            int xOffset,
            int yOffset,
            int zOffset
        ) {
            var focusFormat = ConstructFocusFormat(domNode == dom.FocusedNode, zOffset, width, height);

            switch (domNode) {
                case IDomNode.TextNode node:
                    logger.Debug($"Rendering '{domNode.Key}' {(width, height, xOffset, yOffset)} (Text Content '{node.Content}')");
                    canvas[zOffset].Write(node.Content, xOffset, yOffset, width, height, node.Underlined);
                    canvas[zOffset].ApplyFormatting(xOffset, yOffset, focusFormat);
                    break;
                case IDomNode.StructureNode node:
                    logger.Debug($"Rendering '{domNode.Key}' {(width, height, xOffset, yOffset)} (Children '{node.Children.Count}')");
                    foreach (var comp in Layout(width, height, xOffset, yOffset, layout, dom.ChildNodesOf(node))) {
                        Render(comp.DomNode, canvas, comp.Width, comp.Height, comp.Layout, comp.XOffset, comp.YOffset, node.ZOffset + zOffset);
                    }
                    canvas[zOffset].ApplyFormatting(xOffset, yOffset, focusFormat);

                    break;
                case IDomNode.RootNode node:
                    foreach (var comp in Layout(width, height, xOffset, yOffset, layout, dom.ChildNodesOf(node))) {
                        Render(comp.DomNode, canvas, comp.Width, comp.Height, comp.Layout, comp.XOffset, comp.YOffset, zOffset);
                    }
                    break;
            }
        }

        static readonly Dictionary<int, IFormatting> FocusByZ = new();
        static IFormatting ConstructFocusForZ(int z) => IFormatting.Background(20, 20, 40 + 20 * z, false);

        static IEnumerable<FormattingRange> ConstructFocusFormat(bool inFocus, int z, int w, int h) {
            if (!inFocus) return Enumerable.Empty<FormattingRange>();
            if (!FocusByZ.TryGetValue(z, out var format))
                FocusByZ[z] = format = ConstructFocusForZ(z);

            return Enumerable.Range(0, h).Select(row => new FormattingRange((0, row), (w - 1, row), format));
        }

        record LayoutComponent(IDomNode DomNode, int XOffset, int YOffset, int Width, int Height, Layout Layout) {
            public LayoutComponent(IDomNode domNode, double xOffset, double yOffset, double width, double height, Layout layout) :
                this(domNode, (int)Math.Round(xOffset), (int)Math.Round(yOffset), (int)Math.Round(width), (int)Math.Round(height), layout) { }
        }

        IEnumerable<LayoutComponent> Layout(int width, int height, int xOffset, int yOffset, Layout layout, IEnumerable<IDomNode> children) =>
            layout switch {
                Core.Layout.Absolute => AbsoluteLayout(children),
                Core.Layout.Relative => RelativeLayout(width, height, xOffset, yOffset, children),
                Core.Layout.Vertical => VerticalLayout(width, height, xOffset, yOffset, children),
                Core.Layout.VerticalPreserveHeight => VerticalPreserveHeightLayout(width, height, xOffset, yOffset, children),
                Core.Layout.Horizontal => HorizontalLayout(width, height, xOffset, yOffset, children),
                Core.Layout.HorizontalPreserveHeight => HorizontalPreserveHeightLayout(width, height, xOffset, yOffset, children),
                _ => throw new ArgumentException("Layout may only be one of the defined values")
            };

        IEnumerable<LayoutComponent> AbsoluteLayout(IEnumerable<IDomNode> children) =>
            children.Select<IDomNode, LayoutComponent>(c => c.Transform switch {
                ITransform.PositionTransform t => new(c, t.X, t.Y, t.Width, t.Height, c.Layout == 0 ? Core.Layout.Absolute : c.Layout),
                ITransform.CenteredTransform t => new(c, (console.WindowWidth - t.Width) / 2, (console.WindowHeight - t.Height) / 2, t.Width, t.Height, c.Layout == 0 ? Core.Layout.Relative : c.Layout),
                ITransform.CenteredRationalTransform t => new(
                    c,
                    (1 - t.Width) * console.WindowWidth / 2,
                    (1 - t.Height) * console.WindowHeight / 2,
                    t.Width * console.WindowWidth,
                    t.Height * console.WindowHeight,
                    c.Layout == 0 ? Core.Layout.Absolute : c.Layout
                ),
                _ => throw new ArgumentException("Component in absolute layout group needs to have a positioned transform")
            });

        static IEnumerable<LayoutComponent> RelativeLayout(int width, int height, int xOffset, int yOffset, IEnumerable<IDomNode> children) =>
            children.Select<IDomNode, LayoutComponent>(c => c.Transform switch {
                ITransform.PositionTransform t => new(c, xOffset + t.X, yOffset + t.Y, t.Width, t.Height, c.Layout == 0 ? Core.Layout.Relative : c.Layout),
                ITransform.CenteredTransform t => new(c, xOffset + (width - t.Width) / 2, yOffset + (height - t.Height) / 2, t.Width, t.Height, c.Layout == 0 ? Core.Layout.Relative : c.Layout),
                ITransform.CenteredRationalTransform t => new(c, xOffset + (width - t.Width) / 2, yOffset + (height - t.Height) / 2, t.Width, t.Height, c.Layout == 0 ? Core.Layout.Relative : c.Layout),
                _ => throw new ArgumentException("Component in relative layout group needs to have a positioned transform")
            });

        static IEnumerable<LayoutComponent> VerticalLayout(int width, int height, int xOffset, int yOffset, IEnumerable<IDomNode> children) {
            var weighedChildren = from child in children
                                  let transform = (child.Transform as ITransform.WeightedTransform) ?? throw new ArgumentException("Component in vertical layout group needs to have a weighed transform")
                                  select (transform.Weight, child);
            var totalWeight = weighedChildren.Sum(t => t.Weight);
            var yOff = yOffset;
            foreach (var (weight, childNode) in weighedChildren) {
                var componentHeight = (int)Math.Floor(weight / totalWeight * height);
                yield return new(childNode, xOffset, yOff, width, componentHeight, childNode.Layout == 0 ? Core.Layout.Vertical : childNode.Layout);
                yOff += componentHeight;
            }
        }

        static IEnumerable<LayoutComponent> VerticalPreserveHeightLayout(int width, int height, int xOffset, int yOffset, IEnumerable<IDomNode> children) {
            var yoff = yOffset;
            foreach (var child in children) {
                var h = child.Transform switch {
                    ITransform.CenteredTransform(_, var transformHeight) => transformHeight,
                    ITransform.CenteredRationalTransform(_, var transformHeight) => (int)Math.Round(transformHeight * height),
                    _ => throw new ArgumentException("Component in vertical layout group preserving height needs to have a defined height without a position")
                };
                yield return new(child, xOffset, yoff, width, h, child.Layout == 0 ? Core.Layout.VerticalPreserveHeight : child.Layout);
                yoff += h;
            }
        }


        static IEnumerable<LayoutComponent> HorizontalLayout(int width, int height, int xOffset, int yOffset, IEnumerable<IDomNode> children) {
            var weighedChildren = from child in children
                                  let transform = (child.Transform as ITransform.WeightedTransform) ?? throw new ArgumentException("Component in vertical layout group needs to have a weighed transform")
                                  select (transform.Weight, child);
            var totalWeight = weighedChildren.Sum(t => t.Weight);
            var xOff = xOffset;
            foreach (var (weight, childNode) in weighedChildren) {
                var componentWidth = (int)Math.Floor(weight / totalWeight * width);
                yield return new(childNode, xOff, yOffset, componentWidth, height, childNode.Layout == 0 ? Core.Layout.Horizontal : childNode.Layout);
                xOff += componentWidth;
            }
        }

        static IEnumerable<LayoutComponent> HorizontalPreserveHeightLayout(int width, int height, int xOffset, int yOffset, IEnumerable<IDomNode> children) {
            var xoff = xOffset;
            foreach (var child in children) {
                var w = child.Transform switch {
                    ITransform.CenteredTransform(var transformWidth, _) => transformWidth,
                    ITransform.CenteredRationalTransform(var transformWidth, _) => (int)Math.Round(transformWidth * width),
                    _ => throw new ArgumentException("Component in vertical layout group preserving height needs to have a defined height without a position")
                };
                yield return new(child, xoff, yOffset, w, height, child.Layout == 0 ? Core.Layout.VerticalPreserveHeight : child.Layout);
                xoff += w;
            }
        }
    }
}
