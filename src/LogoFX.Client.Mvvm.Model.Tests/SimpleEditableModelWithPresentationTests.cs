using FluentAssertions;
using Xunit;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    public class SimpleEditableModelWithPresentationTests
    {
        [Fact]        
        public void SimpleEditableModelIsValidAndPresentationIsOverridden_ExternalErrorIsSet_ErrorIsNotNull()
        {
            var model = new SimpleEditableModelWithPresentation();
            model.SetError("external error", "Name");

            model.Error.Should().Be("overridden presentation");
        }
    }
}