using System.Collections.Generic;
using LogoFX.Client.Mvvm.Model.Contracts;

namespace LogoFX.Client.Mvvm.Model.Tests.Objects
{    
    interface ISimpleEditableModel : IEditableModel, ISimpleModel
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
            get => _name;
            set => SetPropertyOptions(ref _name, value);
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
            get => _name;
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
            get => _age;
            set => SetPropertyOptions(ref _age,value);
        }        
    }
}