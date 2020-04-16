using FluentAssertions;
using LogoFX.Client.Mvvm.Model.Tests.Helpers;
using LogoFX.Client.Mvvm.Model.Tests.Objects;
using TechTalk.SpecFlow;

namespace LogoFX.Client.Mvvm.Model.Tests.Steps
{
    [Binding]
    internal sealed class DeepHierarchyEditableModelSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly ModelSteps _modelSteps;

        public DeepHierarchyEditableModelSteps(
            ScenarioContext scenarioContext, 
            ModelSteps modelSteps)
        {
            _scenarioContext = scenarioContext;
            _modelSteps = modelSteps;
        }

        [When(@"The deep hierarchy model is created")]
        public void WhenTheDeepHierarchyModelIsCreated()
        {
            _modelSteps.CreateModel(() => new DeepHierarchyEditableModel());
        }

        [When(@"The deep hierarchy model with all generations is created")]
        public void WhenTheDeepHierarchyModelWithAllGenerationsIsCreated()
        {
            var grandchild = new SimpleEditableModel(DataGenerator.ValidName, 10);
            var child = new CompositeEditableModel("location", new[] {grandchild});
            _scenarioContext.Add("child", child);
            _scenarioContext.Add("grandchild", grandchild);
            _modelSteps.CreateModel(() => { return new DeepHierarchyEditableModel(new[] {child}); });
        }

        [When(@"The child model is created")]
        public void WhenTheChildModelIsCreated()
        {
            var child = new CompositeEditableModel("location");
            _scenarioContext.Add("child", child);
        }

        [When(@"The first child model is created")]
        public void WhenTheFirstChildModelIsCreated()
        {
            var child = new CompositeEditableModel("location");
            _scenarioContext.Add("child-first", child);
        }

        [When(@"The second child model is created")]
        public void WhenTheSecondChildModelIsCreated()
        {
            var child = new CompositeEditableModel("location");
            _scenarioContext.Add("child-second", child);
        }

        [When(@"The grandchild model is created")]
        public void WhenTheGrandchildModelIsCreated()
        {
            var grandchild = new SimpleEditableModel(DataGenerator.ValidName, 10);
            _scenarioContext.Add("grandchild", grandchild);
        }

        [When(@"The first grandchild model is created")]
        public void WhenTheFirstGrandchildModelIsCreated()
        {
            var child = new SimpleEditableModel();
            _scenarioContext.Add("grandchild-first", child);
        }

        [When(@"The second grandchild model is created")]
        public void WhenTheSecondGrandchildModelIsCreated()
        {
            var child = new SimpleEditableModel();
            _scenarioContext.Add("grandchild-second", child);
        }

        [When(@"The grandchild name is updated to '(.*)'")]
        public void WhenTheGrandchildNameIsUpdatedTo(string name)
        {
            var grandchild = _scenarioContext.Get<SimpleEditableModel>("grandchild");
            grandchild.Name = name;
        }

        [When(@"The child is added to the deep hierarchy model")]
        public void WhenTheChildIsAddedToTheDeepHierarchyModel()
        {
            var model = _modelSteps.GetModel<DeepHierarchyEditableModel>();
            var child = _scenarioContext.Get<CompositeEditableModel>("child");
            model.AddCompositeItemImpl(child);
        }

        [When(@"The first child is added to the deep hierarchy model")]
        public void WhenTheFirstChildIsAddedToTheDeepHierarchyModel()
        {
            var model = _modelSteps.GetModel<DeepHierarchyEditableModel>();
            var child = _scenarioContext.Get<CompositeEditableModel>("child-first");
            model.AddCompositeItemImpl(child);
        }

        [When(@"The second child is added to the deep hierarchy model")]
        public void WhenTheSecondChildIsAddedToTheDeepHierarchyModel()
        {
            var model = _modelSteps.GetModel<DeepHierarchyEditableModel>();
            var child = _scenarioContext.Get<CompositeEditableModel>("child-second");
            model.AddCompositeItemImpl(child);
        }

        [When(@"The grandchild is added to the child")]
        public void WhenTheGrandchildIsAddedToTheChild()
        {
            var child = _scenarioContext.Get<CompositeEditableModel>("child");
            var grandchild = _scenarioContext.Get<SimpleEditableModel>("grandchild");
            child.AddSimpleModelImpl(grandchild);
        }

        [When(@"The first grandchild is added to the first child")]
        public void WhenTheFirstGrandchildIsAddedToTheFirstChild()
        {
            var child = _scenarioContext.Get<CompositeEditableModel>("child-first");
            var grandchild = _scenarioContext.Get<SimpleEditableModel>("grandchild-first");
            child.AddSimpleModelImpl(grandchild);
        }

        [When(@"The second grandchild is added to the first child")]
        public void WhenTheSecondGrandchildIsAddedToTheFirstChild()
        {
            var child = _scenarioContext.Get<CompositeEditableModel>("child-first");
            var grandchild = _scenarioContext.Get<SimpleEditableModel>("grandchild-second");
            child.AddSimpleModelImpl(grandchild);
        }

        [When(@"The first child is removed from the deep hierarchy model")]
        public void WhenTheFirstChildIsRemovedFromTheDeepHierarchyModel()
        {
            var model = _modelSteps.GetModel<DeepHierarchyEditableModel>();
            var child = _scenarioContext.Get<CompositeEditableModel>("child-first");
            model.RemoveCompositeModel(child);
        }

        [When(@"The child is updated with removing the grandchild")]
        public void WhenTheChildIsUpdatedWithRemovingTheGrandchild()
        {
            var child = _scenarioContext.Get<CompositeEditableModel>("child");
            var grandchild = _scenarioContext.Get<SimpleEditableModel>("grandchild");
            child.RemoveSimpleItem(grandchild);
        }

        [When(@"The first child is updated with removing the first grandchild")]
        public void WhenTheFirstChildIsUpdatedWithRemovingTheFirstGrandchild()
        {
            var child = _scenarioContext.Get<CompositeEditableModel>("child-first");
            var grandchild = _scenarioContext.Get<SimpleEditableModel>("grandchild-first");
            child.RemoveSimpleItem(grandchild);
        }


        [When(@"The deep hierarchy model changes are committed")]
        public void WhenTheDeepHierarchyModelChangesAreCommitted()
        {
            var model = _modelSteps.GetModel<DeepHierarchyEditableModel>();
            model.CommitChanges();
        }

        [When(@"The deep hierarchy model changes are cancelled")]
        public void WhenTheDeepHierarchyModelChangesAreCancelled()
        {
            var model = _modelSteps.GetModel<DeepHierarchyEditableModel>();
            model.CancelChanges();
        }

        [Then(@"The deep hierarchy model is not marked as dirty")]
        public void ThenTheDeepHierarchyModelIsNotMarkedAsDirty()
        {
            var model = _modelSteps.GetModel<DeepHierarchyEditableModel>();
            model.IsDirty.Should().BeFalse();
        }

        [Then(@"The grandchild name should be identical to the valid name")]
        public void ThenTheGrandchildNameShouldBeIdenticalToTheValidName()
        {
            var grandchild = _scenarioContext.Get<SimpleEditableModel>("grandchild");
            grandchild.Name.Should().Be(DataGenerator.ValidName);
        }

        [Then(@"The deep hierarchy model changes can be cancelled")]
        public void ThenTheDeepHierarchyModelChangesCanBeCancelled()
        {
            var model = _modelSteps.GetModel<DeepHierarchyEditableModel>();
            model.CanCancelChanges.Should().BeTrue();
        }

        [Then(@"The deep hierarchy model changes can not be cancelled")]
        public void ThenTheDeepHierarchyModelChangesCanNotBeCancelled()
        {
            var model = _modelSteps.GetModel<DeepHierarchyEditableModel>();
            model.CanCancelChanges.Should().BeFalse();
        }

        [Then(@"The deep hierarchy model contains the child")]
        public void ThenTheDeepHierarchyModelContainsTheChild()
        {
            var model = _modelSteps.GetModel<DeepHierarchyEditableModel>();
            var child = _scenarioContext.Get<CompositeEditableModel>("child");
            model.CompositeModels.Should().BeEquivalentTo(new[] {child});
        }

        [Then(@"The deep hierarchy model contains all children")]
        public void ThenTheDeepHierarchyModelContainsAllChildren()
        {
            var model = _modelSteps.GetModel<DeepHierarchyEditableModel>();
            var children = new[]
            {
                _scenarioContext.Get<CompositeEditableModel>("child-first"),
                _scenarioContext.Get<CompositeEditableModel>("child-second")
            };
            model.CompositeModels.Should().BeEquivalentTo(children);
        }

        [Then(@"The child contains the grandchild")]
        public void ThenTheChildContainsTheGrandchild()
        {
            var grandchild = _scenarioContext.Get<SimpleEditableModel>("grandchild");
            var child = _scenarioContext.Get<CompositeEditableModel>("child");
            child.SimpleCollection.Should().BeEquivalentTo(new[] {grandchild});
        }

        [Then(@"The child does not contain the grandchild")]
        public void ThenTheChildDoesNotContainTheGrandchild()
        {
            var grandchild = _scenarioContext.Get<SimpleEditableModel>("grandchild");
            var child = _scenarioContext.Get<CompositeEditableModel>("child");
            child.SimpleCollection.Should().NotContain(grandchild);
        }

        [Then(@"The first child contains all grandchildren")]
        public void ThenTheFirstChildContainsAllGrandchildren()
        {
            var grandchildren = new[]
            {
                _scenarioContext.Get<SimpleEditableModel>("grandchild-first"),
                _scenarioContext.Get<SimpleEditableModel>("grandchild-second")
            };
            var child = _scenarioContext.Get<CompositeEditableModel>("child-first");
            child.SimpleCollection.Should().BeEquivalentTo(grandchildren);
        }
    }
}
