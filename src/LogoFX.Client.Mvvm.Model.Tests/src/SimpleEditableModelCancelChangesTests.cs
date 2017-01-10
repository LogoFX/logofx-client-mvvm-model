using FluentAssertions;
using Xunit;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    public class SimpleEditableModelCancelChangesTests
    {
        [Fact]
        public void SimpleModelIsChangedAndChangeIsCancelled_PropertyValueIsReverted()
        {
            var nameOne = "NameOne";            
            var model = new SimpleEditableModelWithUndoRedo(DataGenerator.ValidName, 5);

            model.Name = nameOne;
            model.CancelChanges();

            model.Name.Should().Be(DataGenerator.ValidName);
            model.IsDirty.Should().BeFalse();
        }
    }
}