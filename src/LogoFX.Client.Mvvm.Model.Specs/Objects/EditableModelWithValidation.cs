using System.ComponentModel.DataAnnotations;

namespace LogoFX.Client.Mvvm.Model.Specs.Objects
{
    internal sealed class EditableModelWithValidation : EditableModel
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
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public int Age { get; set; }
    }

    internal sealed class TitleValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var isValid = value is string str && str.Contains("$") == false;
            return isValid ? ValidationResult.Success : new ValidationResult("Name is invalid");
        }
    }
}