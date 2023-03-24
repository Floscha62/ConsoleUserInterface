using System.Collections.Generic;
using System.Linq;

namespace ConsoleUserInterface.Core {
    public record CompoundRenderResult(IEnumerable<(IComponent, bool inFocus)> Components, IEnumerable<FormattingRange> FormattingRanges, int ZOffset) {
        public CompoundRenderResult(IComponent component, bool inFocus = false, int zOffset = 0) : this(new[] { (component, inFocus) }, Enumerable.Empty<FormattingRange>(), zOffset) {

        }

        public CompoundRenderResult(IComponent component, bool inFocus, IEnumerable<FormattingRange> formattingRanges, int zOffset = 0) : 
            this(new[] { (component, inFocus) }, formattingRanges, zOffset) {

        }
        public CompoundRenderResult(IEnumerable<(IComponent, bool inFocus)> components, int zOffset = 0) :
            this(components, Enumerable.Empty<FormattingRange>(), zOffset) {

        }
    }
}
