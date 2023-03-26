namespace ConsoleUserInterfaceWithDOM.Core {
    public record CompoundRenderResult(Layout Layout, IEnumerable<IComponent> Components, bool SelfFocusable, bool ComponentsFocusable, int ZOffset) {
        public CompoundRenderResult(IComponent component, Layout layout = Layout.INHERIT, bool selfFocusable = true, bool componentsFocusable = true, int zOffset = 0) :
            this(layout, new[] {component}, selfFocusable, componentsFocusable, zOffset) {

        }

        public CompoundRenderResult(IEnumerable<IComponent> components, Layout layout = Layout.INHERIT, bool selfFocusable = true, bool componentsFocusable = true, int zOffset = 0) :
            this(layout, components, selfFocusable, componentsFocusable, zOffset) {

        }
    }
}
