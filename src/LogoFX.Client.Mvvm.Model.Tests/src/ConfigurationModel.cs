using System;
using System.Collections.Generic;
using LogoFX.Client.Mvvm.Model.Contracts;
using LogoFX.Core;

namespace LogoFX.Client.Mvvm.Model.Tests
{
    public interface IHost : IModel<Guid>
    {

    }

    public abstract class HostBase : Model<Guid>, IHost
    {

    }

    public interface IHostName : IHost
    {
    }

    public class HostName : HostBase, IHostName
    {
        public HostName(string name = null)
        {
            Name = name;
        }

        private string _name;

        public new string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;

                _name = value;
                NotifyOfPropertyChange();
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public interface IConfiguration : IModel<Guid>
    {
        IEnumerable<IHost> IncludedHosts { get; }
    }

    public abstract class ConfigurationModelBase : EditableModel<Guid>, IConfiguration
    {
        private readonly RangeObservableCollection<IHost> _includedHosts;

        protected ConfigurationModelBase(IHost[] hosts)
        {
            _includedHosts = new RangeObservableCollection<IHost>(hosts);
        }

        [EditableList]
        public IEnumerable<IHost> IncludedHosts
        {
            get { return _includedHosts; }
        }

        public void AddHost(IHost host)
        {
            MakeDirty();
            _includedHosts.Add(host);
        }

        public void RemoveHost(IHost host)
        {
            MakeDirty();
            _includedHosts.Remove(host);
        }
    }

    public class ConfigurationModel : ConfigurationModelBase
    {
        public ConfigurationModel(params IHost[] hosts)
            : base(hosts)
        {
        }
    }
}