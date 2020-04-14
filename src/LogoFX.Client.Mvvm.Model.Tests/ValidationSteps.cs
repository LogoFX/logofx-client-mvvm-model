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
            var isErrorRaised = false;
            var isErrorRaisedRef = new WeakReference(isErrorRaised);
            var isDirtyRaised = false;
            var isDirtyRaisedRef = new WeakReference(isDirtyRaised);
            model.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Error")
                {
                    isErrorRaisedRef.Target = true;
                }
                if (args.PropertyName == "IsDirty")
                {
                    isDirtyRaisedRef.Target = true;
                }
            };
            _scenarioContext.Add("model", model);
            _scenarioContext.Add("isErrorRaisedRef", isErrorRaisedRef);
            _scenarioContext.Add("isDirtyRaisedRef", isErrorRaisedRef);
        }

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
