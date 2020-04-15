using System;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    public class EditableModelWithReadOnlyField : EditableModel<Guid>
    {
        public EditableModelWithReadOnlyField(int status, string logRemark)
        {
            Status = status;
            _logRemark = logRemark;
        }

        private int _status;
        public int Status
        {
            get => _status;
            set
            {
                if (value == _status)
                {
                    return;
                }

                MakeDirty();
                _status = value;
                NotifyOfPropertyChange();
            }
        }

        private readonly string _logRemark;
        public string LogRemark => _logRemark;
    }
}