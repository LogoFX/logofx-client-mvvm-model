﻿using System;
using System.Runtime.CompilerServices;

namespace LogoFX.Client.Mvvm.Model
{
    public partial class EditableModel<T>
    {
        /// <summary>
        /// Compares the current and new values. If they are different, optionally marks the model as dirty, 
        /// updates the respective field 
        /// and fires the property change notification.
        /// NOTE: This method will be removed in the next stable release. Use one with <see cref="SetPropertyOptions"/> instead.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="currentValue">The current value field reference.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="name">The property name.</param>
        /// <param name="markAsDirty">True, if the model should be marked as dirty, false otherwise. The default value is <c>true</c></param>
        [Obsolete]
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

        /// <summary>
        /// Compares the current and new values. If they are different,
        /// invokes the functionality which is set in the <b>options</b> parameter, 
        /// updates the respective field 
        /// and fires the property change notification.
        /// NOTE: This method will be renamed in the next stable release to <c>SetOptions</c>.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="currentValue">The current value field reference.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="options">The set property options.</param>
        /// <param name="name">The property name.</param>
        protected void SetPropertyOptions<TProperty>(
            ref TProperty currentValue,
            TProperty newValue,
            SetPropertyOptions options = null,
            [CallerMemberName] string name = "")
        {
            options = options ?? new SetPropertyOptions();
            if (options.MarkAsDirty)
            {
                if (options.BeforeValueUpdate != null)
                {
                    options.BeforeValueUpdate = () =>
                    {
                        options.BeforeValueUpdate();
                        MakeDirty();
                    };
                }
                else
                {
                    options.BeforeValueUpdate = MakeDirty;
                }
            }
            base.SetProperty(ref currentValue, newValue, options, name);
        }
    }

    /// <summary>
    /// The <see cref="EditableModel"/> set property options
    /// </summary>
    public class SetPropertyOptions : Core.SetPropertyOptions
    {
        /// <summary>
        /// True, if the model should be marked as dirty, false otherwise. The default value is <c>true</c>
        /// </summary>
        public bool MarkAsDirty { get; set; } = true;
    }
}
