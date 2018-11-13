using FluentAssertions;
using Xunit;

namespace LogoFX.Client.Mvvm.Model.Tests
{    
    public class CompositeEditableModelValidationTests
    {
        [Fact]
        public void InnerModelIsValid_ErrorIsNull()
        {
            var compositeModel = new CompositeEditableModel("location");

            compositeModel.Person.Name = DataGenerator.ValidName;

            AssertHelper.AssertModelHasErrorIsFalse(compositeModel);
        }

        [Fact]
        public void InnerModelIsInvalid_ErrorIsNotNull()
        {
            var compositeModel = new CompositeEditableModel("location");

            compositeModel.Person.Name = DataGenerator.InvalidName;

            AssertHelper.AssertModelHasErrorIsTrue(compositeModel);
        }

        [Fact]
        public void InnerModelIsReset_ErrorNotificationIsRaised()
        {
            var compositeModel = new CompositeEditableModel("location");
            var isRaised = false;
            compositeModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Error")
                {
                    isRaised = true;    
                }                
            };

            compositeModel.Person = new SimpleEditableModel(DataGenerator.ValidName, 0);

            isRaised.Should().BeTrue();            
        }

        [Fact(Skip= "This feature isn't supported yet")]        
        public void InnerModelPropertyIsReset_ErrorNotificationIsRaised()
        {
            var compositeModel = new CompositeEditableModel("location");
            var isRaised = false;
            compositeModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Error")
                {
                    isRaised = true;
                }
            };

            compositeModel.Person.Name = DataGenerator.InvalidName;

            isRaised.Should().BeTrue();            
        }
    }
}
