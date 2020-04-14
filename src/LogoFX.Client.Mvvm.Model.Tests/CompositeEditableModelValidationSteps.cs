using TechTalk.SpecFlow;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    [Binding]
    internal sealed class CompositeEditableModelValidationSteps
    {
        private readonly ModelSteps _modelSteps;
        private readonly ValidationSteps _validationSteps;

        public CompositeEditableModelValidationSteps(
            ModelSteps modelSteps,
            ValidationSteps validationSteps)
        {
            _modelSteps = modelSteps;
            _validationSteps = validationSteps;
        }

        [When(@"The composite editable model is created")]
        public void WhenTheCompositeEditableModelIsCreated()
        {
            _modelSteps.CreateModel(() => 
                new CompositeEditableModel("location"));
        }

        [When(@"The internal model property is assigned a valid value")]
        public void WhenTheInternalModelPropertyIsAssignedAValidValue()
        {
            var compositeModel = _modelSteps.GetModel<CompositeEditableModel>();
            compositeModel.Person.Name = DataGenerator.ValidName;
        }

        [When(@"The internal model property is assigned an invalid value")]
        public void WhenTheInternalModelPropertyIsAssignedAnInvalidValue()
        {
            var compositeModel = _modelSteps.GetModel<CompositeEditableModel>();
            compositeModel.Person.Name = DataGenerator.InvalidName;
        }

        [When(@"The internal model is reset")]
        public void WhenTheInternalModelIsReset()
        {
            var compositeModel = _modelSteps.GetModel<CompositeEditableModel>();
            compositeModel.Person = new SimpleEditableModel(DataGenerator.ValidName, 0);
        }

        [Then(@"The composite editable model has no errors")]
        public void ThenTheCompositeEditableModelHasNoErrors()
        {
            _validationSteps.AssertModelHasNoError(_modelSteps.GetModel<CompositeEditableModel>);
        }

        [Then(@"The composite editable model has errors")]
        public void ThenTheCompositeEditableModelHasErrors()
        {
            _validationSteps.AssertModelHasError(_modelSteps.GetModel<CompositeEditableModel>);
        }

        [Then(@"The error notification should be raised")]
        public void ThenTheErrorNotificationShouldBeRaised()
        {
            _modelSteps.AssertNotificationIsRaised(NotificationKind.Error);
        }
    }
}
