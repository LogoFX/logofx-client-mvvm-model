using System.Net;
using FluentAssertions;
using Xunit;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    public class BclObjectInEditableModelTests
    {
        [Fact]
        public void StoreBclProperty()
        {
            var modelWithBcl = new EditableModelWithBclObjects();
            modelWithBcl.IsDirty.Should().BeFalse();
            var oldIp = modelWithBcl.IpAddress;
            
            modelWithBcl.IpAddress = IPAddress.Loopback;
            modelWithBcl.IsDirty.Should().BeTrue();

            modelWithBcl.CancelChanges();
            modelWithBcl.IsDirty.Should().BeFalse();
            var equals = modelWithBcl.IpAddress.Equals(oldIp);
            equals.Should().BeTrue();
        }
    }
}