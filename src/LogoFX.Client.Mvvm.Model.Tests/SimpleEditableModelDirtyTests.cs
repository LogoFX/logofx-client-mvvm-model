using FluentAssertions;
using Xunit;

namespace LogoFX.Client.Mvvm.Model.Tests
{    
    public class SimpleEditableModelDirtyTests
    {
        [Fact]
        public void SimpleModelIsNotMadeDirty_IsDirtyIsFalse()
        {
            var model = new SimpleEditableModel(DataGenerator.ValidName, 5);

            model.IsDirty.Should().BeFalse();            
        }

        [Fact]
        public void SimpleModelIsMadeDirty_IsDirtyIsTrue()
        {
            var model = new SimpleEditableModel(DataGenerator.InvalidName, 5);
            model.MakeDirty();

            model.IsDirty.Should().BeTrue();            
        }
    }
}