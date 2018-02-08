﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LogoFX.Client.Mvvm.Model
{
    partial class TypeInformationProvider
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> InnerDictionary = new ConcurrentDictionary<Type, PropertyInfo[]>();
        private static readonly ConcurrentDictionary<Type, FieldInfo[]> FieldDictionary = new ConcurrentDictionary<Type, FieldInfo[]>();

#if NETSTANDARD2_0
        private static readonly ConcurrentDictionary<Type, bool> BclTypeDictionary = new ConcurrentDictionary<Type, bool>();
#endif

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

        private static IEnumerable<PropertyInfo> GetStorableCandidates(Type modelType)
        {
            var result = modelType.GetRuntimeTypeInfoProperties(
#if NETSTANDARD2_0
                          BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy
#endif
            );

            return result;
        }

#if NETSTANDARD2_0

        private static bool IsEvent(Type type, FieldInfo fieldInfo)
        {
            if (fieldInfo.MemberType == MemberTypes.Event ||
                typeof(Delegate).IsAssignableFrom(fieldInfo.FieldType))
            {
                return true;
            }

            var events = type.GetEvents(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var eventInfo in events)
            {
                if (eventInfo.Name == fieldInfo.Name && eventInfo.EventHandlerType == fieldInfo.FieldType)
                {
                    return true;
                }
            }

            return false;
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

        public static bool IsBclType(this Type type)
        {
            BclTypeDictionary.TryAdd(type, IsBclTypeImpl(type));
            return BclTypeDictionary[type];
        }
        
        private static FieldInfo[] GetStorableFieldsImpl(Type type)
        {
            var result = new List<FieldInfo>();

            var fields = type
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => x.DeclaringType == type && !x.IsNotSerialized && !IsEvent(type, x));

            result.AddRange(fields);

            var baseType = type.BaseType;
            if (baseType != typeof(object))
            {
                result.AddRange(GetStorableFields(baseType));
            }

            return result.ToArray();
        }

        internal static FieldInfo[] GetStorableFields(Type type)
        {
            FieldDictionary.TryAdd(type, GetStorableFieldsImpl(type));
            return FieldDictionary[type];
        }
#endif
    }
}
