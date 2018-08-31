using Microsoft.Win32;
using System.ServiceProcess;
using System.Configuration;
using System;

namespace RegistryEnforcer
{
    public partial class RegistryEnforcer : ServiceBase
    {
        private RegistryWatcher Watcher { get; set; }

        public RegistryEnforcer()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Watcher = new RegistryWatcher(Registry.LocalMachine, Properties.Settings.Default.KeyPath, Properties.Settings.Default.ValueName);
            Watcher.RegistryValueChangeEvent += Watcher_EventArrived;
        }

        protected override void OnStop()
        {
            Watcher = null;
        }

        private void Watcher_EventArrived(object sender, RegistryValueChangeEventArgs e)
        {
            var watcher = sender as RegistryWatcher;
            Console.WriteLine($"{watcher.ValueName} changed to {e.Value}");
            Watcher.Value = Properties.Settings.Default.OverrideValue;
        }

        internal void TestStartupAndStop()
        {
            OnStart(null);
            Console.ReadLine();
            OnStop();
        }
    }
}