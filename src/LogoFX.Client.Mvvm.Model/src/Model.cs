using System;
#if NET45 || NETSTANDARD2_0
using System.Runtime.Serialization;
#endif
using LogoFX.Client.Core;
using LogoFX.Client.Mvvm.Model.Contracts;

namespace LogoFX.Client.Mvvm.Model
{
    /// <summary>
    /// Represents model for domain use
    /// </summary>
    /// <typeparam name="T">Type of model identifier</typeparam>
#if NET45 || NETSTANDARD2_0
    [DataContract]
#endif
    public partial class Model<T> : NotifyPropertyChangedBase<Model<T>>, IModel<T>        
        where T : IEquatable<T> 
    {        
        /// <summary>
        /// Returns current object type.
        /// </summary>
        protected readonly Type Type;

        /// <summary>
        /// Initializes a new instance of the <see cref="Model&lt;T&gt;"/> class.
        /// </summary>
        public Model()
        {
            Type = GetType();
            InitErrorListener();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Model&lt;T&gt;"/> class from other instance.
        /// </summary>
        /// <param name="other">The other model</param>
        public Model(IModel<T> other)
            :this()
        {
            _id = other.Id;
            _name = other.Name;
            _description = other.Description;
        }

#region Public Properties 

#region Id property
        
        private T _id;
        /// <summary>
        /// Model identifier
        /// </summary>
        public T Id
        {
            get { return _id; }
            set
            {
                if (Equals(_id, value))
                {
                    return;
                }                

                T oldValue = _id;
                _id = value;
                OnIdChangedOverride(value, oldValue);
                OnPropertyChanged(()=>Id);
            }
        }

        /// <summary>
        /// Override this method to inject custom logic during id set operation.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="oldValue">The old value.</param>
        protected virtual void OnIdChangedOverride(T newValue, T oldValue)
        {
        }

        #endregion

#region Name property

#if NET45 || NETSTANDARD2_0
        [DataMember(Name="Name")]
#endif
        private string _name;
        /// <summary>
        /// Model name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                    return;

                string oldValue = _name;
                _name = value;
                OnNameChangedOverride(value, oldValue);
                NotifyOfPropertyChange(() => Name);
            }
        }

        /// <summary>
        /// Override this method to inject custom logic during name set operation..
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="oldValue">The old value.</param>
        protected virtual void OnNameChangedOverride(string newValue, string oldValue)
        {
        }

#endregion

#region Description property

#if NET45 || NETSTANDARD2_0
        [DataMember(Name = "Description")]
#endif
        private string _description;
        /// <summary>
        /// Model description
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description == value)
                    return;

                string oldValue = _description;
                _description = value;
                OnDescriptionChangedOverride(value, oldValue);
                OnPropertyChanged(()=>Description);
            }
        }

        /// <summary>
        /// Override this method to inject custom logic during name set operation..
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="oldValue">The old value.</param>
        protected virtual void OnDescriptionChangedOverride(string newValue, string oldValue)
        {
        }

#endregion
        
#endregion

#region Overrides  
              
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Name)?base.ToString():Name;
        }

#endregion
    }

    /// <summary>
    /// Represents model with <see cref="int"/> as identifier.
    /// </summary>
#if NET45 || NETSTANDARD2_0
    [DataContract]
#endif
    public class Model : Model<int>, IModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class
        /// </summary>
        public Model()
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Model"/> class.
        /// </summary>
        /// <param name="other">The other.</param>
        public Model(Model other):base(other)
        {
        }
    }
}
