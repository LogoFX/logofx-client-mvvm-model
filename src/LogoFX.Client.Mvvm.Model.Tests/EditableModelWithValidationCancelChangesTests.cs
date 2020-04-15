using Xunit;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    public class EditableModelWithValidationCancelChangesTests
    {
        [Fact]
        public void EditableModelPropertySetInvalidValueThenCancelChanges_ErrorIsNull()
        {
            var model = new EditableModelWithValidation(DataGenerator.ValidTitle, 21);
            model.Title = DataGenerator.InvalidTitle;
            model.CancelChanges();

            AssertHelper.AssertModelHasErrorIsFalse(model);   
        }

        [Fact]
        public void CancelChanges_EditableModelIsValidAndHasExternalError_ErrorIsNull()
        {
            var model = new EditableModelWithValidation(DataGenerator.ValidTitle, 0);
            model.Title = "Mr.";
            model.Age = 21;

            model.SetError("external error", "Title");
            model.CancelChanges();

            AssertHelper.AssertModelHasErrorIsFalse(model);   
        }

        [Fact]
        public void CancelChanges_EditableModelIsInvalidAndHasExternalError_ErrorIsNotNull()
        {
            var model = new EditableModelWithValidation(DataGenerator.ValidTitle, 0);
            model.SetError("external error", "Title");
            model.Title = "Mr.";
            model.Age = 21;
            model.ClearError("Title");
            model.CancelChanges();

            AssertHelper.AssertModelHasErrorIsTrue(model);   
        }
    }
}