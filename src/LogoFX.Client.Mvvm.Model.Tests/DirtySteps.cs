using System.Linq;
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

        [When(@"The composite editable model is updated with invalid value for child collection item inner property value")]
        public void WhenTheCompositeEditableModelIsUpdatedWithInvalidValueForChildCollectionItemInnerPropertyValue()
        {
            var child = _scenarioContext.Get<SimpleEditableModel>("child");
            child.Name = DataGenerator.InvalidName;
        }

        [When(@"The composite editable model is updated by removing child item from the collection")]
        public void WhenTheCompositeEditableModelIsUpdatedByRemovingChildItemFromTheCollection()
        {
            var model = _modelSteps.GetModel<CompositeEditableModel>();
            var child = _scenarioContext.Get<SimpleEditableModel>("child");
            model.RemoveSimpleItem(child);
        }

        [When(@"The explicit composite editable model is updated by removing child item from the collection")]
        public void WhenTheExplicitCompositeEditableModelIsUpdatedByRemovingChildItemFromTheCollection()
        {
            var model = _modelSteps.GetModel<ExplicitCompositeEditableModel>();
            var child = _scenarioContext.Get<SimpleEditableModel>("child");
            model.RemoveSimpleItem(child);
        }

        [When(@"The composite editable model is cleared of dirty state along with its children")]
        public void WhenTheCompositeEditableModelIsClearedOfDirtyStateAlongWithItsChildren()
        {
            var model = _modelSteps.GetModel<CompositeEditableModel>();
            model.ClearDirty(forceClearChildren:true);
        }

        [When(@"The composite editable model is cleared of dirty state without its children")]
        public void WhenTheCompositeEditableModelIsClearedOfDirtyStateWithoutItsChildren()
        {
            var model = _modelSteps.GetModel<CompositeEditableModel>();
            model.ClearDirty(forceClearChildren: false);
        }

        [When(@"The explicit composite editable model is cleared of dirty state along with its children")]
        public void WhenTheExplicitCompositeEditableModelIsClearedOfDirtyStateAlongWithItsChildren()
        {
            var model = _modelSteps.GetModel<ExplicitCompositeEditableModel>();
            model.ClearDirty(forceClearChildren: true);
        }

        [When(@"The child item is assigned an invalid property value")]
        public void WhenTheChildItemIsAssignedAnInvalidPropertyValue()
        {
            var child = _scenarioContext.Get<SimpleEditableModel>("child");
            child.Name = DataGenerator.InvalidName;
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

        [Then(@"The explicit composite editable model is marked as dirty")]
        public void ThenTheExplicitCompositeEditableModelIsMarkedAsDirty()
        {
            var model = _modelSteps.GetModel<ExplicitCompositeEditableModel>();
            model.IsDirty.Should().BeTrue();
        }

        [Then(@"The explicit composite editable model is not marked as dirty")]
        public void ThenTheExplicitCompositeEditableModelIsNotMarkedAsDirty()
        {
            var model = _modelSteps.GetModel<ExplicitCompositeEditableModel>();
            model.IsDirty.Should().BeFalse();
        }

        [Then(@"The dirty notification should be raised")]
        public void ThenTheDirtyNotificationShouldBeRaised()
        {
            _modelSteps.AssertNotificationIsRaised(NotificationKind.Dirty);
        }
    }
}
