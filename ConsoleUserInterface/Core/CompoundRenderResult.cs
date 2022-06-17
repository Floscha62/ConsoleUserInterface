using System.Collections.Generic;

namespace ConsoleUserInterface.Core {
    public record CompoundRenderResult(IEnumerable<IComponent> Components, IEnumerable<FormattingRange> FormattingRanges) { }
}
