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

                public static ClassSnapshotValue Create(object model)
                {
                    var hashTable = new Dictionary<object, SnapshotValue>();
                    return new ClassSnapshotValue(model, hashTable);
                }

                protected static SnapshotValue Create(object value, IDictionary<object, SnapshotValue> hashTable)
                {
                    if (value == null)
                    {
                        return _nullSnapshotValue;
                    }

                    if (value is ValueType || value is string || value.GetType().IsArray)
                    {
                        return new SimpleSnapshotValue(value);
                    }

                    if (hashTable.TryGetValue(value, out var found))
                    {
                        return found;
                    }

                    if (value is IList list && !list.IsReadOnly && !list.IsFixedSize)
                    {
                        return new ListSnapshotValue(list, hashTable);
                    }

                    return new ClassSnapshotValue(value, hashTable);
                }

                protected abstract void RestorePropertiesOverride(object model);

                public void RestoreProperties(object model)
                {
                    RestorePropertiesOverride(model);
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

                protected override void RestorePropertiesOverride(object model)
                {
                    throw new InvalidOperationException();
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

            private class ClassSnapshotValue : SnapshotValue
            {
                private readonly bool _isOwnDirty;
                private readonly object _referencedModel;
                private readonly Dictionary<FieldInfo, SnapshotValue> _fieldSnapshots = 
                    new Dictionary<FieldInfo, SnapshotValue>();

                public ClassSnapshotValue(object model, IDictionary<object, SnapshotValue> hashTable)
                {
                    if (model is EditableModel<T> editableModel)
                    {
                        _isOwnDirty = editableModel.OwnDirty;
                    }

                    _referencedModel = model;
                    hashTable.Add(model, this);

                    var storableFields = TypeInformationProvider.GetStorableFields(model.GetType());
                    foreach (var fieldInfo in storableFields.Where(x => !x.IsNotSerialized))
                    {
                        var value = fieldInfo.GetValue(model);
                        var snapshot = Create(value, hashTable);
                        
                        if (snapshot != null)
                        {
                            _fieldSnapshots.Add(fieldInfo, snapshot);
                        }
                    }
                }

                protected sealed override object GetValueOverride()
                {
                    var model = _referencedModel;
                    RestoreProperties(model);
                    return model;
                }

                protected override void RestorePropertiesOverride(object model)
                {
                    foreach (var infoPair in _fieldSnapshots)
                    {
                        var memberInfo = infoPair.Key;
                        var snapshot = infoPair.Value;

                        if (memberInfo.IsInitOnly)
                        {
                            var currentModel = memberInfo.GetValue(model);
                            snapshot.RestoreProperties(currentModel);
                        }
                        else
                        {
                            var value = snapshot.GetValue();
                            memberInfo.SetValue(model, value);
                        }
                    }

                    if (model is EditableModel<T> editableModel)
                    {
                        editableModel.OwnDirty = _isOwnDirty;
                    }
                }
            }

            private class ListSnapshotValue : ClassSnapshotValue
            {
                private readonly List<SnapshotValue> _values = new List<SnapshotValue>();

                public ListSnapshotValue(IList list, IDictionary<object, SnapshotValue> hashTable)
                    : base(list, hashTable)
                {
                    _values.AddRange(list.OfType<object>().Select(x => Create(x, hashTable)));
                }

                protected override void RestorePropertiesOverride(object model)
                {
                    var list = (IList) model;
                    list.Clear();
                    _values.ForEach(x => list.Add(x.GetValue()));
                    base.RestorePropertiesOverride(model);
                }
            }

            #endregion

            #region Fields

            private readonly ClassSnapshotValue _snapshotValue;
            private readonly bool _isOwnDirty;

            #endregion

            #region Constructors

            public ComplexSnapshot(EditableModel<T> model)
            {
                _snapshotValue = SnapshotValue.Create(model);
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
