#if NETSTANDARD2_0
using System;
#endif

namespace LogoFX.Client.Mvvm.Model.Contracts
{
    /// <summary>
    /// Represents filter model
    /// </summary>
    public interface IFilterModel : IValueObject
#if NETSTANDARD2_0
          , ICloneable
#endif      
    {

    }
}
