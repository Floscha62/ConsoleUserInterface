using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleUserInterface.Core {

    public abstract class VerticalComponent<Props, State> : Component<Props, State>, IVerticalComponent {
        public override int Width => Render().components.Max(c => c.Width);
        public override int Height => Render().components.Sum(c => c.Height);

        public VerticalComponent(Props props) : base(props) { }
    }

    public abstract class HorizontalComponent<Props, State> : Component<Props, State>, IHorizontalComponent {
        public override int Width => Render().components.Sum(c => c.Width);
        public override int Height => Render().components.Max(c => c.Height);

        public HorizontalComponent(Props props) : base(props) { }
    }

    public abstract class NoLayoutComponent<Props, State> : Component<Props, State>, IHorizontalComponent {
        public override int Width => 0;
        public override int Height => 0;

        public NoLayoutComponent(Props props) : base(props) { }
    }

    public abstract class Component<Props, State> : IComponent {
        public abstract int Width { get; }
        public abstract int Height { get; }
        public int LayerIndex { get; }

        public IConsole Console { get; internal set; }
        void IComponent.SetConsole(IConsole console) => Console = console;

        protected readonly Props props;
        protected State state;

        public Component(Props props, int layerIndex = 0) {
            this.props = props;
            this.state = this.StartingState;
            this.LayerIndex = layerIndex;
        }

        public abstract (IEnumerable<IComponent> components, IEnumerable<FormattingRange> formattings) Render();
        public abstract bool ReceiveKey(ConsoleKeyInfo key);


        protected abstract State StartingState { get; }

    }

    /// <summary>
    /// This component represents a base component. The simplest components to be used by the others.
    /// </summary>
    /// <typeparam name="Props"> The props define the behaviour and starting state of the component. </typeparam>
    /// <typeparam name="State"> The state is changed by the interaction with the user. </typeparam>
    public abstract class BaseComponent<Props, State> : IBaseComponent {

        /// <summary> <inheritdoc/> </summary>
        public abstract int Width { get; }

        /// <summary> <inheritdoc/> </summary>
        public abstract int Height { get; }

        /// <summary> <inheritdoc/> </summary>
        public int LayerIndex { get; }
        public IConsole Console { get; internal set; }
        void IComponent.SetConsole(IConsole console) => Console = console;

        /// <summary> Provides the starting state of the component. </summary>
        protected abstract State StartingState { get; }

        /// <summary> The props of the component to determine behaviour. </summary>
        protected readonly Props props;
        /// <summary> The current state of the component. </summary>
        protected State state;

        /// <summary>
        /// Creates a new base component with the given props.
        /// </summary>
        /// <param name="props"> The props to determine behaviour and starting state. </param>
        /// <param name="layerIndex"> The index of the layer relative to the parent component. </param>
        public BaseComponent(Props props, int layerIndex = 0) {
            this.props = props;
            this.state = StartingState;
            this.LayerIndex = layerIndex;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public (IEnumerable<IComponent> components, IEnumerable<FormattingRange> formattings) Render() => 
            (Enumerable.Empty<IComponent>(), Enumerable.Empty<FormattingRange>());
        
        /// <summary>
        /// Renders the component as a string to be written to the screen.
        /// </summary>
        /// <returns> The string to render. </returns>
        public abstract (string text, List<FormattingRange> formattings) RenderString();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public abstract bool ReceiveKey(ConsoleKeyInfo key);
    }

    internal interface IBaseComponent : IComponent {

        (string text, List<FormattingRange> formattings) RenderString();
    }
    internal interface IVerticalComponent : IComponent { }
    internal interface IHorizontalComponent : IComponent { }
    internal interface INoLayoutComponent : IComponent { }
    internal interface IConstOffsetComponent : IComponent {

        (int x, int y) Offset(IConsole console);

        IComponent RenderComponent();
    }

    /// <summary>
    /// A component to show the information or provide interaction.
    /// </summary>
    public interface IComponent {

        /// <summary> The width of the component. </summary>
        public int Width { get; }
        /// <summary> The height of the component. </summary>
        public int Height { get; }
        /// <summary> The index of the layer relative to the parent. </summary>
        public int LayerIndex { get; }
        /// <summary> The console that can be used during rendering. </summary>
        public IConsole Console { get; }

        internal void SetConsole(IConsole console);

        /// <summary>
        /// Provide the sub-components of this component. <br/>
        /// <br/>
        /// For Layout see 
        /// <see cref="VerticalComponent{Props, State}"/>, 
        /// <see cref="HorizontalComponent{Props, State}"/>, 
        /// <see cref="NoLayoutComponent{Props, State}"/>
        /// </summary>
        /// <returns> The components rendered as part of this component.</returns>
        (IEnumerable<IComponent> components, IEnumerable<FormattingRange> formattings) Render();

        /// <summary>
        /// Receive the key pressed by the user.
        /// </summary>
        /// <param name="key">The key pressed by the user.</param>
        /// <returns> true, if the key was used by the components; false, otherwise. </returns>
        bool ReceiveKey(ConsoleKeyInfo key);
    }
}
