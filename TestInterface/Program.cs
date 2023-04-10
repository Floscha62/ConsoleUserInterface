using ConsoleUserInterface.Core;
using ConsoleUserInterface.Core.Components;
using LoggingConsole;

namespace TestInterface {
    public class Program {

        record Tree(string Label, List<Tree> Children) : ITreeElement<Tree> {

            public Tree(string label) : this(label, new()) { }

            public List<Tree> GetChildren() => Children;
        }

        public static void Main() {
            LoggingFactory.enableConsole = true;

            using var logger = LoggingFactory.Create(typeof(Program));

            var tree = new Tree("Root", new() {
                new("Leaf - 1"),
                new("Tree - 2", new (){ new("Leaf - 2 - 1"), new("Leaf - 2 - 2") } ),
                new("Leaf - 3")
            });

            var component = Components.Container(ITransform.Create(), Layout.HorizontalPreserveHeight, true, 
                Components.TreeView(
                    ITransform.Create(20, 100), 
                    tree, 
                    t => logger.Info($"'Tree - {t.Label}' was selected")
                )
            );

            var renderer = new Renderer("Test Application", component);
            renderer.Start();
        }
    }
}