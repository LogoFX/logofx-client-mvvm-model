#if NET45 || NETSTANDARD2_0
using System.ComponentModel;
#endif
using System.Collections.Generic;
using LogoFX.Client.Mvvm.Model.Contracts;

namespace LogoFX.Client.Mvvm.Model.Tests
{    
    interface ISimpleEditableModel : IEditableModel, ISimpleModel
#if NET45 || NETSTANDARD2_0
        , IDataErrorInfo
#endif
    {

    }        

    class SimpleEditableModel : EditableModel, ISimpleEditableModel
    {
        public SimpleEditableModel(string name, int age)
            : this()
        {
            _name = name;
            Age = age;
        }

        public SimpleEditableModel()
        {

        }

        private string _name;
        [NameValidation]
        public new string Name
        {
            get { return _name; }
            set
            {
                MakeDirty();
                _name = value;                
                NotifyOfPropertyChange();                
            }
        }
        public int Age { get; set; }        
    }

    class SimpleEditableModelWithPresentation : SimpleEditableModel
    {        
        protected override string CreateErrorsPresentation(IEnumerable<string> errors)
        {
            return "overridden presentation";
        }
    }
    class SimpleEditableModelWithUndoRedo : EditableModel.WithUndoRedo, ISimpleEditableModel
    {        
        public SimpleEditableModelWithUndoRedo(string name, int age)
            : this()
        {
            _name = name;
            _age = age;
        }

        public SimpleEditableModelWithUndoRedo()
        {            
        }

        private string _name;
        [NameValidation]
        public new string Name
        {
            get { return _name; }
            set
            {
                MakeDirty();
                _name = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => Error);
            }
        }

        private int _age;

        public int Age
        {
            get { return _age; }
            set
            {
                SetProperty(ref _age,value);
            }
        }        
    }
}