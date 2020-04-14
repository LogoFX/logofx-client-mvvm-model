using System;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    [Binding]
    internal sealed class CompositeEditableModelValidationSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly ValidationSteps _validationSteps;

        public CompositeEditableModelValidationSteps(
            ScenarioContext scenarioContext,
            ValidationSteps validationSteps)
        {
            _scenarioContext = scenarioContext;
            _validationSteps = validationSteps;
        }

        [When(@"The composite editable model is created")]
        public void WhenTheCompositeEditableModelIsCreated()
        {
            _validationSteps.CreateEditableModel(() => 
                new CompositeEditableModel("location"));
        }

        [When(@"The internal model property is assigned a valid value")]
        public void WhenTheInternalModelPropertyIsAssignedAValidValue()
        {
            var compositeModel = _validationSteps.GetModel<CompositeEditableModel>();
            compositeModel.Person.Name = DataGenerator.ValidName;
        }

        [When(@"The internal model property is assigned an invalid value")]
        public void WhenTheInternalModelPropertyIsAssignedAnInvalidValue()
        {
            var compositeModel = _validationSteps.GetModel<CompositeEditableModel>();
            compositeModel.Person.Name = DataGenerator.InvalidName;
        }

        [When(@"The internal model is reset")]
        public void WhenTheInternalModelIsReset()
        {
            var compositeModel = _validationSteps.GetModel<CompositeEditableModel>();
            compositeModel.Person = new SimpleEditableModel(DataGenerator.ValidName, 0);
        }

        [Then(@"The composite editable model has no errors")]
        public void ThenTheCompositeEditableModelHasNoErrors()
        {
            _validationSteps.AssertModelHasNoError(_validationSteps.GetModel<CompositeEditableModel>);
        }

        [Then(@"The composite editable model has errors")]
        public void ThenTheCompositeEditableModelHasErrors()
        {
            _validationSteps.AssertModelHasError(_validationSteps.GetModel<CompositeEditableModel>);
        }

        [Then(@"The error notification should be raised")]
        public void ThenTheErrorNotificationShouldBeRaised()
        {
            var isRaisedRef = _scenarioContext.Get<WeakReference>("isRaisedRef");
            ((bool) isRaisedRef.Target).Should().BeTrue();
        }
    }
}
