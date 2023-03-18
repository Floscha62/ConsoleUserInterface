using System.Collections.Generic;
using System.Linq;

namespace ConsoleUserInterface.Core {
    public record CompoundRenderResult(IEnumerable<(IComponent, bool inFocus)> Components, IEnumerable<FormattingRange> FormattingRanges) {
        public CompoundRenderResult(IComponent component, bool inFocus = false) : this(new[] { (component, inFocus) }, Enumerable.Empty<FormattingRange>()) {

        }

        public CompoundRenderResult(IComponent component, bool inFocus, IEnumerable<FormattingRange> formattingRanges) : 
            this(new[] { (component, inFocus) }, formattingRanges) {

        }
        public CompoundRenderResult(IEnumerable<(IComponent, bool inFocus)> components) :
            this(components, Enumerable.Empty<FormattingRange>()) {

        }
    }
}
