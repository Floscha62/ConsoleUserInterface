using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleUserInterface.Core {

    /// <summary>
    /// This controls the rendering of the components to the console terminal.
    /// </summary>
    public class Renderer {

        private readonly IComponent component;
        private readonly IConsole console;
        private readonly int layerCount;
        private Layer buffer;

        /// <summary>
        /// Create a new renderer to render the component.
        /// </summary>
        /// <param name="component">The component to render.</param>
        /// <param name="layerCount">The number of layers required for the components.</param>
        /// <param name="console">The console to render to. null to indicate default console. </param>
        public Renderer(IComponent component, int layerCount = 2, IConsole console = null) {
            this.component = component;
            this.console = console ?? new DefaultConsole();
            this.layerCount = layerCount;
        }

        /// <summary>
        /// Start the rendering of the component. This method blocks this thread for the purpose of rendering.
        /// </summary>
        public void Start() {
            while (true) {
                RenderFrame(false);
                Receive();
                console.CursorVisible = false;
            }
        }

        public void RenderFrame(bool force) {
            Render(force);
        }

        public void Receive() {
            component.ReceiveKey(console.ReadKey(true));
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

            Render(component, canvas, console.WindowWidth, console.WindowHeight);

            var buf = canvas.Aggregate(blank, (l1, l2) => l1.MergeUp(l2));
            buf.PrintToConsole(buffer, force);
            buffer = buf;
        }

        private void Render(
            IComponent component,
            Layer[] canvas,
            int parentWidth,
            int parentHeight,
            Layout layout = Core.Layout.ABSOLUTE,
            int xOffset = 0,
            int yOffset = 0,
            int zOffset = 0
        ) {
            switch (component) {
                case IBaseComponent comp when layout == Core.Layout.HORIZONTAL || layout == Core.Layout.VERTICAL: {
                        var result = comp.Render(parentWidth, parentHeight);
                        canvas[zOffset].Write(result.Text, xOffset, yOffset, result.FormattingRanges);
                        break;
                    }
                case IBaseComponent comp when layout == Core.Layout.ABSOLUTE: {
                        var transform = comp.Transform;
                        if (transform is PositionTransform position) {
                            var result = comp.Render(position.Width, position.Height);
                            canvas[zOffset].Write(result.Text, position.X, position.Y, result.FormattingRanges);
                            break;
                        }
                        throw new ArgumentException("Component in an absolute position layout component needs to have a position specified");
                    }
                case IBaseComponent comp when layout == Core.Layout.RELATIVE: {
                        var transform = comp.Transform;
                        if (transform is PositionTransform position) {
                            var result = comp.Render(position.Width, position.Height);
                            canvas[zOffset].Write(result.Text, xOffset + position.X, yOffset + position.Y, result.FormattingRanges);
                            break;
                        }
                        throw new ArgumentException("Component in an absolute position layout component needs to have a position specified");
                    }
                case Container container when layout == Core.Layout.HORIZONTAL || layout == Core.Layout.VERTICAL: {
                        var containerLayout = container.Layout;
                        var result = container.Render(parentWidth, parentHeight);
                        foreach (var comp in Layout(parentWidth, parentHeight, containerLayout, result.Components)) {
                            Render(comp.component, canvas, comp.width, comp.height, comp.layout, xOffset + comp.xOffset, yOffset + comp.yOffset, zOffset + comp.zOffset);
                        }
                        canvas[zOffset].Write("", xOffset, yOffset, result.FormattingRanges);
                        break;
                    }
                case Container container when layout == Core.Layout.ABSOLUTE: {
                        var transform = container.Transform;

                        if (transform is PositionTransform position) {
                            var containerLayout = container.Layout;
                            var result = container.Render(position.Width, position.Height);
                            foreach (var comp in Layout(position.Width, position.Height, containerLayout, result.Components)) {
                                Render(comp.component, canvas, comp.width, comp.height, comp.layout, position.X + comp.xOffset, position.Y + comp.yOffset, zOffset + comp.zOffset);
                            }
                            canvas[zOffset].Write("", position.X, position.Y, result.FormattingRanges);
                            break;
                        }
                        throw new ArgumentException("Component in an absolute position layout component needs to have a position specified");
                    }
                case Container container when layout == Core.Layout.RELATIVE: {
                        var transform = container.Transform;

                        if (transform is PositionTransform position) {
                            var containerLayout = container.Layout;
                            var result = container.Render(position.Width, position.Height);
                            foreach (var comp in Layout(position.Width, position.Height, containerLayout, result.Components)) {
                                Render(comp.component, canvas, comp.width, comp.height, comp.layout, xOffset + position.X + comp.xOffset, yOffset + position.Y + comp.yOffset, zOffset + comp.zOffset);
                            }
                            canvas[zOffset].Write("", xOffset + position.X, yOffset + position.Y, result.FormattingRanges);
                            break;
                        }
                        throw new ArgumentException("Component in an absolute position layout component needs to have a position specified");
                    }
            }
        }


        static IEnumerable<(IComponent component, int xOffset, int yOffset, int zOffset, int width, int height, Layout layout)> Layout(int width, int height, Layout layout, IEnumerable<IComponent> components) =>
            layout switch {
                Core.Layout.ABSOLUTE or Core.Layout.RELATIVE => components.Select(c => (c, 0, 0, 0, 0, 0, layout)),
                Core.Layout.VERTICAL => VerticalLayout(width, height, components),
                Core.Layout.HORIZONTAL => HorizontalLayout(width, height, components),
                _ => throw new ArgumentException("Layout may only be one of the defined values")
            };

        static IEnumerable<(IComponent component, int xOffset, int yOffset, int zOffset, int width, int height, Layout layout)> VerticalLayout(int width, int height, IEnumerable<IComponent> components) {
            var weighedComponents = from component in components
                                    let transform = (component.Transform as WeightedTransform) ?? throw new ArgumentException("Component in vertical layout group needs to have a weighed transform")
                                    select (transform.Weight, component);
            var totalWeight = weighedComponents.Sum(t => t.Weight);
            var yOffset = 0;
            foreach (var (weight, comp) in weighedComponents) {
                var componentHeight = (int)Math.Floor(weight / totalWeight * height);
                yield return (comp, 0, yOffset, 0, width, componentHeight, Core.Layout.VERTICAL);
                yOffset += componentHeight;
            }
        }

        static IEnumerable<(IComponent component, int xOffset, int yOffset, int zOffset, int width, int height, Layout layout)> HorizontalLayout(int width, int height, IEnumerable<IComponent> components) {
            var weighedComponents = from component in components
                                    let transform = (component.Transform as WeightedTransform) ?? throw new ArgumentException("Component in vertical layout group needs to have a weighed transform")
                                    select (transform.Weight, component);
            var totalWeight = weighedComponents.Sum(t => t.Weight);
            var xOffset = 0;
            foreach (var (weight, comp) in weighedComponents) {
                var componentWidth = (int)Math.Floor(weight / totalWeight * width);
                yield return (comp, xOffset, 0, 0, componentWidth, height, Core.Layout.HORIZONTAL);
                xOffset += componentWidth;
            }
        }
    }
}
