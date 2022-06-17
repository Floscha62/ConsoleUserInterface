using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleUserInterface.Core.Extensions;

namespace ConsoleUserInterface.Core.Components {
    internal class Label : BaseComponent<Label.Props, Label.State> {
        internal record Props(string Label, bool Underlined = false, int MaxWidth = -1);
        internal record State();

        public override int Width => props.MaxWidth == -1 ? props.Label
            .SplitLines()
            .Max(s => s.Length) : props.MaxWidth;

        public override int Height => props.Label
            .SplitLines()
            .Length;

        public Label(Props props) : base(props) { }

        public override (string, List<FormattingRange>) RenderString() {
            var text = props.MaxWidth == -1 ? props.Label : string.Join('\r', props.Label.Split(props.MaxWidth));
            return (text, text.Split('\r').Select(
                (l, i) => props.Underlined ? 
                IFormatting.Underline((0, i), (l.Length - 1, i)) : 
                IFormatting.Blank((0, i), (l.Length - 1, i))
            ).ToList());
        }

        public override bool ReceiveKey(ConsoleKeyInfo key) => false;

        protected override State StartingState => new State();
    }
}
