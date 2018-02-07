using System.ComponentModel.DataAnnotations;
using Xunit;

// ReSharper disable once CheckNamespace
namespace LogoFX.Client.Mvvm.Model.Tests
{
    public class EditableModelWithValidationCancelChangesTests
    {
        [Fact]
        public void EditableModelPropertyIsValid_ErrorIsNull()
        {
            var model = new EditableModelWithValidation("Title1", 21);

            AssertHelper.AssertModelHasErrorIsFalse(model);   
        }

        [Fact]
        public void EditableModelPropertyIsInvalid_ErrorIsNotNull()
        {
            var model = new EditableModelWithValidation("Title$1", 21);

            AssertHelper.AssertModelHasErrorIsTrue(model);   
        }

        [Fact]
        public void EditableModelPropertySetInvalidValue_ErrorIsNotNull()
        {
            var model = new EditableModelWithValidation("Title1", 21);
            model.Title = "T$itle";

            AssertHelper.AssertModelHasErrorIsTrue(model);   
        }

        [Fact]
        public void EditableModelPropertySetInvalidValueThenCancelChanges_ErrorIsNull()
        {
            var model = new EditableModelWithValidation("Title1", 21);
            model.Title = "T$itle";
            model.CancelChanges();

            AssertHelper.AssertModelHasErrorIsFalse(model);   
        }

        [Fact]
        public void EditableModelIsValidExternalErrorIsSetAndChangesIsCanceled_ErrorIsNull()
        {
            var model = new EditableModelWithValidation("", 0);
            model.Title = "Mr.";
            model.Age = 21;

            model.SetError("external error", "Title");
            model.CancelChanges();

            AssertHelper.AssertModelHasErrorIsFalse(model);   
        }

        [Fact]
        public void EditableModelIsInvalidExternalErrorIsSetAndChangesIsCanceled_ErrorIsNotNull()
        {
            var model = new EditableModelWithValidation("", 0);
            model.SetError("external error", "Title");
            model.Title = "Mr.";
            model.Age = 21;
            model.ClearError("Title");
            model.CancelChanges();

            AssertHelper.AssertModelHasErrorIsTrue(model);   
        }
    }

    class EditableModelWithValidation : EditableModel
    {
        private string _title;

        public EditableModelWithValidation(string title, int age)
        {
            _title = title;
            Age = age;
        }

        [TitleValidationAttribute]
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