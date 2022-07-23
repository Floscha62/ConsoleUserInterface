using System.Collections.Generic;

namespace ConsoleUserInterface.Core {
    public record BaseRenderResult(string Text, IEnumerable<FormattingRange> FormattingRanges) { }
}
