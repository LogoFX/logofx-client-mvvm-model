using System.Linq;
using FluentAssertions;
using Xunit;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    
    public class CompositeEditableModelCommitChangesTests
    {
        [Fact]
        public void InnerModelAddedThenCommitChangesIsCalled_ChangesAreCommittedAndIsDirtyIsFalse()
        {
            var initialPhones = new[] { 546, 432 };
            var compositeModel = new CompositeEditableModel("Here", initialPhones);

            compositeModel.AddPhone(647);
            compositeModel.CommitChanges();
            
            var phones = ((ICompositeEditableModel)compositeModel).Phones.ToArray();
            var expectedPhones = new[] { 546, 432, 647 };
            phones.Should().BeEquivalentTo(expectedPhones);
            var isCompositeDirty = compositeModel.IsDirty;
            isCompositeDirty.Should().BeFalse();
        }
    }
}