using System.Net;
using FluentAssertions;
using Xunit;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    public class ConfigurationModelTests
    {
        [Fact]
        public void ConfigurationModelSimpleTest()
        {
            var ipList = new IHost[]
            {
                new HostName("127.0.0.1"), 
                new HostName("127.0.0.2"), 
                new HostName("127.0.0.3"), 
                new HostName("127.0.0.4"), 
            };

            var model = new ConfigurationModel(ipList);

            model.AddHost(new HostName("192.168.0.1"));

            model.CancelChanges();

            // ReSharper disable once CoVariantArrayConversion
            model.IncludedHosts.Should().BeEquivalentTo(ipList);
        }
    }
}