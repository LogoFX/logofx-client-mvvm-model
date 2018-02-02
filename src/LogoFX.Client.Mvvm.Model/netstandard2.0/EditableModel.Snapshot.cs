using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LogoFX.Client.Mvvm.Model.Contracts;
using Solid.Patterns.Memento;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LogoFX.Client.Mvvm.Model
{
    public partial class EditableModel<T>
    {
        interface ISnapshot
        {
            void Restore(EditableModel<T> model);
        }

        sealed class ComplexSnapshot : ISnapshot
        {
            #region Nested Types

            private abstract class SnapshotValue
            {
                // ReSharper disable once InconsistentNaming
                private static readonly NullSnapshotValue _nullSnapshotValue = 
                    new NullSnapshotValue();

                public static SnapshotValue Create(object value)
                {
                    var hashTable = new Dictionary<object, SnapshotValue>();
                    return Create(value, hashTable);
                }

                protected static SnapshotValue Create(object value, IDictionary<object, SnapshotValue> hashTable)
                {
                    if (value == null)
                    {
                        return _nullSnapshotValue;
                    }

                    if (value is ValueType || value is string)
                    {
                        hashTable[value] = new SimpleSnapshotValue(value);
                    }

                    if (hashTable.TryGetValue(value, out var result))
                    {
                        return result;
                    }

                    if (value is IList list)
                    {
                        return new EnumerableSnapshotValue(list, hashTable);
                    }

                    var type = value.GetType();
                    if (type.IsBclType())
                    {
                        SnapshotValue bclSnapshotValue;
#if NETSTANDARD2_0
                        if (type.IsSerializable)
                        {
                            bclSnapshotValue = new SerializingSnapshotValue(value);
                        }
                        else
#endif
                        {
                            bclSnapshotValue = new SimpleSnapshotValue(value);
                        }
                        hashTable.Add(value, bclSnapshotValue);
                        return bclSnapshotValue;
                    }

                    return new ComplexSnapshotValue(value, hashTable);
                }

                protected abstract object GetValueOverride();

                public object GetValue()
                {
                    return GetValueOverride();
                }
            }

#if NETSTANDARD2_0
            private class SerializingSnapshotValue : SnapshotValue
            {
                private readonly byte[] _data;

                public SerializingSnapshotValue(object value)
                {
                    var formatter = new BinaryFormatter();
                    using (var stream = new MemoryStream())
                    {
                        formatter.Serialize(stream, value);
                        _data = stream.ToArray();
                    }
                }

                protected override object GetValueOverride()
                {
                    var formatter = new BinaryFormatter();
                    using (var stream = new MemoryStream(_data))
                    {
                        return formatter.Deserialize(stream);
                    }
                }
            }
#endif

            private class SimpleSnapshotValue : SnapshotValue
            {
                private readonly object _boxingValue;

                public SimpleSnapshotValue(object value)
                {
                    _boxingValue = value;
                }

                protected override object GetValueOverride()
                {
                    return _boxingValue;
                }
            }

            private sealed class NullSnapshotValue : SimpleSnapshotValue
            {
                public NullSnapshotValue()
                    : base(null)
                {

                }
            }

            private abstract class ComplexSnapshotValueBase : SnapshotValue
            {
                private readonly bool _isOwnDirty;
                private readonly object _referencedModel;

                protected ComplexSnapshotValueBase(object model)
                {
                    _referencedModel = model;

                    if (model is EditableModel<T> editableModel)
                    {
                        _isOwnDirty = editableModel.OwnDirty;
                    }
                }

                protected abstract void RestorePropertiesOverride(object model);

                protected override object GetValueOverride()
                {
                    RestoreProperties(_referencedModel);
                    return _referencedModel;
                }

                public void RestoreProperties(object model)
                {
                    RestorePropertiesOverride(model);

                    if (model is EditableModel<T> editableModel)
                    {
                        editableModel.OwnDirty = _isOwnDirty;
                    }
                }
            }

            private class ComplexSnapshotValue : ComplexSnapshotValueBase
            {
                private readonly Dictionary<MemberInfo, SnapshotValue> _memberSnapshots = 
                    new Dictionary<MemberInfo, SnapshotValue>();

                public ComplexSnapshotValue(object model, IDictionary<object, SnapshotValue> hashTable)
                    : base(model)
                {
                    hashTable.Add(model, this);

                    var storableProperties = TypeInformationProvider.GetStorableProperties(model.GetType());
                    foreach (var propertyInfo in storableProperties)
                    {
                        MemberInfo memberInfo = propertyInfo;
#if NETSTANDARD2_0
                        if (propertyInfo.IsDefined(typeof(EditablePropertyProxyAttribute), true))
                        {
                            var proxyAttr = propertyInfo.GetCustomAttribute<EditablePropertyProxyAttribute>();
                            var fieldInfo = TypeInformationProvider.GetPrivateField(propertyInfo.DeclaringType, proxyAttr.FieldName);
                            memberInfo = fieldInfo;
                        }
                        else 
#endif
                        if (!CanStore(propertyInfo))
                        {
                            continue;
                        }

                        SnapshotValue snapshot = null;
                        var value = GetValue(memberInfo, model);

                        if (value == null)
                        {
                            snapshot = Create(null, hashTable);
                        }
                        else
                        {
                            if (memberInfo.IsDefined(typeof(EditableListAttribute), true))
                            {
                                if (typeof(IList).GetTypeInfo()
                                    .IsAssignableFrom(value.GetType().GetTypeInfo()))
                                {
                                    snapshot = Create(value, hashTable);
                                }
                            }
                            else
                            {
                                snapshot = Create(value, hashTable);
                            }
                        }

                        if (snapshot != null)
                        {
                            _memberSnapshots.Add(memberInfo, snapshot);
                        }
                    }
                }

                private bool CanStore(PropertyInfo propertyInfo)
                {
                    if (propertyInfo.IsDefined(typeof(EditableListAttribute)))
                    {
                        return true;
                    }

                    if (propertyInfo.DeclaringType.GetTypeInfo().IsInterface)
                    {
                        return false;
                    }

                    return propertyInfo.CanWrite && propertyInfo.CanRead && propertyInfo.SetMethod != null;
                }

                protected override void RestorePropertiesOverride(object model)
                {
                    foreach (var infoPair in _memberSnapshots)
                    {
                        var memberInfo = infoPair.Key;
                        var snapshot = infoPair.Value;

                        if (snapshot is EnumerableSnapshotValue enumerableSnapshot)
                        {
                            var value = GetValue(memberInfo, model);
                            enumerableSnapshot.RestoreProperties(value);
                        }
                        else
                        {
                            SetValue(memberInfo, model, snapshot.GetValue());
                        }
                    }
                }

                private object GetValue(MemberInfo memberInfo, object obj)
                {
                    if (memberInfo is FieldInfo fieldInfo)
                    {
                        return fieldInfo.GetValue(obj);
                    }

                    if (memberInfo is PropertyInfo propertyInfo)
                    {
                        return propertyInfo.GetValue(obj, null);
                    }

                    throw new InvalidOperationException();
                }

                private void SetValue(MemberInfo memberInfo, object obj, object value)
                {
                    if (memberInfo is FieldInfo fieldInfo)
                    {
                        fieldInfo.SetValue(obj, value);
                    }
                    else if (memberInfo is PropertyInfo propertyInfo)
                    {
                        propertyInfo.SetValue(obj, value);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            private class EnumerableSnapshotValue : ComplexSnapshotValueBase
            {
                private readonly List<SnapshotValue> _values = new List<SnapshotValue>();

                public EnumerableSnapshotValue(IList list, IDictionary<object, SnapshotValue> hashTable)
                    : base(list)
                {
                    hashTable.Add(list, this);
                    _values.AddRange(list.OfType<object>().Select(x => Create(x, hashTable)));
                }

                protected override void RestorePropertiesOverride(object model)
                {
                    var list = (IList) model;
                    list.Clear();
                    _values.ForEach(x => list.Add(x.GetValue()));
                }
            }

            #endregion

            #region Fields

            private readonly ComplexSnapshotValue _snapshotValue;
            private readonly bool _isOwnDirty;

            #endregion

            #region Constructors

            public ComplexSnapshot(EditableModel<T> model)
            {
                _snapshotValue = (ComplexSnapshotValue) SnapshotValue.Create(model);
                _isOwnDirty = model.OwnDirty;
            }

            #endregion

            #region ISnapshot

            public void Restore(EditableModel<T> model)
            {
                _snapshotValue.RestoreProperties(model);
                model.OwnDirty = _isOwnDirty;
            }

            #endregion
        }

        sealed class SnapshotMementoAdapter : IMemento<EditableModel<T>>
        {
            private readonly ISnapshot _snapshot;

            internal SnapshotMementoAdapter(EditableModel<T> model)
            {
                _snapshot = new ComplexSnapshot(model);
            }

            public IMemento<EditableModel<T>> Restore(EditableModel<T> target)
            {
                IMemento<EditableModel<T>> inverse = new SnapshotMementoAdapter(target);
                _snapshot.Restore(target);
                return inverse;
            }
        }
    }
}
