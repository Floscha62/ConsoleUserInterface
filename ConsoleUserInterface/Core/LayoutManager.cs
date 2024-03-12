using ConsoleUserInterface.Core.Dom;

namespace ConsoleUserInterface.Core;

internal static class LayoutManager {

    public static IEnumerable<LayoutComponent> Layout(int windowWidth, int windowHeight, int width, int height, int xOffset, int yOffset, Layout layout, IEnumerable<IDomNode> children) =>
        layout switch {
            Core.Layout.Absolute => AbsoluteLayout(windowWidth, windowHeight, children),
            Core.Layout.Relative => RelativeLayout(width, height, xOffset, yOffset, children),
            Core.Layout.Vertical => VerticalLayout(width, height, xOffset, yOffset, children),
            Core.Layout.VerticalPreserveHeight => VerticalPreserveHeightLayout(width, height, xOffset, yOffset, children),
            Core.Layout.Horizontal => HorizontalLayout(width, height, xOffset, yOffset, children),
            Core.Layout.HorizontalPreserveWidth => HorizontalPreserveHeightLayout(width, height, xOffset, yOffset, children),
            _ => throw new ArgumentException("Layout may only be one of the defined values")
        };

    static IEnumerable<LayoutComponent> AbsoluteLayout(int windowWidth, int windowHeight, IEnumerable<IDomNode> children) =>
        children.Select<IDomNode, LayoutComponent>(c => c.Transform switch {
            ITransform.PositionTransform t => new(c, t.X, t.Y, t.Width, t.Height, c.Layout == 0 ? Core.Layout.Absolute : c.Layout),
            ITransform.CenteredTransform t => new(c, (windowWidth - t.Width) / 2, (windowHeight - t.Height) / 2, t.Width, t.Height, c.Layout == 0 ? Core.Layout.Relative : c.Layout),
            ITransform.CenteredRationalTransform t => new(
            c,
            (1 - t.Width) * windowWidth / 2,
            (1 - t.Height) * windowHeight / 2,
            t.Width * windowWidth,
                t.Height * windowHeight,
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
            yield return new(child, xoff, yOffset, w, height, child.Layout == 0 ? Core.Layout.HorizontalPreserveWidth : child.Layout);
            xoff += w;
        }
    }

    public record LayoutComponent(IDomNode DomNode, int XOffset, int YOffset, int Width, int Height, Layout Layout) {
        public LayoutComponent(IDomNode domNode, double xOffset, double yOffset, double width, double height, Layout layout) :
            this(domNode, (int)Math.Round(xOffset), (int)Math.Round(yOffset), (int)Math.Round(width), (int)Math.Round(height), layout) { }
    }
}