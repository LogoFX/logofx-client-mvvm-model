using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LogoFX.Client.Mvvm.Model
{
    partial class TypeInformationProvider
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> InnerDictionary = new ConcurrentDictionary<Type, PropertyInfo[]>();
        private static readonly ConcurrentDictionary<Type, bool> BclTypeDictionary = new ConcurrentDictionary<Type, bool>();

        /// <summary>
        /// Retrieves collection of storable properties for the given type
        /// </summary>
        /// <param name="type">Type of property container</param>
        /// <returns>Collection of storable properties</returns>
        internal static PropertyInfo[] GetStorableProperties(Type type)
        {
            InnerDictionary.TryAdd(type, GetStorablePropertiesImpl(type));
            return InnerDictionary[type];
        }

        private static PropertyInfo[] GetStorablePropertiesImpl(Type type)
        {
            var storableProperties = new HashSet<PropertyInfo>();
            var modelProperties = GetStorableCandidates(type).ToArray();
            foreach (var modelProperty in modelProperties)
            {
                storableProperties.Add(modelProperty);
            }
            var declaredInterfaces = type.GetInterfaces();
            var explicitProperties = declaredInterfaces.SelectMany(GetStorableCandidates);
            foreach (var explicitProperty in explicitProperties)
            {
                storableProperties.Add(explicitProperty);
            }
            return storableProperties.ToArray();
        }

#if NETSTANDARD2_0                
        internal static FieldInfo GetPrivateField(Type type, string fieldName)
        {
            var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return field;
        }
#endif

        private static IEnumerable<PropertyInfo> GetStorableCandidates(Type modelType)
        {
            var result = modelType.GetRuntimeTypeInfoProperties(
#if NETSTANDARD2_0
                          BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy
#endif
            );

            return result;
        }

        private static Dictionary<string, string> GetAssemblyInfo(Assembly assembly)
        {
            var result = new Dictionary<string, string>();

            var fullName = assembly.FullName;
            var pairs = fullName.Split(',');
            foreach (var pair in pairs)
            {
                var index = pair.IndexOf('=');
                
                string key;
                string value;
                
                if (index < 0)
                {
                    key = pair.Trim();
                    value = null;
                }
                else
                {
                    key = pair.Substring(0, index).Trim();
                    value = pair.Substring(index + 1).Trim();
                }

                result.Add(key, value);
            }

            return result;
        }

        private static string GetPublickKeyToken(Assembly assembly)
        {
            var info = GetAssemblyInfo(assembly);
            const string publicKeyTokenKey = "PublicKeyToken";
            info.TryGetValue(publicKeyTokenKey, out var result);
            return result;
        }

        private static bool IsBclTypeImpl(Type type)
        {
            const string bclToken1 = "b77a5c561934e089";
            const string bclToken2 = "b03f5f7f11d50a3a";

            var publicKeyToken = GetPublickKeyToken(type.GetTypeInfo().Assembly);

            return publicKeyToken != null &&
                   string.Compare(publicKeyToken, bclToken1, StringComparison.OrdinalIgnoreCase) == 0 ||
                   string.Compare(publicKeyToken, bclToken2, StringComparison.OrdinalIgnoreCase) == 0;
        }

        internal static bool IsBclType(this Type type)
        {
            BclTypeDictionary.TryAdd(type, IsBclTypeImpl(type));
            return BclTypeDictionary[type];
        }
    }
}
