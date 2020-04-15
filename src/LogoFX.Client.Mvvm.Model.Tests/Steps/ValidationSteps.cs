using System;
using System.ComponentModel;
using LogoFX.Client.Mvvm.Model.Tests.Helpers;
using TechTalk.SpecFlow;

namespace LogoFX.Client.Mvvm.Model.Tests.Steps
{
    internal enum NotificationKind
    {
        Error = 0,
        Dirty = 1
    };

    [Binding]
    internal sealed class ValidationSteps
    {
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
