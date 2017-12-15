using System.ComponentModel;
using System.Linq;
using FluentAssertions;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    static class AssertHelper
    {
        internal static void AssertModelHasErrorIsFalse<T>(T model) where T : INotifyDataErrorInfo
#if NET45 || NETSTANDARD2_0
            , IDataErrorInfo
#endif
        {
            var hasErrors = model.HasErrors;
            var collectionOfErrorsIsEmpty = model.GetErrors(null).OfType<string>().Any() == false;
#if NET45 || NETSTANDARD2_0
            model.Error.Should().BeNullOrEmpty();
#endif
            hasErrors.Should().BeFalse();
            collectionOfErrorsIsEmpty.Should().BeTrue();
        }

        internal static void AssertModelHasErrorIsTrue<T>(T model) where T : INotifyDataErrorInfo
#if NET45 || NETSTANDARD2_0
            , IDataErrorInfo
#endif
        {
            var hasErrors = model.HasErrors;
            var collectionOfErrorsIsEmpty = model.GetErrors(null).OfType<string>().Any() == false;
#if NET45 || NETSTANDARD2_0
            model.Error.Should().NotBeNullOrEmpty();
#endif
            hasErrors.Should().BeTrue();
            collectionOfErrorsIsEmpty.Should().BeFalse();
        }
    }
}