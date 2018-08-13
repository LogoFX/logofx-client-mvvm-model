using System;
using FluentAssertions;
using Xunit;

// ReSharper disable once CheckNamespace
namespace LogoFX.Client.Mvvm.Model.Tests
{
    public class TicketDetails : EditableModel<Guid>
    {
        public TicketDetails(string listItemDescription, int status, string logRemark)
        {
            Name = listItemDescription;
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
    }


    public class EditableModelWithReadonlyFiledTests
    {
        [Fact]
        public void EditableModelWithReadonlyFiledCancelChanges()
        {
            var ticketDetails = new TicketDetails("Descr", 1, "Remark");
            ticketDetails.IsDirty.Should().Be(true);
            ticketDetails.CancelChanges();
            ticketDetails.IsDirty.Should().Be(false);
        }
    }
}