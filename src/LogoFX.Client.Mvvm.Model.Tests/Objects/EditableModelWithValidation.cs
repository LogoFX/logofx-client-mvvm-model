using System.ComponentModel.DataAnnotations;

namespace LogoFX.Client.Mvvm.Model.Tests.Objects
{
    class EditableModelWithValidation : EditableModel
    {
        private string _title;

        public EditableModelWithValidation(string title, int age)
        {
            _title = title;
            Age = age;
        }

        [TitleValidation]
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title == value)
                {
                    return;
                }

                MakeDirty();
                _title = value;
                NotifyOfPropertyChange();
            }
        }

        public int Age { get; set; }
    }

    class TitleValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var isValid = value is string str && str.Contains("$") == false;
            return isValid ? ValidationResult.Success : new ValidationResult("Name is invalid");
        }
    }
}