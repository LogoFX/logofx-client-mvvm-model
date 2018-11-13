using System.Runtime.CompilerServices;

namespace LogoFX.Client.Mvvm.Model
{
    public partial class EditableModel<T>
    {
        /// <summary>
        /// Compares the current and new values. If they are different, optionally marks the model as dirty, 
        /// updates the respective field 
        /// and fires the property change notification.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="currentValue">The current value field reference.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="name">The property name.</param>
        /// <param name="markAsDirty">True, if the model should be marked as dirty, false otherwise. The default value is <c>true</c></param>
        protected void SetProperty<TProperty>(
            ref TProperty currentValue, 
            TProperty newValue, 
            [CallerMemberName] string name = "",
            bool markAsDirty = true)
        {
            if (Equals(currentValue, newValue))
            {
                return;
            }
            if (markAsDirty)
            {
                MakeDirty();
            }            
            currentValue = newValue;
            NotifyOfPropertyChange(name);
        }
    }
}
