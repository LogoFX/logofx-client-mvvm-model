using System.Linq;
using FluentAssertions;
using Xunit;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    public class DeepHierarchyEditableModelSaveChangesTests
    {
        [Fact]
        public void InnerModelInsideCollectionIsRemovedAndSaveChangesIsCalled_ModelIsChangedAndDirtyStatusIsCleared()
        {
            var simpleEditableModel = new SimpleEditableModel();
            var compositeModel = new CompositeEditableModel("location");
            var deepHierarchyModel = new DeepHierarchyEditableModel();
            compositeModel.AddSimpleModelImpl(simpleEditableModel);
            deepHierarchyModel.AddCompositeItemImpl(compositeModel);
            compositeModel.RemoveSimpleItem(simpleEditableModel);
            deepHierarchyModel.ClearDirty(forceClearChildren:true);

            deepHierarchyModel.CanCancelChanges.Should().BeFalse();
            deepHierarchyModel.IsDirty.Should().BeFalse();
            deepHierarchyModel.CompositeModels.Should().BeEquivalentTo(new[] { compositeModel });
            deepHierarchyModel.CompositeModels.First()
                .SimpleCollection.Should()
                .BeEquivalentTo(new ISimpleEditableModel[] { });            
        }
    }
}