﻿using System;
using System.Collections.Generic;
using System.Linq;
using static ConsoleUserInterface.Core.Extensions.Destructors;

namespace ConsoleUserInterface.Core.Components {
    public class HorizontalLayoutGroup : HorizontalComponent<HorizontalLayoutGroup.Props, HorizontalLayoutGroup.State> {

        public record Props(IEnumerable<(int weight, IComponent comp)> Components);
        public record State(int FocusedIndex);

        private readonly double totalWeight;

        public HorizontalLayoutGroup(Props props) : base(props) {
            totalWeight = props.Components.Sum(t => t.weight);
        }

        public override bool ReceiveKey(ConsoleKeyInfo key) {
            if (state.FocusedIndex > 0 && props.Components.ElementAt(state.FocusedIndex - 1).comp.ReceiveKey(key)) {
                return true;
            }

            switch (key) {
                case ConsoleKeyInfo(ConsoleKey.Tab, _, _):
                    state = state with { FocusedIndex = (state.FocusedIndex + 1) % (props.Components.Count() + 1) };
                    return state.FocusedIndex > 0 && props.Components.ElementAt(state.FocusedIndex - 1).comp.ReceiveKey(key);
            }

            return false;
        }

        public override (IEnumerable<IComponent>, IEnumerable<FormattingRange>) Render() {
            int width = Console.WindowWidth;
            return (props.Components.Select((tuple, i) => {
                tuple.comp.SetConsole(Console);
                return new Container(new Container.Props(
                    tuple.comp, 
                    (int)Math.Floor(tuple.weight / totalWeight * width), 
                    tuple.comp.Height,
                    i == state.FocusedIndex - 1 && !(tuple.comp is VerticalLayoutGroup) && !(tuple.comp is HorizontalLayoutGroup)
                    )
                );
            }), Enumerable.Empty<FormattingRange>());
        }

        protected override State StartingState => new State(0);

    }
}
