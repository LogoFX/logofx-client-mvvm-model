using FluentAssertions;
using Xunit;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    public class EditableModelWithReferenceToItselfTests
    {
        [Fact(Skip = "This feature isn't suported yet.")]
        public void EditableModelWithReferenceToItselfValuePropertyChanged_IsDirtyIsTrue()
        {
            var model = new SelfEditableModel();
            model.Self = model;
            model.Value = "new value";

            model.IsDirty.Should().BeTrue();
        }

        [Fact(Skip = "This feature isn't suported yet.")]
        public void EditableModelWithReferenceToItselfValuePropertyChangedThenCancelChangedIsCalled_IsDirtyIsFalseAndValueIsNull()
        {
            var model = new SelfEditableModel();
            model.Self = model;
            model.Value = "new value";
            model.CancelChanges();

            model.IsDirty.Should().BeTrue();
            model.Value.Should().Be(null);
        }
    }

    public class SelfEditableModel : EditableModel
    {
        private string _value;

        public string Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (_value == value)
                {
                    return;
                }

                MakeDirty();
                _value = value;
                NotifyOfPropertyChange();
            }
        }

        public SelfEditableModel Self { get; set; }
    }
}