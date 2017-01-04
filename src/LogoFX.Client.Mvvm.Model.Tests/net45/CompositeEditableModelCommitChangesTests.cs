using System.Linq;
using NUnit.Framework;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    [TestFixture]
    class CompositeEditableModelCommitChangesTests
    {
        [Test]
        public void InnerModelAddedThenCommitChangesIsCalledT_ChangedsAreCommittedAndIsDirtyIsFalse()
        {
            var initialPhones = new[] { 546, 432 };
            var compositeModel = new CompositeEditableModel("Here", initialPhones);

            compositeModel.AddPhone(647);
            compositeModel.CommitChanges();
            
            var phones = ((ICompositeEditableModel)compositeModel).Phones.ToArray();
            var expectedPhones = new[] { 546, 432, 647 };
            CollectionAssert.AreEqual(expectedPhones, phones);
            var isCompositeDirty = compositeModel.IsDirty;
            Assert.IsFalse(isCompositeDirty);
        }
    }
}