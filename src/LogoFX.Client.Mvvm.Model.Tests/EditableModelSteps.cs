using TechTalk.SpecFlow;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    [Binding]
    internal sealed class EditableModelSteps
    {
        private readonly ModelSteps _modelSteps;

        public EditableModelSteps(
            ModelSteps modelSteps)
        {
            _modelSteps = modelSteps;
        }

        [When(@"The editable model is created with valid title")]
        public void WhenTheEditableModelIsCreatedWithValidTitle()
        {
            _modelSteps.CreateModel(() =>
                new EditableModelWithValidation(DataGenerator.ValidTitle, 21));
        }

        [When(@"The editable model is created with invalid title")]
        public void WhenTheEditableModelIsCreatedWithInvalidTitle()
        {
            _modelSteps.CreateModel(() =>
                new EditableModelWithValidation(DataGenerator.InvalidTitle, 21));
        }

        [When(@"The editable model is updated with invalid value for property")]
        public void WhenTheEditableModelIsUpdatedWithInvalidValueForProperty()
        {
            var model = _modelSteps.GetModel<EditableModelWithValidation>();
            model.Title = DataGenerator.InvalidTitle;
        }

        [When(@"The editable model is updated with value '(.*)' for property")]
        public void WhenTheEditableModelIsUpdatedWithValueForProperty(string value)
        {
            var model = _modelSteps.GetModel<EditableModelWithValidation>();
            model.Title = value;
        }

        [When(@"The editable model is updated with external error")]
        public void WhenTheEditableModelIsUpdatedWithExternalError()
        {
            var model = _modelSteps.GetModel<EditableModelWithValidation>();
            model.SetError("external error", "Title");
        }

        [When(@"The editable model is cleared from external errors")]
        public void WhenTheEditableModelIsClearedFromExternalErrors()
        {
            var model = _modelSteps.GetModel<EditableModelWithValidation>();
            model.ClearError("Title");
        }

        [Then(@"The editable model has no errors")]
        public void ThenTheEditableModelHasNoErrors()
        {
            var model = _modelSteps.GetModel<EditableModelWithValidation>();
            AssertHelper.AssertModelHasErrorIsFalse(model);
        }

        [Then(@"The editable model has errors")]
        public void ThenTheEditableModelHasErrors()
        {
            var model = _modelSteps.GetModel<EditableModelWithValidation>();
            AssertHelper.AssertModelHasErrorIsTrue(model);
        }
    }
}
