using System;
using System.Collections.Generic;
using System.Reflection;

namespace LogoFX.Client.Mvvm.Model
{
    static class ReflectionExtensions
    {
        internal static IEnumerable<PropertyInfo> GetDeclaredTypeInfoProperties(this Type type)
        {
            return type
#if NET45
                .GetProperties()
#else
                .GetTypeInfo().DeclaredProperties
#endif
                ;
        }

        internal static IEnumerable<PropertyInfo> GetRuntimeTypeInfoProperties(this Type type
#if NET45
            , BindingFlags flags
#endif
            )
        {
            return type
#if NET45
                .GetProperties(flags)
#else
                .GetRuntimeProperties()
#endif
                ;
        }

        internal static IEnumerable<Type> GetInterfaces(this Type type)
        {
            return type
#if NET45
                .GetInterfaces()
#else
                .GetTypeInfo()
                .ImplementedInterfaces
#endif
                ;
        }
    }
}
