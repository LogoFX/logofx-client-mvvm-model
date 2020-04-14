using TechTalk.SpecFlow;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    [Binding]
    internal sealed class SimpleEditableModelValidationSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly ValidationSteps _validationSteps;

        public SimpleEditableModelValidationSteps(
            ScenarioContext scenarioContext,
            ValidationSteps validationSteps)
        {
            _scenarioContext = scenarioContext;
            _validationSteps = validationSteps;
        }

        [When(@"The simple editable model is created with valid name")]
        public void WhenTheSimpleEditableModelIsCreatedWithValidName()
        {
            CreateSimpleEditableModel(DataGenerator.ValidName);
        }

        private void CreateSimpleEditableModel(string name)
        {
            _validationSteps.CreateEditableModel(() => 
                new SimpleEditableModel(name, 5));
        }

        [When(@"The simple editable model is created with invalid name")]
        public void WhenTheSimpleEditableModelIsCreatedWithInvalidName()
        {
            CreateSimpleEditableModel(DataGenerator.InvalidName);
        }

        [When(@"The simple editable model is updated with external error")]
        public void WhenTheSimpleEditableModelIsUpdatedWithExternalError()
        {
            var model = _validationSteps.GetModel<SimpleEditableModel>();
            var propertyName = "Name";
            _scenarioContext.Add("propertyName",propertyName);
            model.SetError("external error", "Name");
        }

        [When(@"The simple editable model is cleared from external errors")]
        public void WhenTheSimpleEditableModelIsClearedFromExternalErrors()
        {
            var model = _validationSteps.GetModel<SimpleEditableModel>();
            var propertyName = _scenarioContext.Get<string>("propertyName");
            model.ClearError(propertyName);
        }

        [When(@"The simple editable model is updated with invalid value for property")]
        public void WhenTheSimpleEditableModelIsUpdatedWithInvalidValueForProperty()
        {
            var model = _validationSteps.GetModel<SimpleEditableModel>();
            model.Name = DataGenerator.InvalidName;
        }

        [Then(@"The simple editable model has no errors")]
        public void ThenTheSimpleEditableModelHasNoErrors()
        {
            _validationSteps.AssertModelHasNoError(_validationSteps.GetModel<SimpleEditableModel>);
        }

        [Then(@"The simple editable model has errors")]
        public void ThenTheSimpleEditableModelHasErrors()
        {
            _validationSteps.AssertModelHasError(_validationSteps.GetModel<SimpleEditableModel>);
        }
    }
}
