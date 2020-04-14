using System;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    [Binding]
    internal sealed class DirtySteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly ModelSteps _modelSteps;

        public DirtySteps(
            ScenarioContext scenarioContext,
            ModelSteps modelSteps)
        {
            _scenarioContext = scenarioContext;
            _modelSteps = modelSteps;
        }

        [When(@"The simple editable model is made dirty")]
        public void WhenTheSimpleEditableModelIsMadeDirty()
        {
            var model = _modelSteps.GetModel<SimpleEditableModel>();
            model.MakeDirty();
        }

        [When(@"The composite editable model is updated with invalid value for inner property value")]
        public void WhenTheCompositeEditableModelIsUpdatedWithInvalidValueForInnerPropertyValue()
        {
            var model = _modelSteps.GetModel<CompositeEditableModel>();
            model.Person.Name = DataGenerator.InvalidName;
        }

        [Then(@"The simple editable model is not marked as dirty")]
        public void ThenTheSimpleEditableModelIsNotMarkedAsDirty()
        {
            var model = _modelSteps.GetModel<SimpleEditableModel>();
            model.IsDirty.Should().BeFalse();
        }

        [Then(@"The simple editable model is marked as dirty")]
        public void ThenTheSimpleEditableModelIsMarkedAsDirty()
        {
            var model = _modelSteps.GetModel<SimpleEditableModel>();
            model.IsDirty.Should().BeTrue();
        }

        [Then(@"The composite editable model is not marked as dirty")]
        public void ThenTheCompositeEditableModelIsNotMarkedAsDirty()
        {
            var model = _modelSteps.GetModel<CompositeEditableModel>();
            model.IsDirty.Should().BeFalse();
        }

        [Then(@"The composite editable model is marked as dirty")]
        public void ThenTheCompositeEditableModelIsMarkedAsDirty()
        {
            var model = _modelSteps.GetModel<CompositeEditableModel>();
            model.IsDirty.Should().BeTrue();
        }

        [Then(@"The dirty notification should be raised")]
        public void ThenTheDirtyNotificationShouldBeRaised()
        {
            var isDirtyRaisedRef = _scenarioContext.Get<WeakReference>("isDirtyRaisedRef");
            ((bool)isDirtyRaisedRef.Target).Should().BeTrue();
        }
    }
}
