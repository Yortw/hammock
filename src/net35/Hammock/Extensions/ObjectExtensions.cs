using System;

#if NETCF
using System.Linq;
#endif

#if WINRT
using System.Reflection;
using System.Linq;
#endif

namespace Hammock.Extensions
{
    internal static class ObjectExtensions
    {
        public static bool Implements(this object instance, Type interfaceType)
        {
#if !WINRT
            return interfaceType.IsGenericTypeDefinition
                       ? instance.ImplementsGeneric(interfaceType)
                       : interfaceType.IsAssignableFrom(instance.GetType());
#else
					var ti = interfaceType.GetTypeInfo();
					return ti.IsGenericTypeDefinition
										 ? instance.ImplementsGeneric(interfaceType)
										 : ti.IsAssignableFrom(instance.GetType().GetTypeInfo());
#endif
        }

        private static bool ImplementsGeneric(this Type type, Type targetType)
        {
#if !WINRT
            var interfaceTypes = type.GetInterfaces();
#else
					var interfaceTypes = type.GetTypeInfo().ImplementedInterfaces;
#endif
            foreach (var interfaceType in interfaceTypes)
            {
#if !WINRT
							if (!interfaceType.IsGenericType)
#else
                if (!interfaceType.GetTypeInfo().IsGenericType)
#endif
							{
                    continue;
                }

                if (interfaceType.GetGenericTypeDefinition() == targetType)
                {
                    return true;
                }
            }

#if !WINRT
            var baseType = type.BaseType;
#else
						var baseType = type.GetTypeInfo().BaseType;
#endif
            if (baseType == null)
            {
                return false;
            }

#if !WINRT
            return baseType.IsGenericType
                       ? baseType.GetGenericTypeDefinition() == targetType
                       : baseType.ImplementsGeneric(targetType);
#else
						return baseType.GetTypeInfo().IsGenericType
											 ? baseType.GetGenericTypeDefinition() == targetType
											 : baseType.ImplementsGeneric(targetType);
#endif
        }

        private static bool ImplementsGeneric(this object instance, Type targetType)
        {
            return instance.GetType().ImplementsGeneric(targetType);
        }

        public static Type GetDeclaredTypeForGeneric(this object instance, Type interfaceType)
        {
            return instance.GetType().GetDeclaredTypeForGeneric(interfaceType);
        }

        public static Type GetDeclaredTypeForGeneric(this Type baseType, Type interfaceType)
        {
            var type = default(Type);

            if (baseType.ImplementsGeneric(interfaceType))
            {
#if NETCF
                var generic = baseType.GetInterfaces()
                    .Single(i => i.FullName.Equals(interfaceType.FullName));
#elif WINRT
							var generic = (from i in interfaceType.GetTypeInfo().ImplementedInterfaces 
														 where String.Compare(i.FullName, interfaceType.FullName, StringComparison.OrdinalIgnoreCase) == 0 
														 select i).First();
#else
                var generic = baseType.GetInterface(interfaceType.FullName, true);
#endif

#if !WINRT
                if (generic.IsGenericType)
#else
							if (generic.GetTypeInfo().IsGenericType)
#endif
                {
#if !WINRT
                    var args = generic.GetGenericArguments();
#else
									var args = generic.GetTypeInfo().GenericTypeArguments;
#endif
                    if (args.Length == 1)
                    {
                        type = args[0];
                    }
                }
            }

            return type;
        }
    }
}