using System.Runtime.CompilerServices;
using static ConsoleUserInterface.Core.Components.TreeViewComponent;

namespace ConsoleUserInterface.Core.Components {

    /// <summary>
    /// This class provides methods to create most basic components.
    /// </summary>
    public static class Components {

        /// <summary>
        /// Creates a container to be used for layouting purposes.
        /// </summary>
        /// <param name="transform"> The transform for the component. </param>
        /// <param name="layout"> The layout to be used for laying out the child components. </param>
        /// <param name="childrenFocusable"> Can the child components be focused? </param>
        /// <param name="components"> The child components of the container. </param>
        /// <returns> The newly created container. </returns>
        public static IComponent Container(ITransform transform, Layout layout, bool childrenFocusable, params IComponent[] components) =>
            Container(transform, layout, childrenFocusable, components.ToList());

        /// <summary>
        /// Creates a container to be used for layouting purposes.
        /// </summary>
        /// <param name="transform"> The transform for the component. </param>
        /// <param name="layout"> The layout to be used for laying out the child components. </param>
        /// <param name="childrenFocusable"> Can the child components be focused? </param>
        /// <param name="components"> The child components of the container. </param>
        /// <returns> The newly created container. </returns>
        public static IComponent Container(ITransform transform, Layout layout, bool childrenFocusable, List<IComponent> components) =>
            new Container(new(layout, childrenFocusable, 0), components, transform);

        /// <summary>
        /// Creates a modal, whose elements are rendered a layer above its parent. The layout is <see cref="Layout.Absolute"/>.
        /// </summary>
        /// <param name="transform"> The transform of the modal. </param>
        /// <param name="childrenFocusable"> Can the child components be focused? </param>
        /// <param name="components"> The child components of the container. </param>
        /// <returns> The newly created modal. </returns>
        public static IComponent Modal(ITransform transform, bool childrenFocusable, params IComponent[] components) =>
            Modal(transform, childrenFocusable, components.ToList());

        /// <summary>
        /// Creates a modal, whose elements are rendered a layer above its parent. The layout is <see cref="Layout.Absolute"/>.
        /// </summary>
        /// <param name="transform"> The transform of the modal. </param>
        /// <param name="childrenFocusable"> Can the child components be focused? </param>
        /// <param name="components"> The child components of the container. </param>
        /// <returns> The newly created modal. </returns>
        public static IComponent Modal(ITransform transform, bool childrenFocusable, List<IComponent> components) =>
            new Container(new(Layout.Absolute, childrenFocusable, 1), components, transform);

        /// <summary>
        /// Creates a text field.
        /// </summary>
        /// <param name="transform"> The transform of the modal. </param>
        /// <param name="startText"> The text, with which the text field is initiated. </param>
        /// <param name="onChange"> Callback, which is called whenever the text is changed in response to a received key. </param>
        /// <returns> The newly created modal. </returns>
        public static IComponent TextField(ITransform transform, string startText, Action<string> onChange) =>
            new TextField(new(startText, onChange), transform);

        /// <summary>
        /// Creates a label.
        /// </summary>
        /// <param name="transform"> The transform of the label. </param>
        /// <param name="label"> The text displayed by this label. </param>
        /// <param name="underlined"> Is the text to be underlined. </param>
        /// <returns> The newly created label. </returns>
        public static IComponent Label(ITransform transform, string label, bool underlined) =>
            new Label(new(label, underlined), transform);

        /// <summary>
        /// Creates a button.
        /// </summary>
        /// <param name="transform"> The transform of the button. </param>
        /// <param name="label"> The text displayed by this button. </param>
        /// <param name="action"> The action performed on receiving a <see cref="ConsoleKey.Enter"/>. </param>
        /// <returns> The newly created button. </returns>
        public static IComponent Button(ITransform transform, string label, Action action) =>
            new Button(new(label, action), transform);

        /// <summary>
        /// Creates a list selection.
        /// </summary>
        /// <typeparam name="T"> The type of items of the list. </typeparam>
        /// <param name="transform"> The transform of the list selection. </param>
        /// <param name="values"> The values available for selection. </param>
        /// <param name="onSelect"> The method called, when the selection changes on receiving an <see cref="ConsoleKey.UpArrow"/> or <see cref="ConsoleKey.DownArrow"/>. </param>
        /// <param name="labelFunc"> The function providing the string representation of the values. <see cref="object.ToString()"/>, if not provided. </param>
        /// <param name="startIndex"> The index which is initally selected. </param>
        /// <returns> The newly created list selection. </returns>
        public static IComponent ListSelection<T>(ITransform transform, List<T> values, Action<T> onSelect, Func<T, string>? labelFunc = null, int startIndex = 0) =>
            new ListSelection<T>(new(values, labelFunc ?? (t => t?.ToString() ?? ""), onSelect, startIndex), transform);

        /// <summary>
        /// Creates a tree view.
        /// </summary>
        /// <typeparam name="T">The type of nodes representing the tree.</typeparam>
        /// <param name="transform">The transform of the tree view.</param>
        /// <param name="rootElement">The root element of the tree.</param>
        /// <param name="onSelect">The method called, when the selection changes on receiving an <see cref="ConsoleKey.Enter"/>.</param>
        /// <returns> The newly created tree view. </returns>
        public static IComponent TreeView<T>(ITransform transform, T rootElement, Action<T> onSelect) where T : ITreeElement<T> =>
            TreeViewComponent.TreeView(transform, rootElement, onSelect);

        /// <summary>
        /// Creates a new functionally implemented component.
        /// </summary>
        /// <typeparam name="P"> The static state of the functional component. </typeparam>
        /// <typeparam name="S"> The dynamic state of the functional component. </typeparam>
        /// <param name="transform"> The transform of the functional component. </param>
        /// <param name="p"> The props provided to the component. </param>
        /// <param name="initialState"> The initial state of the component. </param>
        /// <param name="implementation"> The functional implementation of the component. <see cref="CompoundComponent{Props, State}.Render()"/></param>
        /// <param name="handleKeys"> The key handler of the component <see cref="Component{Props, State}.ReceiveKey(ConsoleKeyInfo)"/>. </param>
        /// <param name="implementationName">The captured argument. Must not be provided. </param>
        /// <returns> The newly created functional component. </returns>
        public static IComponent FunctionComponent<P, S>(
            ITransform transform,
            P p,
            S? initialState,
            Func<P, S, Action<S>, Callbacks, CompoundRenderResult> implementation,
            Func<ConsoleKeyInfo, P, S, Action<S>, bool>? handleKeys = null, 
            [CallerArgumentExpression("implementation")] string? implementationName = null
        ) where S : new() => new FunctionCompoundComponent<P, S>(new(p, initialState, implementationName, handleKeys, implementation), transform);

        /// <summary>
        /// Creates a new functionally implemented component.
        /// </summary>
        /// <typeparam name="P"> The static state of the functional component. </typeparam>
        /// <typeparam name="S"> The dynamic state of the functional component. </typeparam>
        /// <param name="transform"> The transform of the functional component. </param>
        /// <param name="p"> The props provided to the component. </param>
        /// <param name="initialState"> The initial state of the component. </param>
        /// <param name="implementation"> The functional implementation of the component. <see cref="BaseComponent{Props, State}.Render()"/></param>
        /// <param name="handleKeys"> The key handler of the component <see cref="Component{Props, State}.ReceiveKey(ConsoleKeyInfo)"/>. </param>
        /// <param name="implementationName">The captured argument. Must not be provided. </param>
        /// <returns> The newly created functional component. </returns>
        public static IComponent FunctionComponent<P, S>(
            ITransform transform,
            P p,
            S? initialState,
            Func<P, S, Action<S>, Callbacks, BaseRenderResult> implementation,
            Func<ConsoleKeyInfo, P, S, Action<S>, bool>? handleKeys = null,
            [CallerArgumentExpression("implementation")] string? implementationName = null
        ) where S : new() => new FunctionBaseComponent<P, S>(new(p, initialState, implementationName, handleKeys, implementation), transform);
    }
}
