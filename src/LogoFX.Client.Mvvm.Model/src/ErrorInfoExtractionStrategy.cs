using System;
using System.Collections.Generic;
using System.Linq;

namespace LogoFX.Client.Mvvm.Model
{
    internal interface IErrorInfoExtractionStrategy
    {
        IEnumerable<string> ExtractChildrenErrors(Type type, object propertyContainer);
        IEnumerable<string> GetPropertyInfoSources(Type type);
        bool IsPropertyErrorInfoSource(Type type, string propertyName);        
        object GetErrorInfoSourceValue<T>(Type type, string propertyName, Model<T> model) where T : IEquatable<T>;
    }

    internal sealed class DataErrorInfoExtractionStrategy : IErrorInfoExtractionStrategy
    {
        public IEnumerable<string> ExtractChildrenErrors(Type type, object propertyContainer)
        {
            return
                TypeInformationProvider.GetDataErrorInfoSourceValuesUnboxed(type, propertyContainer)
                    .Where(t => t != null)
                    .Select(t => t.Error)
                    .ToArray();
        }

        public IEnumerable<string> GetPropertyInfoSources(Type type)
        {
            return TypeInformationProvider.GetDataErrorInfoSources(type);
        }

        public bool IsPropertyErrorInfoSource(Type type, string propertyName)
        {
            return TypeInformationProvider.IsPropertyDataErrorInfoSource(type, propertyName);
        }

        public object GetErrorInfoSourceValue<T>(Type type, string propertyName, Model<T> model) where T : IEquatable<T>
        {
            return TypeInformationProvider.GetDataErrorInfoSourceValue(type, propertyName, model);
        }
    }

    internal sealed class NotifyDataErrorInfoExtractionStrategy : IErrorInfoExtractionStrategy
    {
        public IEnumerable<string> ExtractChildrenErrors(Type type, object propertyContainer)
        {
            return
                TypeInformationProvider.GetNotifyDataErrorInfoSourceValuesUnboxed(type, propertyContainer)
                    .Where(t => t != null)
                    .Select(t => t.GetErrors(null))
                    .SelectMany(t => t.OfType<string>())
                    .ToArray();
        }

        public IEnumerable<string> GetPropertyInfoSources(Type type)
        {
            return TypeInformationProvider.GetNotifyDataErrorInfoSources(type);
        }

        public bool IsPropertyErrorInfoSource(Type type, string propertyName)
        {
            return TypeInformationProvider.IsPropertyNotifyDataErrorInfoSource(type, propertyName);
        }

        public object GetErrorInfoSourceValue<T>(Type type, string propertyName, Model<T> model) where T : IEquatable<T>
        {
            return TypeInformationProvider.GetNotifyDataErrorInfoSourceValue(type, propertyName, model);
        }
    }
}