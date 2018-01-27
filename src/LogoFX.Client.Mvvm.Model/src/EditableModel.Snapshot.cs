using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LogoFX.Client.Mvvm.Model.Contracts;
using LogoFX.Core;
using Solid.Patterns.Memento;

namespace LogoFX.Client.Mvvm.Model
{
    public partial class EditableModel<T>
    {
        interface ISnapshot
        {
            void Restore(EditableModel<T> model);
        }

        sealed class Snapshot : ISnapshot
        {
            private readonly IDictionary<PropertyInfo, object> _state = new Dictionary<PropertyInfo, object>();

            private readonly IDictionary<PropertyInfo, IList<object>> _listsState = new Dictionary<PropertyInfo, IList<object>>();

            private readonly bool _isDirty;

            public Snapshot(EditableModel<T> model)
            {                
                var storableProperties = TypeInformationProvider.GetStorableProperties(model.GetType());                
                foreach (PropertyInfo propertyInfo in storableProperties)
                {
                    if (propertyInfo.IsDefined(typeof(EditableListAttribute), true) &&
                        typeof(IList).GetTypeInfo().IsAssignableFrom(propertyInfo.GetValue(model, null).GetType().GetTypeInfo()))
                    {
                        _listsState.Add(new KeyValuePair<PropertyInfo, IList<object>>(propertyInfo,
                            new List<object>(((IList)propertyInfo.GetValue(model, null)).OfType<object>())));
                    }
                    else if (propertyInfo.CanWrite && propertyInfo.CanRead && propertyInfo.SetMethod != null)
                    {
                        _state.Add(new KeyValuePair<PropertyInfo, object>(propertyInfo,
                                                                          propertyInfo.GetValue(model, null)));
                    }
                }                
                _isDirty = model.IsDirty;
            }

            public void Restore(EditableModel<T> model)
            {
                foreach (KeyValuePair<PropertyInfo, object> result in _state)
                {
                    if (result.Key.GetCustomAttributes(typeof(EditableSingleAttribute), true).Any() && result.Value is ICloneable<object>)
                    {
                        result.Key.SetValue(model, (result.Value as ICloneable<object>).Clone(), null);
                    }
                    else
                    {
                        result.Key.SetValue(model, result.Value, null);
                    }
                }

                foreach (KeyValuePair<PropertyInfo, IList<object>> result in _listsState)
                {
                    IList il = (IList)result.Key.GetValue(model, null);
                    //TODO:optimize this
                    il.Clear();
                    if (((EditableListAttribute)result.Key.GetCustomAttributes(typeof(EditableListAttribute), true).First()).CloneItems)
                        result.Value.ForEach(a => il.Add(a is ICloneable<object> ? ((ICloneable<object>)a).Clone() : a));
                    else
                        result.Value.ForEach(a => il.Add(a));                    
                }
                model.NotifyOfPropertyChange(() => model.Error);
                model.OwnDirty = _isDirty;
            }
        }

        sealed class HierarchicalSnapshot : ISnapshot
        {
            private readonly IDictionary<PropertyInfo, object> _state = new Dictionary<PropertyInfo, object>();
            private readonly IDictionary<PropertyInfo, IList<object>> _listsState = new Dictionary<PropertyInfo, IList<object>>();
            private readonly bool _isOwnDirty;

            public HierarchicalSnapshot(EditableModel<T> model)
            {
                var storableProperties = TypeInformationProvider.GetStorableProperties(model.GetType());
                foreach (PropertyInfo propertyInfo in storableProperties)
                {
                    if (propertyInfo.IsDefined(typeof(EditableListAttribute), true))
                    {
                        var value = propertyInfo.GetValue(model, null);
                        if (typeof(IList).GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()))
                        {
                            if (value is IEnumerable<IEditableModel>)
                            {
                                var unboxedValue = value as IEnumerable<IEditableModel>;
                                var serializedList =
                                    unboxedValue.Select(
                                        t => (t is ICloneable<object>) ? ((ICloneable<object>)t).Clone() : t).ToArray();
                                _listsState.Add(new KeyValuePair<PropertyInfo, IList<object>>(propertyInfo,
                                    serializedList));
                            }
                            else
                            {
                                _listsState.Add(new KeyValuePair<PropertyInfo, IList<object>>(propertyInfo,
                                    new List<object>(((IList)value).OfType<object>())));
                            }
                        }
                    }
                    else if (propertyInfo.CanWrite && propertyInfo.CanRead && propertyInfo.SetMethod != null)
                    {
                        _state.Add(new KeyValuePair<PropertyInfo, object>(propertyInfo,
                                                                          propertyInfo.GetValue(model, null)));
                    }
                }

                _isOwnDirty = model.OwnDirty;
            }

            public void Restore(EditableModel<T> model)
            {
                foreach (KeyValuePair<PropertyInfo, object> result in _state)
                {
                    if (result.Key.GetCustomAttributes(typeof(EditableSingleAttribute), true).Any() && result.Value is ICloneable<object>)
                    {
                        result.Key.SetValue(model, (result.Value as ICloneable<object>).Clone(), null);
                    }
                    else
                    {
                        result.Key.SetValue(model, result.Value, null);
                    }
                }

                foreach (KeyValuePair<PropertyInfo, IList<object>> result in _listsState)
                {
                    IList list = (IList)result.Key.GetValue(model, null);
                    list.Clear();
                    result.Value.ForEach(a => list.Add(a));
                }

                model.OwnDirty = _isOwnDirty;
            }
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
#if DEBUG
                    var modelEquals = ReferenceEquals(model, _referencedModel);
                    Debug.Assert(modelEquals);
#endif
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
                    var storableProperties = TypeInformationProvider.GetStorableProperties(model.GetType());
                    foreach (var propertyInfo in storableProperties)
                    {
                        if (propertyInfo.IsDefined(typeof(NotEditableAttribute), true))
                        {
                            continue;
                        }

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
                        if (memberInfo.IsDefined(typeof(EditableListAttribute), true))
                        {
                            var value = GetValue(memberInfo, model);
                            if (typeof(IList).GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()))
                            {
                                snapshot = Create(value, hashTable);
                            }
                        }
                        else
                        {
                            var value = GetValue(memberInfo, model);
                            snapshot = Create(value, hashTable);
                        }

                        if (snapshot != null)
                        {
                            _memberSnapshots.Add(memberInfo, snapshot);
                        }
                    }
                }

                private bool CanStore(PropertyInfo propertyInfo)
                {
                    return propertyInfo.IsDefined(typeof(EditableListAttribute)) ||
                           propertyInfo.CanWrite && propertyInfo.CanRead && propertyInfo.SetMethod != null;
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
