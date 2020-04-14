using System;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    [Binding]
    internal sealed class CompositeEditableModelValidationSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public CompositeEditableModelValidationSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [When(@"The composite editable model is created")]
        public void WhenTheCompositeEditableModelIsCreated()
        {
            var compositeModel = new CompositeEditableModel("location");
            var isRaised = false;
            var isRaisedRef= new WeakReference(isRaised);
            compositeModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Error")
                {
                    isRaisedRef.Target = true;
                }
            };
            _scenarioContext.Add("compositeModel", compositeModel);
            _scenarioContext.Add("isRaisedRef", isRaisedRef);
        }

        [When(@"The internal model property is assigned a valid value")]
        public void WhenTheInternalModelPropertyIsAssignedAValidValue()
        {
            var compositeModel = _scenarioContext.Get<CompositeEditableModel>("compositeModel");
            compositeModel.Person.Name = DataGenerator.ValidName;
        }

        [When(@"The internal model property is assigned an invalid value")]
        public void WhenTheInternalModelPropertyIsAssignedAnInvalidValue()
        {
            var compositeModel = _scenarioContext.Get<CompositeEditableModel>("compositeModel");
            compositeModel.Person.Name = DataGenerator.InvalidName;
        }

        [When(@"The internal model is reset")]
        public void WhenTheInternalModelIsReset()
        {
            var compositeModel = _scenarioContext.Get<CompositeEditableModel>("compositeModel");
            compositeModel.Person = new SimpleEditableModel(DataGenerator.ValidName, 0);
        }

        [Then(@"The composite editable model has no errors")]
        public void ThenTheCompositeEditableModelHasNoErrors()
        {
            var compositeModel = _scenarioContext.Get<CompositeEditableModel>("compositeModel");
            AssertHelper.AssertModelHasErrorIsFalse(compositeModel);
        }

        [Then(@"The composite editable model has errors")]
        public void ThenTheCompositeEditableModelHasErrors()
        {
            var compositeModel = _scenarioContext.Get<CompositeEditableModel>("compositeModel");
            AssertHelper.AssertModelHasErrorIsTrue(compositeModel);
        }

        [Then(@"The error notification should be raised")]
        public void ThenTheErrorNotificationShouldBeRaised()
        {
            var isRaisedRef = _scenarioContext.Get<WeakReference>("isRaisedRef");
            ((bool) isRaisedRef.Target).Should().BeTrue();
        }
    }
}
