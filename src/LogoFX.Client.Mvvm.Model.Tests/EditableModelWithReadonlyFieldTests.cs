using System;
using FluentAssertions;
using Xunit;

// ReSharper disable once CheckNamespace
namespace LogoFX.Client.Mvvm.Model.Tests
{
    public class TicketDetails : EditableModel<Guid>
    {
        public TicketDetails(int status, string logRemark)
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

        public string LogRemark => _logRemark;

        private readonly string _logRemark;
    }


    public class EditableModelWithReadonlyFieldTests
    {
        [Fact]
        public void EditableModelWithReadonlyFiled_CancelChanges_IsDirtyShouldBeFalse()
        {
            var ticketDetails = new TicketDetails(1, "Log Remark");
            ticketDetails.Status = 2;
            ticketDetails.CancelChanges();

            ticketDetails.IsDirty.Should().Be(false);
            ticketDetails.LogRemark.Should().Be("Log Remark");
        }
    }
}