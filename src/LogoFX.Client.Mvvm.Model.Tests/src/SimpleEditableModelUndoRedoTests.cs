using FluentAssertions;
using Xunit;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    public class SimpleEditableModelUndoRedoTests
    {
        [Fact]
        public void SimpleModelIsChangedTwiceThenUndoIsCalledOnce_ChangedPropertyValueIsCorrectAndIsDirtyIsTrue()
        {
            var nameOne = "NameOne";
            var nameTwo = "NameTwo";
            var model = new SimpleEditableModelWithUndoRedo(DataGenerator.ValidName, 5);

            model.Name = nameOne;
            model.Name = nameTwo;
            model.Undo();

            model.Name.Should().Be(nameOne);
            model.IsDirty.Should().BeTrue();            
        }

        [Fact]
        public void SimpleModelIsChangedTwiceThenUndoIsCalledTwice_ChangedPropertyValueIsCorrectAndIsDirtyIsFalse()
        {
            var nameOne = "NameOne";
            var nameTwo = "NameTwo";
            var initialName = DataGenerator.ValidName;
            var model = new SimpleEditableModelWithUndoRedo(initialName, 5);

            model.Name = nameOne;
            model.Name = nameTwo;
            model.Undo();
            model.Undo();

            model.Name.Should().Be(initialName);
            model.IsDirty.Should().BeFalse();            
        }

        [Fact]
        public void SimpleModelIsChangedTwiceThenUndoIsCalledOnceThenRedoIsCalledOnce_ChangedPropertyValueIsCorrectAndIsDirtyIsTrue()
        {
            var nameOne = "NameOne";
            var nameTwo = "NameTwo";
            var model = new SimpleEditableModelWithUndoRedo(DataGenerator.ValidName, 5);

            model.Name = nameOne;
            model.Name = nameTwo;
            model.Undo();
            model.Redo();

            model.Name.Should().Be(nameTwo);
            model.IsDirty.Should().BeTrue();            
        }        
    }
}