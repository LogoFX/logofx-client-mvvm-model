using FluentAssertions;
using Xunit;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    public class SimpleModelValidationTests
    {
        [Fact]
        public void SimpleModelIsValid_ErrorIsNull()
        {
            var model = new SimpleModel(DataGenerator.ValidName, 5);

            AssertHelper.AssertModelHasErrorIsFalse(model);
        }

        [Fact]
        public void SimpleModelIsInvalid_ErrorIsNotNull()
        {
            var model = new SimpleEditableModel(DataGenerator.InvalidName, 5);

            AssertHelper.AssertModelHasErrorIsTrue(model);
        }

        [Fact]
        public void SimpleModelIsValidExternalErrorIsSet_ErrorIsNotNull()
        {
            var model = new SimpleEditableModel(DataGenerator.ValidName, 5);
            model.SetError("external error", "Name");

            AssertHelper.AssertModelHasErrorIsTrue(model);
        }

        [Fact]
        public void SimpleModelIsValidExternalErrorIsSetAndErrorIsRemoved_ErrorIsNull()
        {
            var model = new SimpleEditableModel(DataGenerator.ValidName, 5);
            model.SetError("external error", "Name");
            model.ClearError("Name");

            AssertHelper.AssertModelHasErrorIsFalse(model);
        }

        [Fact]
        public void SimpleModelIsValidAndModelBecomesInvalid_ErrorIsNotNull()
        {
            var model = new SimpleEditableModel(DataGenerator.ValidName, 5);
            model.Name = DataGenerator.InvalidName;

            AssertHelper.AssertModelHasErrorIsTrue(model);
            model.Error.Should().Be("Name is invalid");
        }
    }
}