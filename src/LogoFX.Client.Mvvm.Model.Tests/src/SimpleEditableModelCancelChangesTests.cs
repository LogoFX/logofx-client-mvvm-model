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

    public class SimpleEditableModelSetPropertyTests
    {
        [Fact]
        public void SimpleModelIsChangedViaSetProperty_ValueisChangeAndPropertyNotificationIsFired()
        {
            var newAge = 24;
            var model = new SimpleEditableModelWithUndoRedo(DataGenerator.ValidName, 5);
            var isRaised = false;
            model.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Age")
                {
                    isRaised = true;
                }
            };

            model.Age = newAge;            

            model.Age.Should().Be(newAge);
            isRaised.Should().BeTrue();
        }
    }
}