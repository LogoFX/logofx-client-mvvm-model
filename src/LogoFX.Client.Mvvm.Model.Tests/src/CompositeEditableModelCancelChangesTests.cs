using System.Linq;
using FluentAssertions;
using Xunit;

namespace LogoFX.Client.Mvvm.Model.Tests
{    
    public class CompositeEditableModelCancelChangesTests
    {
        [Fact]
        public void InnerModelIsMadeDirtyThenCancelChangesIsCalled_ModelDataIsRestoredAndIsDirtyIsFalse()
        {
            var simpleEditableModel = new SimpleEditableModel("Old Value", 10);
            var compositeModel = new CompositeEditableModel("location", new[] { simpleEditableModel });
            var deepHierarchyModel = new DeepHierarchyEditableModel(new[] { compositeModel });            
            simpleEditableModel.Name = "New Value";            
            deepHierarchyModel.CancelChanges();

            deepHierarchyModel.IsDirty.Should().BeFalse();
            simpleEditableModel.Name.Should().Be("Old Value");            
        }

        [Fact]
        public void InnerModelAddedThenCancelChangesIsCalled_ModelDataIsRestoredAndIsDirtyIsFalse()
        {
            var expectedPhones = new[] { 546, 432 };
            var compositeModel = new CompositeEditableModel("Here", expectedPhones);
            compositeModel.AddPhone(647);
            compositeModel.CancelChanges();

            var phones = ((ICompositeEditableModel)compositeModel).Phones.ToArray();
            phones.Should().BeEquivalentTo(expectedPhones);            
            var isCompositeDirty = compositeModel.IsDirty;
            isCompositeDirty.Should().BeFalse();            
        }

        [Fact]
        public void InnerModelAddedThenCommitChangesIsCalledThenInnerModelAddedThenCancelChangesIsCalled_ModelDataIsRestoredAndIsDirtyIsFalse()
        {
            var initialPhones = new[] { 546, 432 };
            var compositeModel = new CompositeEditableModel("Here", initialPhones);

            compositeModel.AddPhone(647);
            compositeModel.CommitChanges();
            compositeModel.AddPhone(555);
            compositeModel.CancelChanges();

            var phones = ((ICompositeEditableModel)compositeModel).Phones.ToArray();
            var expectedPhones = new[] {546, 432, 647};
            phones.Should().BeEquivalentTo(expectedPhones);            
            var isCompositeDirty = compositeModel.IsDirty;
            isCompositeDirty.Should().BeFalse();
        }

        [Fact]
        public void InnerModelInsideCollectionIsRemoved_CanCancelChangesIsTrue()
        {
            var simpleEditableModel = new SimpleEditableModel();
            var compositeModel = new CompositeEditableModel("location", new[] { simpleEditableModel });
            var deepHierarchyModel = new DeepHierarchyEditableModel(new[] {compositeModel});
            compositeModel.RemoveSimpleItem(simpleEditableModel);

            deepHierarchyModel.CanCancelChanges.Should().BeTrue();            
        }        
    }
}