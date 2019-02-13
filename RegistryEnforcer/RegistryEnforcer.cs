using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;

namespace RegistryEnforcer
{
    public partial class RegistryEnforcer : ServiceBase
    {
        private List<RegistryWatcher> Watchers { get; set; } = new List<RegistryWatcher>();

        public RegistryEnforcer()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            foreach (RegistryItem item in Properties.Settings.Default.ItemsToEnforce)
            {
                RegistryWatcher watcher = new RegistryWatcher(Registry.LocalMachine, item.KeyPath, item.ValueName);
                watcher.RegistryValueChangeEvent += Watcher_EventArrived;
                Watchers.Add(watcher);
            }
        }

        protected override void OnStop()
        {
            Watchers.Clear();
        }

        private void Watcher_EventArrived(object sender, RegistryValueChangeEventArgs e)
        {
            RegistryWatcher watcher = sender as RegistryWatcher;
            Console.WriteLine($"{watcher.ValueName} changed to {e.Value}");
            RegistryItem item = Properties.Settings.Default.ItemsToEnforce.SingleOrDefault(i => i.KeyPath == watcher.KeyPath && i.ValueName == watcher.ValueName);

            if (item != null)
            {
                Console.WriteLine($"{watcher.ValueName} changed to {e.Value}");
                watcher.Value = item.OverrideValue; 
            }
        }

        internal void TestStartupAndStop()
        {
            OnStart(null);
            Console.ReadLine();
            OnStop();
        }
    }
}