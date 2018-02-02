using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Solid.Patterns.Memento;

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

                    if (value is IList list && !list.IsReadOnly && !list.IsFixedSize)
                    {
                        return new EnumerableSnapshotValue(list, hashTable);
                    }

                    return new ComplexSnapshotValue(value, hashTable);
                }

                protected abstract object GetValueOverride();

                public object GetValue()
                {
                    return GetValueOverride();
                }
            }

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

                    var storableFields = TypeInformationProvider.GetStorableFields(model.GetType());
                    foreach (var fieldInfo in storableFields)
                    {
                        var value = fieldInfo.GetValue(model);
                        var snapshot = Create(value, hashTable);
                        
                        if (snapshot != null)
                        {
                            _memberSnapshots.Add(fieldInfo, snapshot);
                        }
                    }
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
