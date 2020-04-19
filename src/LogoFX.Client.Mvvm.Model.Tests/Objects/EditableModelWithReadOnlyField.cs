using System;

namespace LogoFX.Client.Mvvm.Model.Tests.Objects
{
    public class EditableModelWithReadOnlyField : EditableModel<Guid>
    {
        public EditableModelWithReadOnlyField(int status, string logRemark)
        {
            Status = status;
            LogRemark = logRemark;
        }

        private int _status;
        public int Status
        {
            get => _status;
            set => SetPropertyOptions(ref _status, value);
        }

        public string LogRemark { get; }
    }

    public class EditableModelWithBeforeValueUpdate : EditableModel<Guid>
    {
        public EditableModelWithBeforeValueUpdate(int status)
        {
            Status = status;
        }

        private int _status;
        public int Status
        {
            get => _status;
            set => SetPropertyOptions(ref _status, value, new SetPropertyOptions()
            {
                BeforeValueUpdate = () => PreviousValue = _status
            });
        }

        public int PreviousValue { get; private set; }
    }
}