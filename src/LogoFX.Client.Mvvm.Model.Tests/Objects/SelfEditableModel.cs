namespace LogoFX.Client.Mvvm.Model.Tests.Objects
{
    public class SelfEditableModel : EditableModel
    {
        private string _value;

        public string Value
        {
            get
            {
                return _value;
            }

            set
            {
                if (_value == value)
                {
                    return;
                }

                MakeDirty();
                _value = value;
                NotifyOfPropertyChange();
            }
        }

        public SelfEditableModel Self { get; set; }
    }
}