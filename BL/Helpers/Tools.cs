using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
namespace Helpers
{
    internal static class Tools
    {
        public static string ToStringProperty<T>(this T t)
        {
            if (t == null)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            Type type = t.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (var prop in properties)
            {
                object value = prop.GetValue(t);

                if (value == null)
                {
                    sb.AppendLine($"{prop.Name}: null");
                }
                else if (value is IEnumerable enumerable && !(value is string))
                {
                    sb.AppendLine($"{prop.Name}: [");

                    foreach (var item in enumerable)
                    {
                        if (item == null)
                        {
                            sb.AppendLine("  null");
                        }
                        else
                        {
                            // אם הפריט הוא טיפוס פשוט - מציגים אותו ישירות
                            Type itemType = item.GetType();
                            if (IsSimpleType(itemType))
                            {
                                sb.AppendLine($"  {item}");
                            }
                            else
                            {
                                // אם הפריט הוא אובייקט מורכב - קוראים לו רקורסיבית
                                sb.AppendLine($"  {item.ToStringProperty()}");
                            }
                        }
                    }

                    sb.AppendLine("]");
                }
                else
                {
                    sb.AppendLine($"{prop.Name}: {value}");
                }
            }

            return sb.ToString();
        }

        private static bool IsSimpleType(Type type)
        {
            return
                type.IsPrimitive ||
                type.IsEnum ||
                type.Equals(typeof(string)) ||
                type.Equals(typeof(decimal)) ||
                type.Equals(typeof(DateTime)) ||
                type.Equals(typeof(DateTimeOffset)) ||
                type.Equals(typeof(TimeSpan)) ||
                type.Equals(typeof(Guid));
        }
    }
}
