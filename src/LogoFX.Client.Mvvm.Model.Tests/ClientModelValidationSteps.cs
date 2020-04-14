using FluentAssertions;
using TechTalk.SpecFlow;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    [Binding]
    internal sealed class ClientModelValidationSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public ClientModelValidationSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [When(@"The simple test value object is created with name '(.*)'")]
        public void WhenTheSimpleTestValueObjectIsCreatedWithName(string name)
        {
            var valueObject = new SimpleTestValueObject(name, 5);
            _scenarioContext.Add("valueObject", valueObject);
        }

        [Then(@"The simple test value object has no errors")]
        public void ThenTheSimpleTestValueObjectHasNoErrors()
        {
            var valueObject = _scenarioContext.Get<SimpleTestValueObject>("valueObject");
            var error = valueObject.Error;
            error.Should().BeNullOrEmpty();
        }

        [Then(@"The simple test value object has errors")]
        public void ThenTheSimpleTestValueObjectHasErrors()
        {
            var valueObject = _scenarioContext.Get<SimpleTestValueObject>("valueObject");
            var error = valueObject.Error;
            error.Should().NotBeNullOrEmpty();
        }
    }
}
