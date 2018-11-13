using System.Linq;
using FluentAssertions;
using Xunit;

namespace LogoFX.Client.Mvvm.Model.Tests
{    
    public class CompositeEditableModelUndoRedoTests
    {
        [Fact]
        public void InnerModelAddedThenUndoIsCalled_ModelDataIsRestoredAndIsDirtyIsFalse()
        {
            var expectedPhones = new[] { 546, 432 };
            var compositeModel = new CompositeEditableModelWithUndoRedo("Here", expectedPhones);

            compositeModel.AddPhone(647);
            compositeModel.Undo();

            var phones = ((ICompositeEditableModel)compositeModel).Phones.ToArray();
            phones.Should().BeEquivalentTo(expectedPhones);
            var isCompositeDirty = compositeModel.IsDirty;
            isCompositeDirty.Should().BeFalse();
        }

        [Fact]
        public void InnerModelPropertyIsChanged_ThenCanUndoIsTrueAndIsDirtyIsTrue()
        {            
            var person = new SimpleEditableModel(DataGenerator.ValidName, 25);
            var name = "NewName";
            var compositeModel = new CompositeEditableModelWithUndoRedo("Here", new [] {person});

            person.Name = name;

            var canUndo = compositeModel.CanUndo;
            canUndo.Should().BeTrue();
            var isCompositeDirty = compositeModel.IsDirty;
            isCompositeDirty.Should().BeTrue();
        }

        [Fact]
        public void InnerModelAddedThenUndoIsCalledThenRedoIsCalled_InnerModelIsKeptAndIsDirtyIsTrue()
        {
            var initialPhones = new[] { 546, 432 };
            var compositeModel = new CompositeEditableModelWithUndoRedo("Here", initialPhones);

            compositeModel.AddPhone(647);
            compositeModel.Undo();
            compositeModel.Redo();

            var expectedPhones = new[] { 546, 432, 647 };
            var phones = ((ICompositeEditableModel)compositeModel).Phones.ToArray();
            phones.Should().BeEquivalentTo(expectedPhones);            
            var isCompositeDirty = compositeModel.IsDirty;
            isCompositeDirty.Should().BeTrue();            
        }
    }
}
