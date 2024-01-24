using ConsoleUserInterface.Core;
using ConsoleUserInterface.Core.Dom;
using NUnit.Framework;

namespace ConsoleUserInterface.Tests;

[TestFixture]
internal class LayoutManager {

    private static IDomNode DomNode(ITransform transform, Layout layout) =>
        new TestNode(transform, layout);

    private record TestNode(ITransform Transform, Layout Layout) : IDomNode {
        public string? ParentKey { get { Assert.Fail("Unexpected property access"); return null; } }

        public string Key { get { Assert.Fail("Unexpected property access"); return null; } }

        public bool SelfFocusable { get { Assert.Fail("Unexpected property access"); return false; } }

        public List<int> IndexChain {
            get { Assert.Fail("Unexpected property access"); return null; }
        }
    }


    [Test]
    public void Vertical_Layout_Respects_Weight_Provided_By_Transform() {
        var children = new IDomNode[] {
            DomNode(ITransform.Create(12), Layout.Relative),
            DomNode(ITransform.Create(6), Layout.Horizontal),
            DomNode(ITransform.Create(12), Layout.VerticalPreserveHeight),
            DomNode(ITransform.Create(3), Layout.HorizontalPreserveWidth),
            DomNode(ITransform.Create(2), Layout.Absolute),
            DomNode(ITransform.Create(1), Layout.Vertical),
            DomNode(ITransform.Create(4), Layout.Inherit),
            DomNode(ITransform.Create(8), Layout.Horizontal),
            DomNode(ITransform.Create(12), Layout.HorizontalPreserveWidth),
        };
        var expectedHeights = new int[] {
            24,
            12,
            24,
            6,
            4,
            2,
            8,
            16,
            24
        };

        var result = Core.LayoutManager.Layout(100, 120, 100, 120, 0, 0, Layout.Vertical, children).ToList();
        var expectedY = 0;
        Assert.AreEqual(children.Length, result.Count);
        for (int i = 0; i < result.Count; i++) {
            Assert.AreEqual(children[i], result[i].DomNode);
            Assert.AreEqual(0, result[i].XOffset);
            Assert.AreEqual(expectedY, result[i].YOffset);
            Assert.AreEqual(100, result[i].Width);
            Assert.AreEqual(expectedHeights[i], result[i].Height);
            if (children[i].Layout == Layout.Inherit) {
                Assert.AreEqual(Layout.Vertical, result[i].Layout);
            } else {
                Assert.AreEqual(children[i].Layout, result[i].Layout);
            }
            expectedY += expectedHeights[i];
        }
    }

    [Test]
    public void Vertical_Preserve_Height_Layout_Respects_Height_Provided_By_Transform() {
        var children = new IDomNode[] {
            DomNode(ITransform.Create(100, 24), Layout.Relative),
            DomNode(ITransform.Create(100, 15), Layout.Horizontal),
            DomNode(ITransform.Create(100, 80), Layout.VerticalPreserveHeight),
            DomNode(ITransform.Create(100, 10), Layout.HorizontalPreserveWidth),
            DomNode(ITransform.Create(100, 25), Layout.Absolute),
            DomNode(ITransform.Create(100, 17), Layout.Vertical),
            DomNode(ITransform.Create(100, 91), Layout.Inherit),
        };
        var expectedHeights = new int[] {
            24,
            15,
            80,
            10,
            25,
            17,
            91
        };

        var result = Core.LayoutManager.Layout(100, 120, 100, 120, 0, 0, Layout.VerticalPreserveHeight, children).ToList();
        var expectedY = 0;
        Assert.AreEqual(children.Length, result.Count);
        for (int i = 0; i < result.Count; i++) {
            Assert.AreEqual(children[i], result[i].DomNode);
            Assert.AreEqual(0, result[i].XOffset);
            Assert.AreEqual(expectedY, result[i].YOffset);
            Assert.AreEqual(100, result[i].Width);
            Assert.AreEqual(expectedHeights[i], result[i].Height);
            if (children[i].Layout == Layout.Inherit) {
                Assert.AreEqual(Layout.VerticalPreserveHeight, result[i].Layout);
            } else {
                Assert.AreEqual(children[i].Layout, result[i].Layout);
            }
            expectedY += expectedHeights[i];
        }
    }

    [Test]
    public void Vertical_Preserve_Height_Layout_Respects_Rational_Height_Provided_By_Transform() {
        var children = new IDomNode[] {
            DomNode(ITransform.Create(1, .2), Layout.Relative),
            DomNode(ITransform.Create(1, .2), Layout.Horizontal),
            DomNode(ITransform.Create(1, .1), Layout.VerticalPreserveHeight),
            DomNode(ITransform.Create(1, .1), Layout.HorizontalPreserveWidth),
            DomNode(ITransform.Create(1, .3), Layout.Absolute),
            DomNode(ITransform.Create(1, .05), Layout.Vertical),
            DomNode(ITransform.Create(1, .05), Layout.Inherit),
        };
        var expectedHeights = new int[] {
            24,
            24,
            12,
            12,
            36,
            6,
            6
        };

        var result = Core.LayoutManager.Layout(100, 120, 100, 120, 0, 0, Layout.VerticalPreserveHeight, children).ToList();
        var expectedY = 0;
        Assert.AreEqual(children.Length, result.Count);
        for (int i = 0; i < result.Count; i++) {
            Assert.AreEqual(children[i], result[i].DomNode);
            Assert.AreEqual(0, result[i].XOffset);
            Assert.AreEqual(expectedY, result[i].YOffset);
            Assert.AreEqual(100, result[i].Width);
            Assert.AreEqual(expectedHeights[i], result[i].Height);
            if (children[i].Layout == Layout.Inherit) {
                Assert.AreEqual(Layout.VerticalPreserveHeight, result[i].Layout);
            } else {
                Assert.AreEqual(children[i].Layout, result[i].Layout);
            }
            expectedY += expectedHeights[i];
        }
    }


    [Test]
    public void Horizontal_Layout_Respects_Weight_Provided_By_Transform() {
        var children = new IDomNode[] {
            DomNode(ITransform.Create(12), Layout.Relative),
            DomNode(ITransform.Create(6), Layout.Horizontal),
            DomNode(ITransform.Create(12), Layout.VerticalPreserveHeight),
            DomNode(ITransform.Create(3), Layout.HorizontalPreserveWidth),
            DomNode(ITransform.Create(2), Layout.Absolute),
            DomNode(ITransform.Create(1), Layout.Vertical),
            DomNode(ITransform.Create(4), Layout.Inherit),
            DomNode(ITransform.Create(8), Layout.Horizontal),
            DomNode(ITransform.Create(12), Layout.HorizontalPreserveWidth),
        };
        var expectedWidths = new int[] {
            24,
            12,
            24,
            6,
            4,
            2,
            8,
            16,
            24
        };

        var result = Core.LayoutManager.Layout(120, 100, 120, 100, 0, 0, Layout.Horizontal, children).ToList();
        var expectedX = 0;
        Assert.AreEqual(children.Length, result.Count);
        for (int i = 0; i < result.Count; i++) {
            Assert.AreEqual(children[i], result[i].DomNode);
            Assert.AreEqual(expectedX, result[i].XOffset);
            Assert.AreEqual(0, result[i].YOffset);
            Assert.AreEqual(expectedWidths[i], result[i].Width);
            Assert.AreEqual(100, result[i].Height);
            if (children[i].Layout == Layout.Inherit) {
                Assert.AreEqual(Layout.Horizontal, result[i].Layout);
            } else {
                Assert.AreEqual(children[i].Layout, result[i].Layout);
            }
            expectedX += expectedWidths[i];
        }
    }

    [Test]
    public void Horizontal_Preserve_Height_Layout_Respects_Width_Provided_By_Transform() {
        var children = new IDomNode[] {
            DomNode(ITransform.Create(24, 100), Layout.Relative),
            DomNode(ITransform.Create(15, 100), Layout.Horizontal),
            DomNode(ITransform.Create(80, 100), Layout.VerticalPreserveHeight),
            DomNode(ITransform.Create(10, 100), Layout.HorizontalPreserveWidth),
            DomNode(ITransform.Create(25, 100), Layout.Absolute),
            DomNode(ITransform.Create(17, 100), Layout.Vertical),
            DomNode(ITransform.Create(91, 100), Layout.Inherit),
        };
        var expectedWidths = new int[] {
            24,
            15,
            80,
            10,
            25,
            17,
            91
        };

        var result = Core.LayoutManager.Layout(120, 100, 120, 100, 0, 0, Layout.HorizontalPreserveWidth, children).ToList();
        var expectedX = 0;
        Assert.AreEqual(children.Length, result.Count);
        for (int i = 0; i < result.Count; i++) {
            Assert.AreEqual(children[i], result[i].DomNode);
            Assert.AreEqual(expectedX, result[i].XOffset);
            Assert.AreEqual(0, result[i].YOffset);
            Assert.AreEqual(expectedWidths[i], result[i].Width);
            Assert.AreEqual(100, result[i].Height);
            if (children[i].Layout == Layout.Inherit) {
                Assert.AreEqual(Layout.HorizontalPreserveWidth, result[i].Layout);
            } else {
                Assert.AreEqual(children[i].Layout, result[i].Layout);
            }
            expectedX += expectedWidths[i];
        }
    }

    [Test]
    public void Horizontal_Preserve_Width_Layout_Respects_Rational_Height_Provided_By_Transform() {
        var children = new IDomNode[] {
            DomNode(ITransform.Create(.2, 1), Layout.Relative),
            DomNode(ITransform.Create(.2, 1), Layout.Horizontal),
            DomNode(ITransform.Create(.1, 1), Layout.VerticalPreserveHeight),
            DomNode(ITransform.Create(.1, 1), Layout.HorizontalPreserveWidth),
            DomNode(ITransform.Create(.3, 1), Layout.Absolute),
            DomNode(ITransform.Create(.05, 1), Layout.Vertical),
            DomNode(ITransform.Create(.05, 1), Layout.Inherit),
        };
        var expectedWidths = new int[] {
            24,
            24,
            12,
            12,
            36,
            6,
            6
        };

        var result = Core.LayoutManager.Layout(120, 100, 120, 100, 0, 0, Layout.HorizontalPreserveWidth, children).ToList();
        var expectedX = 0;
        Assert.AreEqual(children.Length, result.Count);
        for (int i = 0; i < result.Count; i++) {
            Assert.AreEqual(children[i], result[i].DomNode);
            Assert.AreEqual(expectedX, result[i].XOffset);
            Assert.AreEqual(0, result[i].YOffset);
            Assert.AreEqual(expectedWidths[i], result[i].Width);
            Assert.AreEqual(100, result[i].Height);
            if (children[i].Layout == Layout.Inherit) {
                Assert.AreEqual(Layout.HorizontalPreserveWidth, result[i].Layout);
            } else {
                Assert.AreEqual(children[i].Layout, result[i].Layout);
            }
            expectedX += expectedWidths[i];
        }
    }

    [Test]
    public void Absolute_Layout_Respects_Position_And_Size_Proveded_By_Transform() {
        var children = new[] {
            DomNode(ITransform.Create(10, 20, 100, 80), Layout.Relative),
            DomNode(ITransform.Create(10, 20, 100, 80), Layout.Horizontal),
            DomNode(ITransform.Create(10, 20, 100, 80), Layout.VerticalPreserveHeight),
            DomNode(ITransform.Create(10, 20, 100, 80), Layout.HorizontalPreserveWidth),
            DomNode(ITransform.Create(10, 20, 100, 80), Layout.Absolute),
            DomNode(ITransform.Create(10, 20, 100, 80), Layout.Vertical),
            DomNode(ITransform.Create(10, 20, 100, 80), Layout.Inherit),
        };

        var result = Core.LayoutManager.Layout(120, 100, 0, 0, 10, 8, Layout.Absolute, children).ToList();

        Assert.AreEqual(children.Length, result.Count);
        foreach (var (item, child) in result.Zip(children))
        {
            Assert.AreEqual(item.Height, 80);
            Assert.AreEqual(item.Width, 100);
            Assert.AreEqual(item.DomNode, child);
            if (child.Layout == Layout.Inherit) {
                Assert.AreEqual(Layout.Absolute, item.Layout);
            } else {
                Assert.AreEqual(child.Layout, item.Layout);
            }
            Assert.AreEqual(item.XOffset, 10);
            Assert.AreEqual(item.YOffset, 20);
        }

    }
}
