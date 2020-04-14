using TechTalk.SpecFlow;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    [Binding]
    internal sealed class ModelSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public ModelSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        internal T GetModel<T>() =>
            _scenarioContext.Get<T>("model");
    }
}
