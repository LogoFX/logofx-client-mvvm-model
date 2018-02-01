using System.Net;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    class EditableModelWithBclObjects : EditableModel
    {
        private IPAddress _ipAddress = IPAddress.None;

        public IPAddress IpAddress
        {
            get { return _ipAddress; }
            set
            {
                if (Equals(_ipAddress, value))
                {
                    return;
                }

                MakeDirty();
                _ipAddress = value;
                NotifyOfPropertyChange();
            }
        }
    }
}