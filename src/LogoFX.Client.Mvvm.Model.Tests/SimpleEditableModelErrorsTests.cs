using FluentAssertions;
using Xunit;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    public class SimpleEditableModelErrorsTests
    {
        [Fact]        
        public void GetErrors_PropertyDoesnHaveValidationInfo_ErrorsAreEmpty()
        {
            var model = new SimpleEditableModel("Test", 21);

            var errors = model.GetErrors("Age");

            errors.Should().BeEmpty();
        }
    }
}
