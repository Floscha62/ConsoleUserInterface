using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConsoleUserInterface.Core.Extensions;

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

            Render(component, 0, 0, 0, canvas);

            var buf = canvas.Aggregate(blank, (l1, l2) => l1.MergeUp(l2));
            buf.PrintToConsole(buffer, force);
            buffer = buf;
        }

        private void Render(IComponent component, int xOffset, int yOffset, int zOffset, Layer[] canvas) {
            component.SetConsole(console);
            switch (component) {
                case IBaseComponent comp:
                    var (text, formattings) = comp.RenderString();
                    canvas[zOffset + comp.LayerIndex].Write(text, xOffset, yOffset, formattings);
                    break;
                case IVerticalComponent vert:
                    var (vertChilds, vertF) = vert.Render();
                    canvas[zOffset + vert.LayerIndex].Write("", xOffset, yOffset, vertF);
                    var yOff = yOffset;
                    foreach (var child in vertChilds) {
                        Render(child, xOffset, yOff, zOffset + vert.LayerIndex, canvas);
                        yOff += child.Height;
                    }
                    break;
                case IHorizontalComponent hori:
                    var (horiChilds, horiF) = hori.Render();
                    canvas[zOffset + hori.LayerIndex].Write("", xOffset, yOffset, horiF);
                    var xOff = xOffset;
                    foreach (var child in horiChilds) {
                        Render(child, xOff, yOffset, zOffset + hori.LayerIndex, canvas);
                        xOff += child.Width;
                    }
                    break;
                case INoLayoutComponent noLayout:
                    var (noLayoutChilds, noLayoutF) = noLayout.Render();
                    canvas[zOffset + noLayout.LayerIndex].Write("", xOffset, yOffset, noLayoutF);
                    foreach (var child in noLayoutChilds) {
                        Render(child, 0, 0, zOffset + noLayout.LayerIndex, canvas);
                    }
                    break;
                case IConstOffsetComponent offset:
                    var c = offset.RenderComponent();
                    var (x, y) = offset.Offset(console);
                    Render(c, x, y, zOffset + offset.LayerIndex, canvas);
                    break;
            }
        }
    }
}
