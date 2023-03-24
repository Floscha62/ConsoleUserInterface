
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleUserInterface.Core.Components {
    public static class Form {
        [AttributeUsage(AttributeTargets.Property)]
        public class LabelAttribute : Attribute {
            internal readonly string label;

            public LabelAttribute(string label) {
                this.label = label;
            }
        }

        [AttributeUsage(AttributeTargets.Property)]
        public class TextAreaAttribute : Attribute {
        }


        [AttributeUsage(AttributeTargets.Property)]
        public class DisplayValueCountAttribute : Attribute {

            internal readonly int value;

            public DisplayValueCountAttribute(int value) {
                this.value = value;
            }
        }

        public static IComponent ForValue<T>(ITransform transform, T value, Action<T> onChange, Action onCancel) {
            List<IComponent> comps = Components(Copy(value), onChange, OnCancel).ToList();
            void OnCancel() {
                comps = Components(Copy(value), onChange, OnCancel).ToList();
                onCancel();
            } 

            IEnumerable<IComponent> Comps() {
                foreach (var comp in comps) yield return comp;
            }

            return Core.Components.Components.Container(transform, Layout.VERTICAL, Comps());
        }

        internal static T Copy<T>(T value) {
            var type = typeof(T);
            var props = type.GetProperties();

            var copy = (T)Activator.CreateInstance(type);

            foreach (var p in props) {
                if (p.CanRead && p.CanWrite) {
                    p.SetValue(copy, p.GetValue(value));
                }
            }
            return copy;
        }

        internal static IEnumerable<IComponent> Components<T>(T value, Action<T> onChange, Action onCancel) {
            var type = typeof(T);
            var props = type.GetProperties();
            foreach (var p in props) {
                var label = p.GetCustomAttributes(typeof(LabelAttribute), false).Cast<LabelAttribute>().LastOrDefault();
                yield return Core.Components.Components.Label(ITransform.Create(1), label?.label ?? p.Name, underlined: true);

                if (p.PropertyType == typeof(string)) {
                    var textArea = p.GetCustomAttributes(typeof(TextAreaAttribute), false).Any();
                    if (textArea) {
                        yield return Core.Components.Components.TextArea(ITransform.Create(1), (string)p.GetValue(value), s => p.SetValue(value, s), 100);
                    } else {
                        yield return Core.Components.Components.TextField(ITransform.Create(1), (string)p.GetValue(value), s => p.SetValue(value, s));
                    }
                }

                if (p.PropertyType.IsEnum) {
                    var list = Enum.GetValues(p.PropertyType).OfType<object>().ToList();
                    var displayValueCount = p.GetCustomAttributes(typeof(DisplayValueCountAttribute), false)
                        .Cast<DisplayValueCountAttribute>()
                        .FirstOrDefault()?.value;
                    yield return Core.Components.Components.ListSelection(ITransform.Create(1), list, o => o.ToString(), o => p.SetValue(value, o), startIndex:list.IndexOf(p.GetValue(value)), displayedSize: displayValueCount);
                }
            }
            if(onChange != null) yield return Core.Components.Components.Button(ITransform.Create(1), "Save", action: () => onChange(value), underlined: true);
            if(onCancel != null) yield return Core.Components.Components.Button(ITransform.Create(1), "OnCancel", action: onCancel, underlined: true);
        }
    }
}
