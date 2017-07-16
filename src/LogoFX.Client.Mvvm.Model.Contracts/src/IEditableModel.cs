#if NET45
using System.ComponentModel;
#endif

namespace LogoFX.Client.Mvvm.Model.Contracts
{
    /// <summary>
    /// Represents editable model
    /// </summary>
    public interface IEditableModel :
#if NET45
        IEditableObject,
#endif
ICanBeDirty, ICanCancelChanges, ICanCommitChanges
    {
        
    }
}