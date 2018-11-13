using System.Net;
using FluentAssertions;
using Xunit;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    public class BclObjectInEditableModelCancelChangesTests
    {
        [Fact]
        public void BclObjectIsChangedAndChangeIsCancelled_PropertyValueIsReverted()
        {
            var modelWithBcl = new EditableModelWithBclObjects();
            var oldIp = modelWithBcl.IpAddress;
            
            modelWithBcl.IpAddress = IPAddress.Loopback;
            modelWithBcl.CancelChanges();

            modelWithBcl.IpAddress.Should().Be(oldIp);
            modelWithBcl.IsDirty.Should().BeFalse();
        }
    }
}