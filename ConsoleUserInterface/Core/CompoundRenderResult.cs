using System.Collections.Generic;
using System.Linq;

namespace ConsoleUserInterface.Core {
    public record CompoundRenderResult(IEnumerable<IComponent> Components, IEnumerable<FormattingRange> FormattingRanges) {
        public CompoundRenderResult(IComponent component) : this(new[] { component }, Enumerable.Empty<FormattingRange>()) {

        }
    }
}
