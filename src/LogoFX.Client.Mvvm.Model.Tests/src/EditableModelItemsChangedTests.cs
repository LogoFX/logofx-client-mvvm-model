using System.Linq;
using FluentAssertions;
using Xunit;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    public class EditableModelItemsChangedTests
    {
        [Fact]
        public void InnerModelInsideCollectionIsRemovedAndCancelChangesIsCalled_ModelIsRestored()
        {
            var simpleEditableModelOne = new SimpleEditableModel();
            var simpleEditableModelTwo = new SimpleEditableModel();
            var compositeModelOne = new CompositeEditableModel("location");
            var compositeModelTwo = new CompositeEditableModel("location");
            var deepHierarchyModel = new DeepHierarchyEditableModel();
            compositeModelOne.AddSimpleModelImpl(simpleEditableModelOne);
            compositeModelOne.AddSimpleModelImpl(simpleEditableModelTwo);
            deepHierarchyModel.AddCompositeItemImpl(compositeModelOne);
            deepHierarchyModel.AddCompositeItemImpl(compositeModelTwo);

            //deepHierarchyModel.RemoveCompositeModel(compositeModelOne);
            //deepHierarchyModel.CancelChanges();
            //deepHierarchyModel.CanCancelChanges.Should().BeFalse();

            ((CompositeEditableModel)deepHierarchyModel.CompositeModels.First()).RemoveSimpleItem(simpleEditableModelOne);
            deepHierarchyModel.CancelChanges();

            deepHierarchyModel.CanCancelChanges.Should().BeFalse();
            deepHierarchyModel.IsDirty.Should().BeFalse();
            deepHierarchyModel.CompositeModels.Should().BeEquivalentTo(new[] { compositeModelOne, compositeModelTwo });
            deepHierarchyModel.CompositeModels.First()
                .SimpleCollection.Should()
                .BeEquivalentTo(new[] { simpleEditableModelOne, simpleEditableModelTwo });            
        }

    }
}