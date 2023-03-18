using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleUserInterface.Core {

    /// <summary>
    /// This controls the rendering of the components to the console terminal.
    /// </summary>
    public class Renderer {

        private readonly IComponent component;
        private readonly IConsole console;
        private readonly int layerCount;
        private readonly ILogger logger;
        private Layer buffer;

        /// <summary>
        /// Create a new renderer to render the component.
        /// </summary>
        /// <param name="component">The component to render.</param>
        /// <param name="layerCount">The number of layers required for the components.</param>
        /// <param name="console">The console to render to. null to indicate default console. </param>
        public Renderer(IComponent component, int layerCount = 2, IConsole console = null, ILogger logger = null) {
            this.component = component;
            this.console = console ?? new DefaultConsole();
            this.layerCount = layerCount;
            this.logger = logger;
        }

        /// <summary>
        /// Start the rendering of the component. This method blocks this thread for the purpose of rendering.
        /// </summary>
        public void Start() {
            var frame = 0;
            while (true) {
                logger?.LogString($"Frame: {frame,7}");
                RenderFrame(true);
                Receive();
                console.CursorVisible = false;
                frame++;
            }
        }

        public void RenderFrame(bool force) {
            Render(force);
        }

        public void Receive() {
            var info = console.ReadKey(true);
            logger?.LogString($"Read key {info.Key}");
            component.ReceiveKey(info);
        }

        private void Render(bool force) {
            Render(component, layerCount, force);
        }

        private void Render(IComponent component, int layerCount, bool force) {
            var canvas = new Layer[layerCount];
            var blank = new Layer(console.WindowWidth, console.WindowHeight, console);
            blank.Write("", 0, 0, new List<FormattingRange>() { IFormatting.Blank((0, 0), (console.WindowWidth - 1, console.WindowHeight - 1)) });
            for (int i = 0; i < layerCount; i++) {
                canvas[i] = new Layer(console.WindowWidth, console.WindowHeight, console);
            }

            Render(component, canvas, console.WindowWidth, console.WindowHeight, Core.Layout.ABSOLUTE, 0, 0, 0, true);

            var buf = canvas.Aggregate(blank, (l1, l2) => l1.MergeUp(l2));
            buf.PrintToConsole(buffer, force);
            buffer = buf;
        }

        private void Render(
            IComponent component,
            Layer[] canvas,
            int parentWidth,
            int parentHeight,
            Layout layout,
            int xOffset,
            int yOffset,
            int zOffset,
            bool inFocus
        ) {
            var (w, h, x, y) = (layout, component.Transform) switch {
                (Core.Layout.HORIZONTAL, _) => (parentWidth, parentHeight, xOffset, yOffset),
                (Core.Layout.VERTICAL, _) => (parentWidth, parentHeight, xOffset, yOffset),
                
                (Core.Layout.ABSOLUTE, ITransform.PositionTransform pos) => (pos.Width, pos.Height, pos.X, pos.Y),
                (Core.Layout.ABSOLUTE, ITransform.CenteredTransform c) => (c.Width, c.Height, (console.WindowWidth - c.Width) / 2, (console.WindowHeight - c.Height) / 2),
                (Core.Layout.ABSOLUTE, _) =>
                    throw new ArgumentException("Component in an absolute position layout component needs to have a position specified"),
                
                (Core.Layout.RELATIVE, ITransform.PositionTransform pos) => (pos.Width, pos.Height, xOffset + pos.X, yOffset + pos.Y),
                (Core.Layout.RELATIVE, ITransform.CenteredTransform c) => (c.Width, c.Height, xOffset + (parentWidth - c.Width) / 2, yOffset + (parentHeight - c.Height) / 2),
                (Core.Layout.RELATIVE, _) =>
                    throw new ArgumentException("Component in an relative position layout component needs to have a position specified"),
                (var l, _) => 
                    throw new ArgumentException($"Unknown layout {l}")
            };

            switch (component) {
                case IBaseComponent comp: {
                        var result = comp.Render(w, h);
                        canvas[zOffset].Write(result.Text, x, y, result.FormattingRanges, inFocus);
                        break;
                    }
                case Container container: {
                        var containerLayout = container.Layout;
                        var result = container.Render(w, h, inFocus);
                        foreach (var comp in Layout(w, h, containerLayout, result.Components)) {
                            Render(comp.component, canvas, comp.width, comp.height, comp.layout, x + comp.xOffset, y + comp.yOffset, zOffset + comp.zOffset, comp.inFocus);
                        }
                        canvas[zOffset].Write("", x, y, result.FormattingRanges);
                        break;
                    }
                case ICompoundComponent container: {
                        var result = container.Render(w, h, inFocus);
                        foreach (var comp in Layout(w, h, layout, result.Components)) {
                            Render(comp.component, canvas, comp.width, comp.height, comp.layout, x + comp.xOffset, y + comp.yOffset, zOffset + comp.zOffset, comp.inFocus);
                        }
                        canvas[zOffset].Write("", x, y, result.FormattingRanges);
                        break;
                    }
            }
        }


        static IEnumerable<(IComponent component, int xOffset, int yOffset, int zOffset, int width, int height, Layout layout, bool inFocus)> Layout(int width, int height, Layout layout, IEnumerable<(IComponent, bool inFocus)> components) =>
            layout switch {
                Core.Layout.ABSOLUTE or Core.Layout.RELATIVE => components.Select(c => (c.Item1, 0, 0, 0, 0, 0, layout, c.inFocus)),
                Core.Layout.VERTICAL => VerticalLayout(width, height, components),
                Core.Layout.HORIZONTAL => HorizontalLayout(width, height, components),
                _ => throw new ArgumentException("Layout may only be one of the defined values")
            };

        static IEnumerable<(IComponent component, int xOffset, int yOffset, int zOffset, int width, int height, Layout layout, bool inFocus)> VerticalLayout(int width, int height, IEnumerable<(IComponent, bool inFocus)> components) {
            var weighedComponents = from component in components
                                    let transform = (component.Item1.Transform as ITransform.WeightedTransform) ?? throw new ArgumentException("Component in vertical layout group needs to have a weighed transform")
                                    select (transform.Weight, component.Item1, component.inFocus);
            var totalWeight = weighedComponents.Sum(t => t.Weight);
            var yOffset = 0;
            foreach (var (weight, comp, inFocus) in weighedComponents) {
                var componentHeight = (int)Math.Floor(weight / totalWeight * height);
                yield return (comp, 0, yOffset, 0, width, componentHeight, Core.Layout.VERTICAL, inFocus);
                yOffset += componentHeight;
            }
        }

        static IEnumerable<(IComponent component, int xOffset, int yOffset, int zOffset, int width, int height, Layout layout, bool inFocus)> HorizontalLayout(int width, int height, IEnumerable<(IComponent, bool inFocus)> components) {
            var weighedComponents = from component in components
                                    let transform = (component.Item1.Transform as ITransform.WeightedTransform) ?? throw new ArgumentException("Component in vertical layout group needs to have a weighed transform")
                                    select (transform.Weight, component.Item1, component.inFocus);
            var totalWeight = weighedComponents.Sum(t => t.Weight);
            var xOffset = 0;
            foreach (var (weight, comp, inFocus) in weighedComponents) {
                var componentWidth = (int)Math.Floor(weight / totalWeight * width);
                yield return (comp, xOffset, 0, 0, componentWidth, height, Core.Layout.HORIZONTAL, inFocus);
                xOffset += componentWidth;
            }
        }
    }
}
