using System;
using System.Linq;

namespace ConsoleUserInterface.Core.Utils {

    internal static class FunctionUtils {

        public static bool Any<T1, T2>(T1 t1, T2 t2, params Func<T1, T2, bool>[] predicates) =>
            predicates.Any(p => p(t1, t2));

    }
}
