using System;
using System.Collections.Generic;

namespace ConsoleUserInterface.Core.Extensions {
    internal static class TextStyleExtension {

        internal static IEnumerable<string> Split(this string str, int n) {
            if (string.IsNullOrEmpty(str) || n < 1) {
                throw new ArgumentException("");
            }

            var rest = str;

            while (rest != "") {
                var newLine = rest.IndexOf('\n');
                if (newLine >= n || newLine == -1) {
                    if (rest.Length <= n) {
                        yield return rest;
                        yield break;
                    }
                    var space = rest.LastIndexOf(' ', n);
                    if (space == -1) {
                        yield return rest[..n];
                        rest = rest[n..];
                    } else {
                        yield return rest[..space];
                        rest = rest[(space + 1)..];
                    }
                } else {
                    yield return rest[..newLine];
                    rest = rest[(newLine + 1)..];
                }
            }
        }

        internal static string Ellipsis(this string str, string ellipsis, int maxText) {
            if (str.Length <= maxText) {
                return str;
            } else {
                return $"{str[0..(maxText - ellipsis.Length)]}{ellipsis}";
            }
        }

        internal static string[] SplitLines(this string input) => 
            input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
    }
}
