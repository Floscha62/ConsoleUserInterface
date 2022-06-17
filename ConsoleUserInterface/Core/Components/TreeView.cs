﻿using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleUserInterface.Core.Extensions;
using static ConsoleUserInterface.Core.Components.Components;

namespace ConsoleUserInterface.Core.Components {

    public abstract class TreeElement<T> where T : TreeElement<T> {

        internal bool Leaf => !GetChildren().Any();
        internal bool Open { get; set; }
        public abstract string Label { get; }

        internal T this[List<int> selectedElement] {
            get {
                if (selectedElement.Count == 0) {
                    return (T)this;
                }
                return GetChildren().ElementAt(selectedElement[0])[selectedElement.Skip(1).ToList()];
            }
        }

        internal List<int> Previous(List<int> currentElement) {
            if (currentElement.Count == 0) {
                return currentElement; //Closed sub tree, leaf or currently selected element
            }

            var nextOfChild = GetChildren().ElementAt(currentElement[0]).Previous(currentElement.Skip(1).ToList());

            if (Enumerable.SequenceEqual(currentElement.Skip(1), nextOfChild)) {
                if (currentElement[0] > 0 && GetChildren().ElementAt(currentElement[0] - 1).Open) {
                    return new List<int>() { currentElement[0] - 1 }.Concat(GetChildren().ElementAt(currentElement[0] - 1).Last()).ToList();
                } else if (currentElement[0] > 0) {
                    return new List<int>() { currentElement[0] - 1 };
                } else {
                    return currentElement.Take(currentElement.Count - 1).ToList(); // start of sub elements reached
                }
            } else {
                return nextOfChild.Prepend(currentElement[0]).ToList();
            }
        }

        internal List<int> Next(List<int> currentElement) {
            if (currentElement.Count == 0 && (!Open || Leaf)) {
                return currentElement; //Closed sub tree, leaf or currently selected element
            }

            if (currentElement.Count == 0) {
                return new List<int>() { 0 };
            }
            var children = GetChildren();
            var nextOfChild = children.ElementAt(currentElement[0]).Next(currentElement.Skip(1).ToList());
            if (Enumerable.SequenceEqual(currentElement.Skip(1), nextOfChild)) {
                if (currentElement[0] < children.Count() - 1) {
                    return new List<int>() { currentElement[0] + 1 };
                } else {
                    return currentElement; // end of sub elements reached
                }
            } else {
                return nextOfChild.Prepend(currentElement[0]).ToList();
            }
        }

        internal List<int> Last() {
            if (Leaf || !Open) {
                return new List<int>();
            }

            var childCount = GetChildren().Count();
            return GetChildren().ElementAt(childCount - 1).Last().Prepend(childCount - 1).ToList();
        }

        internal IEnumerable<T> GetChildrenRecursive() =>
            GetChildren().SelectMany(c => c.GetChildrenRecursive()).Prepend(this[new List<int>()]);

        public abstract IEnumerable<T> GetChildren();
    }

    internal class TreeView<T> : VerticalComponent<TreeView<T>.Props, TreeView<T>.State> where T : TreeElement<T> {
        internal record Props(T RootElement, Action<T> OnSelectElement);
        internal record State(List<int> SelectedElement, List<int> HoveredElement);

        protected override State StartingState => new State(new List<int>() { }, new List<int> { });

        public TreeView(Props props) : base(props) { }

        public override bool ReceiveKey(ConsoleKeyInfo key) {
            switch (key) {
                case ConsoleKeyInfo(ConsoleKey.RightArrow, _, _):
                    props.RootElement[state.HoveredElement].Open = true;
                    return true;
                case ConsoleKeyInfo(ConsoleKey.LeftArrow, _, _):
                    props.RootElement[state.HoveredElement].Open = false;
                    return true;
                case ConsoleKeyInfo(ConsoleKey.DownArrow, _, _):
                    state = state with { HoveredElement = props.RootElement.Next(state.HoveredElement) };
                    return true;
                case ConsoleKeyInfo(ConsoleKey.UpArrow, _, _):
                    state = state with { HoveredElement = props.RootElement.Previous(state.HoveredElement) };
                    return true;
                case ConsoleKeyInfo(ConsoleKey.Enter, _, _) when !Enumerable.SequenceEqual(state.HoveredElement, state.SelectedElement):
                    props.OnSelectElement(props.RootElement[state.HoveredElement]);
                    state = state with { SelectedElement = state.HoveredElement };
                    return true;
            }
            return false;
        }

        public override (IEnumerable<IComponent>, IEnumerable<FormattingRange>) Render() =>
            (RenderTree(props.RootElement), Enumerable.Empty<FormattingRange>());

        private IEnumerable<IComponent> RenderTree(TreeElement<T> current) {
            var flattenedTree = FlattenTree(current[new List<int>()]);
            var selected = current[state.SelectedElement];
            var hovered = current[state.HoveredElement];

            foreach (var (elem, depth) in flattenedTree) {
                var indent = Label(new string(' ', depth));
                var indicator = Label(elem.Equals(hovered) ? "→  " : "   ");
                var status = Label(elem.Leaf ? " " : elem.Open ? "\\" : "-");
                var label = Label(elem.Label, elem.Equals(selected));
                yield return Group(Layout.HORIZONTAL, indent, indicator, status, label);
            }
        }

        private IEnumerable<(T element, int depth)> FlattenTree(T root, int depth = 0) {
            return !root.Leaf && root.Open ?
                root.GetChildren().SelectMany(t => FlattenTree(t, depth + 1)).Prepend((root, depth)) :
                new List<(T, int)>() { (root, depth) };
        }
    }
}
