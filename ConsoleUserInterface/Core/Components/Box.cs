using System.Collections.Generic;
using System.Linq;
using static ConsoleUserInterface.Core.Components.Components;

namespace ConsoleUserInterface.Core.Components {
    internal class Box : VerticalComponent<Box.Props, Box.State> {
        internal record Props(IComponent Child, BorderSet Border);
        internal record State();
        public record BorderSet(char VerticalBorder, char Corner, char HorizontalBorder);

        public Box(Props props) : base(props) { }

        public override bool ReceiveKey(System.ConsoleKeyInfo key) =>
            props.Child.ReceiveKey(key);

        public override (IEnumerable<IComponent>, IEnumerable<FormattingRange>) Render() {
            var width = props.Child.Width;
            var height = props.Child.Height;
            return (new IComponent[] {
                Label($"{props.Border.Corner}{new string(props.Border.HorizontalBorder, width + 2)}{props.Border.Corner}"),
                Label($"{props.Border.VerticalBorder}{new string(' ', width + 2)}{props.Border.VerticalBorder}"),
                Group(Layout.HORIZONTAL,
                    Group(Layout.VERTICAL, Enumerable.Repeat(Label($"{props.Border.VerticalBorder} "), height)),
                    props.Child,
                    Group(Layout.VERTICAL, Enumerable.Repeat(Label($" {props.Border.VerticalBorder}"), height))
                ),
                Label($"{props.Border.VerticalBorder}{new string(' ', width + 2)}{props.Border.VerticalBorder}"),
                Label($"{props.Border.Corner}{new string(props.Border.HorizontalBorder, width + 2)}{props.Border.Corner}")
            }, Enumerable.Empty<FormattingRange>());
        }

        protected override State StartingState => new State();

    }
}
