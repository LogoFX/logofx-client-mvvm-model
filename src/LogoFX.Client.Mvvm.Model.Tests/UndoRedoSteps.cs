using FluentAssertions;
using TechTalk.SpecFlow;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    [Binding]
    internal sealed class UndoRedoSteps
    {
        private readonly ModelSteps _modelSteps;

        public UndoRedoSteps(ModelSteps modelSteps)
        {
            _modelSteps = modelSteps;
        }

        [When(@"The simple editable model with undo-redo is created with valid name")]
        public void WhenTheSimpleEditableModelWithUndo_RedoIsCreatedWithValidName()
        {
            _modelSteps.CreateModel(() =>
                new SimpleEditableModelWithUndoRedo(DataGenerator.ValidName, 5));
        }

        [When(@"The name is updated to '(.*)'")]
        public void WhenTheNameIsUpdatedTo(string name)
        {
            var model = _modelSteps.GetModel<SimpleEditableModelWithUndoRedo>();
            model.Name = name;
        }

        [When(@"The last operation for simple editable model with undo-redo is undone")]
        public void WhenTheLastOperationForSimpleEditableModelWithUndo_RedoIsUndone()
        {
            var model = _modelSteps.GetModel<SimpleEditableModelWithUndoRedo>();
            model.Undo();
        }

        [When(@"The last operation for simple editable model with undo-redo is redone")]
        public void WhenTheLastOperationForSimpleEditableModelWithUndo_RedoIsRedone()
        {
            var model = _modelSteps.GetModel<SimpleEditableModelWithUndoRedo>();
            model.Redo();
        }

        [Then(@"The name should be '(.*)'")]
        public void ThenTheNameShouldBe(string expectedName)
        {
            var model = _modelSteps.GetModel<SimpleEditableModelWithUndoRedo>();
            model.Name.Should().Be(expectedName);
        }

        [Then(@"The name should be identical to the valid name")]
        public void ThenTheNameShouldBeIdenticalToTheValidName()
        {
            var model = _modelSteps.GetModel<SimpleEditableModelWithUndoRedo>();
            model.Name.Should().Be(DataGenerator.ValidName);
        }

        [Then(@"The simple editable model with undo-redo is marked as dirty")]
        public void ThenTheSimpleEditableModelWithUndo_RedoIsMarkedAsDirty()
        {
            var model = _modelSteps.GetModel<SimpleEditableModelWithUndoRedo>();
            model.IsDirty.Should().BeTrue();
        }

        [Then(@"The simple editable model with undo-redo is not marked as dirty")]
        public void ThenTheSimpleEditableModelWithUndo_RedoIsNotMarkedAsDirty()
        {
            var model = _modelSteps.GetModel<SimpleEditableModelWithUndoRedo>();
            model.IsDirty.Should().BeFalse();
        }
    }
}