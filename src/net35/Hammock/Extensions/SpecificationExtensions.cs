using System;
using Hammock.Specifications;

#if WINRT
using System.Reflection;
#endif

namespace Hammock.Extensions
{
    internal static class SpecificationExtensions
    {
        public static bool Satisfies<T>(this object instance) where T : ISpecification
        {
            var marker = Activator.CreateInstance<T>();
            var type = typeof (ISpecification<>).MakeGenericType(instance.GetType());
            var match = marker.Implements(type);

            if (!match)
            {
                return false;
            }

#if !WINRT
            var method = type.GetMethod("IsSatisfiedBy");
#else
						var method = type.GetTypeInfo().GetDeclaredMethod("IsSatisfiedBy");
#endif
            var result = method.Invoke(marker, new[] {instance});

            return (bool) result;
        }

        public static bool Satisfies(this object instance, ISpecification specificationType)
        {
            var type = typeof (ISpecification<>).MakeGenericType(instance.GetType());
            var match = specificationType.Implements(type);

            if (!match)
            {
                return false;
            }

#if !WINRT
            var method = type.GetMethod("IsSatisfiedBy");
#else
						var method = type.GetTypeInfo().GetDeclaredMethod("IsSatisfiedBy");
#endif
            var result = method.Invoke(specificationType, new[] {instance});

            return (bool) result;
        }
    }
}