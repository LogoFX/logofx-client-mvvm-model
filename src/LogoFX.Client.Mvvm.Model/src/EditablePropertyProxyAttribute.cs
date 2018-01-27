using System;

namespace LogoFX.Client.Mvvm.Model.Contracts
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EditablePropertyProxyAttribute : Attribute
    {
        public EditablePropertyProxyAttribute(string fieldName)
        {
            FieldName = fieldName;
        }

        public string FieldName { get; private set; }
    }
}