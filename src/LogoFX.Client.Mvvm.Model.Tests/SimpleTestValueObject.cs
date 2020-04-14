using System.ComponentModel.DataAnnotations;

namespace LogoFX.Client.Mvvm.Model.Tests
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
            var str = value as string;
            var isValid = str != null && str.Contains("$") == false;
            if (isValid)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Name is invalid");
            }
        }
    }
}
