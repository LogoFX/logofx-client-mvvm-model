using System.ComponentModel.DataAnnotations;

namespace LogoFX.Client.Mvvm.Model.Specs.Objects
{
    class SimpleTestValueObject : ValueObject
    {
        public SimpleTestValueObject(string name, int age)
        {
            Name = name;
            Age = age;
        }

        [NameValidation]
        public string Name { get; set; }
        public int Age { get; set; }
    }

    class NameValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var isValid = value is string str && str.Contains("$") == false;
            return isValid ? ValidationResult.Success : new ValidationResult("Name is invalid");
        }
    }
}
