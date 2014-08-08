using System.Collections.Generic;
using System.Reflection;

namespace Hammock.Extensions
{
    internal static class ReflectionExtensions
    {
        public static IEnumerable<T> GetCustomAttributes<T>(this PropertyInfo info, bool inherit)
            where T : class
        {
            var attributes = info.GetCustomAttributes(typeof (T), inherit);
#if !WINRT
            return attributes.ToEnumerable<T>();
#else
						return attributes.ToEnumerable<T>();
#endif
        }

        public static object GetValue(this object instance, string property)
        {
#if !WINRT
            var info = instance.GetType().GetProperty(property);
#else
					var info = instance.GetType().GetTypeInfo().GetDeclaredProperty(property);
#endif
            if (info != null)
            {
                var value = info.GetValue(instance, null);
                return value;
            }
            return null; 
        }

        public static void SetValue(this object instance, string property, object value)
        {
#if !WINRT
            var info = instance.GetType().GetProperty(property);
#else
            var info = instance.GetType().GetTypeInfo().GetDeclaredProperty(property);
#endif
            info.SetValue(instance, value, null);
        }
    }
}