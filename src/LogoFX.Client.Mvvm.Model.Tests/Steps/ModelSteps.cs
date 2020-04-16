using System;
using System.ComponentModel;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace LogoFX.Client.Mvvm.Model.Tests.Steps
{
    [Binding]
    internal sealed class ModelSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public ModelSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        internal void CreateModel<T>(Func<T> modelFactory)
            where T : INotifyPropertyChanged
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
            _scenarioContext.Add("isDirtyRaisedRef", isDirtyRaisedRef);
        }

        internal T GetModel<T>() =>
            _scenarioContext.Get<T>("model");

        internal void AssertNotificationIsRaised(NotificationKind kind)
        {
            string key = string.Empty;
            switch (kind)
            {
                case NotificationKind.Dirty:
                    key = "isDirtyRaisedRef";
                    break;
                case NotificationKind.Error:
                    key = "isErrorRaisedRef";
                    break;
                default:
                    key = null;
                    break;
            }
            if (key == null)
            {
                throw new NotSupportedException($"Notification {kind} is not supported");
            }
            var isRaisedRef = _scenarioContext.Get<WeakReference>(key);
            ((bool)isRaisedRef.Target).Should().BeTrue();
        }
    }
}
