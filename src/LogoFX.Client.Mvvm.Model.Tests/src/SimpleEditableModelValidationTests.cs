using Xunit;

namespace LogoFX.Client.Mvvm.Model.Tests
{    
    public class SimpleEditableModelValidationTests
    {
        [Fact]
        public void SimpleEditableModelIsValid_ErrorIsNull()
        {
            var model = new SimpleEditableModel(DataGenerator.ValidName, 5);

            AssertHelper.AssertModelHasErrorIsFalse(model);
        }

        [Fact]
        public void SimpleEditableModelIsInvalid_ErrorIsNotNull()
        {            
            var model = new SimpleEditableModel(DataGenerator.InvalidName, 5);

            AssertHelper.AssertModelHasErrorIsTrue(model);
        }

        [Fact]
        public void SimpleEditableModelIsValidExternalErrorIsSet_ErrorIsNotNull()
        {
            var model = new SimpleEditableModel(DataGenerator.ValidName, 5);
            model.SetError("external error", "Name");

            AssertHelper.AssertModelHasErrorIsTrue(model);   
        }

        [Fact]
        public void SimpleEditableModelIsValidExternalErrorIsSetAndErrorIsRemoved_ErrorIsNull()
        {
            var model = new SimpleEditableModel(DataGenerator.ValidName, 5);
            model.SetError("external error", "Name");
            model.ClearError("Name");

            AssertHelper.AssertModelHasErrorIsFalse(model);
        }        

        [Fact]
        public void SimpleEditableModelIsValidAndModelBecomesInvalid_ErrorIsNotNull()
        {
            var model = new SimpleEditableModel(DataGenerator.ValidName, 5);
            model.Name = DataGenerator.InvalidName;

            AssertHelper.AssertModelHasErrorIsTrue(model);
        }
    }
}
