using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using LogoFX.Core;

namespace LogoFX.Client.Mvvm.Model
{
    public partial class EditableModel<T>
    {
        sealed class ComplexSnapshot : ISnapshot
        {
            private abstract class SnapshotValue
            {
                // ReSharper disable once InconsistentNaming
                private static readonly NullSnapshotValue _nullSnapshotValue = 
                    new NullSnapshotValue();

                public static ClassSnapshotValue Create(object model)
                {
                    var hashTable = new Dictionary<object, SnapshotValue>();
                    var result = new ClassSnapshotValue(model, hashTable);
                    hashTable.Clear();
                    return result;
                }

                protected static SnapshotValue Create(object value, IDictionary<object, SnapshotValue> hashTable, bool isInitOnly)
                {
                    if (value == null)
                    {
                        return _nullSnapshotValue;
                    }

                    if (hashTable.TryGetValue(value, out var found))
                    {
                        return found;
                    }

                    if (value is IDictionary dictionary && !dictionary.IsReadOnly && !dictionary.IsFixedSize)
                    {
                        return new DictionarySnapshotValue(dictionary, hashTable);
                    }

                    if (value is IList list && !list.IsReadOnly && !list.IsFixedSize)
                    {
                        return new ListSnapshotValue(list, hashTable);
                    }

                    var type = value.GetType();

                    bool isSimpleType = value is ValueType || value is string || type.IsArray;

                    if (!isSimpleType && type.IsBclType())
                    {
                        if (type.IsSerializable && !isInitOnly)
                        {
                            return new SerializingSnapshotValue(value, hashTable);
                        }

                        isSimpleType = true;
                    }

                    if (isSimpleType)
                    {
                        if (isInitOnly)
                        {
                            return null;
                        }

                        return new SimpleSnapshotValue(value);
                    }

                    return new ClassSnapshotValue(value, hashTable);
                }

                protected abstract void RestorePropertiesOverride(object model, Dictionary<SnapshotValue, object> cache);

                public void RestoreProperties(object model)
                {
                    var cache = new Dictionary<SnapshotValue, object> {{this, model}};
                    RestoreProperties(model, cache);
                    cache.Clear();
                }

                internal void RestoreProperties(object model, Dictionary<SnapshotValue, object> cache)
                {
                    RestorePropertiesOverride(model, cache);
                }
                
                protected abstract object GetValueOverride(Dictionary<SnapshotValue, object> cache);

                internal object GetValue(Dictionary<SnapshotValue, object> cache)
                {
                    if (cache.TryGetValue(this, out var result))
                    {
                        return result;
                    }

                    result = GetValueOverride(cache);
                    
                    return result;
                }
            }

            private class SerializingSnapshotValue : SnapshotValue
            {
                private readonly byte[] _data;

                public SerializingSnapshotValue(object value, IDictionary<object, SnapshotValue> hashTable)
                {
                    hashTable.Add(value, this);

                    var formatter = new BinaryFormatter();
                    using (var stream = new MemoryStream())
                    {
                        formatter.Serialize(stream, value);
                        _data = stream.ToArray();
                    }
                }

                protected override void RestorePropertiesOverride(object model, Dictionary<SnapshotValue, object> cache)
                {
                    throw new NotImplementedException();
                }

                protected override object GetValueOverride(Dictionary<SnapshotValue, object> cache)
                {
                    object result;

                    var formatter = new BinaryFormatter();
                    using (var stream = new MemoryStream(_data))
                    {
                        result = formatter.Deserialize(stream);
                    }

                    cache.Add(this, result);
                    return result;
                }
            }
            
            private class SimpleSnapshotValue : SnapshotValue
            {
                private readonly object _boxingValue;

                public SimpleSnapshotValue(object value)
                {
                    _boxingValue = value;
                }

                protected override void RestorePropertiesOverride(object model, Dictionary<SnapshotValue, object> cache)
                {
                    throw new InvalidOperationException();
                }

                protected override object GetValueOverride(Dictionary<SnapshotValue, object> cache)
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
                    : this(model, hashTable, false)
                {

                }

                protected ClassSnapshotValue(object model, IDictionary<object, SnapshotValue> hashTable, bool isList)
                {
                    if (model is EditableModel<T> editableModel)
                    {
                        _isOwnDirty = editableModel.OwnDirty;
                    }

                    _referencedModel = model;
                    hashTable.Add(model, this);

                    if (isList)
                    {
                        return;
                    }

                    var storableFields = TypeInformationProvider.GetStorableFields(model.GetType());
                    foreach (var fieldInfo in storableFields.Where(x => !x.IsNotSerialized))
                    {
                        var value = fieldInfo.GetValue(model);
                        var snapshot = Create(value, hashTable, fieldInfo.IsInitOnly);
                        
                        if (snapshot != null)
                        {
                            _fieldSnapshots.Add(fieldInfo, snapshot);
                        }
                    }
                }

                protected sealed override object GetValueOverride(Dictionary<SnapshotValue, object> cache)
                {
                    var model = _referencedModel;
                    cache.Add(this, model);
                    RestoreProperties(model, cache);
                    return model;
                }

                protected override void RestorePropertiesOverride(object model, Dictionary<SnapshotValue, object> cache)
                {
                    foreach (var infoPair in _fieldSnapshots)
                    {
                        var memberInfo = infoPair.Key;
                        var snapshot = infoPair.Value;

                        if (memberInfo.IsInitOnly)
                        {
                            var currentModel = memberInfo.GetValue(model);
                            snapshot.RestoreProperties(currentModel, cache);
                        }
                        else
                        {
                            var value = snapshot.GetValue(cache);
                            memberInfo.SetValue(model, value);
                        }
                    }

                    if (model is EditableModel<T> editableModel)
                    {
                        editableModel.OwnDirty = _isOwnDirty;
                    }
                }
            }

            private class DictionarySnapshotValue : ClassSnapshotValue
            {
                private readonly Dictionary<object, SnapshotValue> _values = new Dictionary<object, SnapshotValue>();

                public DictionarySnapshotValue(IDictionary dictionary, IDictionary<object, SnapshotValue> hashTable)
                    : base(dictionary, hashTable, true)
                {
                    foreach (var key in dictionary.Keys)
                    {
                        var value = dictionary[key];
                        _values.Add(key, Create(value, hashTable, false));
                    }
                }

                protected override void RestorePropertiesOverride(object model, Dictionary<SnapshotValue, object> cache)
                {
                    var dictionary = (IDictionary) model;
                    dictionary.Clear();
                    _values.ForEach(x => dictionary.Add(x.Key, x.Value.GetValue(cache)));
                    
                    base.RestorePropertiesOverride(model, cache);
                }
            }

            private class ListSnapshotValue : ClassSnapshotValue
            {
                private readonly List<SnapshotValue> _values = new List<SnapshotValue>();

                public ListSnapshotValue(IList list, IDictionary<object, SnapshotValue> hashTable)
                    : base(list, hashTable, true)
                {
                    _values.AddRange(list.OfType<object>().Select(x => Create(x, hashTable, false)));
                }

                protected override void RestorePropertiesOverride(object model, Dictionary<SnapshotValue, object> cache)
                {
                    var list = (IList) model;
                    list.Clear();
                    _values.ForEach(x => list.Add(x.GetValue(cache)));
                    
                    base.RestorePropertiesOverride(model, cache);
                }
            }

            private readonly ClassSnapshotValue _snapshotValue;
            private readonly bool _isOwnDirty;

            public ComplexSnapshot(EditableModel<T> model)
            {
                _snapshotValue = SnapshotValue.Create(model);
                _isOwnDirty = model.OwnDirty;
            }

            public void Restore(EditableModel<T> model)
            {
                _snapshotValue.RestoreProperties(model);
                model.OwnDirty = _isOwnDirty;
            }
        }

    }
}