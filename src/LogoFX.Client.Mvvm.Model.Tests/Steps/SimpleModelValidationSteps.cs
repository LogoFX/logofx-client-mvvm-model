﻿using LogoFX.Client.Mvvm.Model.Tests.Helpers;
using LogoFX.Client.Mvvm.Model.Tests.Objects;
using TechTalk.SpecFlow;

namespace LogoFX.Client.Mvvm.Model.Tests.Steps
{
    [Binding]
    internal sealed class SimpleModelValidationSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly ModelSteps _modelSteps;
        private readonly ValidationSteps _validationSteps;

        public SimpleModelValidationSteps(
            ScenarioContext scenarioContext,
            ModelSteps modelSteps,
            ValidationSteps validationSteps)
        {
            _scenarioContext = scenarioContext;
            _modelSteps = modelSteps;
            _validationSteps = validationSteps;
        }

        [When(@"The simple model is created with valid name")]
        public void WhenTheSimpleModelIsCreatedWithValidName()
        {
            _modelSteps.CreateModel(()=> new SimpleModel(DataGenerator.ValidName, 5));
        }

        [When(@"The simple model is created with invalid name")]
        public void WhenTheSimpleModelIsCreatedWithInvalidName()
        {
            _modelSteps.CreateModel(() => new SimpleModel(DataGenerator.InvalidName, 5));
        }

        [When(@"The simple model is updated with external error")]
        public void WhenTheSimpleModelIsUpdatedWithExternalError()
        {
            var model = _modelSteps.GetModel<SimpleModel>();
            var propertyName = "Name";
            _scenarioContext.Add("propertyName", propertyName);
            model.SetError("external error", "Name");
        }

        [When(@"The simple model is cleared from external errors")]
        public void WhenTheSimpleModelIsClearedFromExternalErrors()
        {
            var model = _modelSteps.GetModel<SimpleModel>();
            var propertyName = _scenarioContext.Get<string>("propertyName");
            model.ClearError(propertyName);
        }

        [When(@"The simple model is updated with invalid value for property")]
        public void WhenTheSimpleModelIsUpdatedWithInvalidValueForProperty()
        {
            var model = _modelSteps.GetModel<SimpleModel>();
            model.Name = DataGenerator.InvalidName;
        }

        [Then(@"The simple model has no errors")]
        public void ThenTheSimpleModelHasNoErrors()
        {
            _validationSteps.AssertModelHasNoError(_modelSteps.GetModel<SimpleModel>);
        }

        [Then(@"The simple model has errors")]
        public void ThenTheSimpleModelHasErrors()
        {
            _validationSteps.AssertModelHasError(_modelSteps.GetModel<SimpleModel>);
        }
    }
}
