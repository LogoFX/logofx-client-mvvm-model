using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using LogoFX.Client.Mvvm.Model.Contracts;

namespace LogoFX.Client.Mvvm.Model.Tests.Objects
{
    interface ICompositeEditableModel : IEditableModel, IDataErrorInfo
    {
        IEnumerable<int> Phones { get; }

        ISimpleEditableModel Person { get; set; }

        IEnumerable<ISimpleEditableModel> SimpleCollection { get; } 
    }

    class CompositeEditableModel : EditableModel, ICompositeEditableModel, ICloneable<CompositeEditableModel>, IEquatable<CompositeEditableModel>
    {       
        public CompositeEditableModel(string location)
        {                                      
            Location = location;
            _person = new SimpleEditableModel();            
        }

        public CompositeEditableModel(string location, IEnumerable<int> phones)
        {
            Location = location;
            _person = new SimpleEditableModel();            
            Phones.AddRange(phones);            
        }

        public CompositeEditableModel(string location, IEnumerable<SimpleEditableModel> simpleCollection)
        {
            Location = location;
            _person = new SimpleEditableModel();            
            foreach (var simpleEditableModel in simpleCollection)
            {
                SimpleCollectionImpl.Add(simpleEditableModel);
            }            
        }

        public string Location { get; }

        private ISimpleEditableModel _person;
        public ISimpleEditableModel Person
        {
            get => _person;
            set => SetPropertyOptions(ref _person, value);
        }

        private ObservableCollection<SimpleEditableModel> SimpleCollectionImpl { get; } = new ObservableCollection<SimpleEditableModel>();

        public IEnumerable<ISimpleEditableModel> SimpleCollection => SimpleCollectionImpl;

        IEnumerable<int> ICompositeEditableModel.Phones => Phones;

        private List<int> Phones { get; } = new List<int>();

        public void AddPhone(int number)
        {
            MakeDirty();
            Phones.Add(number);
        }

        public void RemoveSimpleItem(SimpleEditableModel item)
        {
            MakeDirty();
            SimpleCollectionImpl.Remove(item);
        }

        public void AddSimpleModelImpl(SimpleEditableModel simpleEditableModel)
        {
            SimpleCollectionImpl.Add(simpleEditableModel);
        }

        public CompositeEditableModel Clone()
        {
            var composite = new CompositeEditableModel(Location, Phones);
            composite.Id = composite.Id;
            foreach (var simpleEditableModel in SimpleCollectionImpl)
            {
                composite.AddSimpleModelImpl(simpleEditableModel);
            }
            return composite;
        }

        public bool Equals(CompositeEditableModel other)
        {
            return other != null && other.Id == Id;
        }
    }

    class ExplicitCompositeEditableModel : EditableModel, ICompositeEditableModel
    {
        public ExplicitCompositeEditableModel(string location)
        {
            Location = location;
            _person = new SimpleEditableModel();
        }

        public ExplicitCompositeEditableModel(string location, IEnumerable<int> phones)
        {
            Location = location;
            _person = new SimpleEditableModel();
            Phones.AddRange(phones);
        }

        public ExplicitCompositeEditableModel(string location, IEnumerable<SimpleEditableModel> simpleCollection)
        {
            Location = location;
            _person = new SimpleEditableModel();
            foreach (var simpleEditableModel in simpleCollection)
            {
                _simpleCollection.Add(simpleEditableModel);
            }
        }

        public string Location { get; }

        private ISimpleEditableModel _person;
        public ISimpleEditableModel Person
        {
            get => _person;
            set => SetPropertyOptions(ref _person, value, new EditableSetPropertyOptions()
            {
                MarkAsDirty = false
            });
        }

        private readonly ObservableCollection<SimpleEditableModel> _simpleCollection = new ObservableCollection<SimpleEditableModel>();

        IEnumerable<ISimpleEditableModel> ICompositeEditableModel.SimpleCollection => _simpleCollection;

        IEnumerable<int> ICompositeEditableModel.Phones => Phones;

        private List<int> Phones { get; } = new List<int>();

        public void AddPhone(int number)
        {
            MakeDirty();
            Phones.Add(number);
        }

        public void RemoveSimpleItem(SimpleEditableModel item)
        {
            MakeDirty();
            _simpleCollection.Remove(item);
        }
    }

    class CompositeEditableModelWithUndoRedo : EditableModel.WithUndoRedo, ICompositeEditableModel, ICloneable<CompositeEditableModelWithUndoRedo>, IEquatable<CompositeEditableModelWithUndoRedo>
    {
        public CompositeEditableModelWithUndoRedo(string location)
        {
            Location = location;
            _person = new SimpleEditableModel();
        }

        public CompositeEditableModelWithUndoRedo(string location, IEnumerable<int> phones)
        {
            Location = location;
            _person = new SimpleEditableModel();
            Phones.AddRange(phones);
        }

        public CompositeEditableModelWithUndoRedo(string location, IEnumerable<SimpleEditableModel> simpleCollection)
        {
            Location = location;
            _person = new SimpleEditableModel();
            foreach (var simpleEditableModel in simpleCollection)
            {
                SimpleCollectionImpl.Add(simpleEditableModel);
            }
        }

        public string Location { get; }

        private ISimpleEditableModel _person;
        public ISimpleEditableModel Person
        {
            get => _person;
            set => SetPropertyOptions(ref _person, value);
        }

        private ObservableCollection<SimpleEditableModel> SimpleCollectionImpl { get; } = new ObservableCollection<SimpleEditableModel>();

        public IEnumerable<ISimpleEditableModel> SimpleCollection => SimpleCollectionImpl;

        IEnumerable<int> ICompositeEditableModel.Phones => Phones;

        private List<int> Phones { get; } = new List<int>();

        public void AddPhone(int number)
        {
            MakeDirty();
            Phones.Add(number);
        }

        public void RemoveSimpleItem(SimpleEditableModel item)
        {
            MakeDirty();
            SimpleCollectionImpl.Remove(item);
        }

        public void AddSimpleModelImpl(SimpleEditableModel simpleEditableModel)
        {
            SimpleCollectionImpl.Add(simpleEditableModel);
        }

        public CompositeEditableModelWithUndoRedo Clone()
        {
            var composite = new CompositeEditableModelWithUndoRedo(Location, Phones);
            composite.Id = composite.Id;
            foreach (var simpleEditableModel in SimpleCollectionImpl)
            {
                composite.AddSimpleModelImpl(simpleEditableModel);
            }
            return composite;
        }

        public bool Equals(CompositeEditableModelWithUndoRedo other)
        {
            return other != null && other.Id == Id;
        }
    }
}