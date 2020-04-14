using System;
using System.ComponentModel;
using TechTalk.SpecFlow;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    [Binding]
    internal sealed class ValidationSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public ValidationSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        internal void CreateModel<T>(Func<T> modelFactory) 
            where T:INotifyPropertyChanged
        {
            var model = modelFactory();
            var isRaised = false;
            var isRaisedRef = new WeakReference(isRaised);
            model.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Error")
                {
                    isRaisedRef.Target = true;
                }
            };
            _scenarioContext.Add("model", model);
            _scenarioContext.Add("isRaisedRef", isRaisedRef);
        }

        internal T GetModel<T>() => 
            _scenarioContext.Get<T>("model");

        internal void AssertModelHasNoError<T>(Func<T> getModel) where T : INotifyDataErrorInfo, IDataErrorInfo
        {
            AssertHelper.AssertModelHasErrorIsFalse(getModel());
        }

        internal void AssertModelHasError<T>(Func<T> getModel) where T : INotifyDataErrorInfo, IDataErrorInfo
        {
            AssertHelper.AssertModelHasErrorIsTrue(getModel());
        }
    }
}
