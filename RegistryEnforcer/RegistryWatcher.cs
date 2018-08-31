/****************************** Module Header ******************************\
* Module Name:  RegistryWatcher.cs
* Project:	    CSMonitorRegistryChange
* Copyright (c) Microsoft Corporation.
*
* This class derived from ManagementEventWatcher. It is used to
* 1. Supply the supported hives.
* 2. Construct a WqlEventQuery from Hive and KeyPath.
* 3. Wrap the EventArrivedEventArgs to RegistryKeyChangeEventArg.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management;

namespace RegistryEnforcer
{
    public class RegistryWatcher : ManagementEventWatcher, IDisposable
    {
        /// <summary>
        /// Changes to the HKEY_CLASSES_ROOT and HKEY_CURRENT_USER hives are not supported
        /// by RegistryEvent or classes derived from it, such as RegistryKeyChangeEvent.
        /// </summary>
        private static ReadOnlyCollection<RegistryKey> SupportedHives = new List<RegistryKey> {
                Registry.LocalMachine,
                Registry.Users,
                Registry.CurrentConfig
            }.AsReadOnly();

        public RegistryKey Hive { get; private set; }
        public string KeyPath { get; private set; }
        public string ValueName { get; private set; }
        public RegistryKey KeyToMonitor { get; private set; }

        public object Value
        {
            get
            {
                return KeyToMonitor?.GetValue(ValueName);
            }
            set
            {
                object currentValue = KeyToMonitor?.GetValue(ValueName);
                if (currentValue != null && currentValue != value)
                {
                    KeyToMonitor?.SetValue(ValueName, value);
                }
            }
        }

        public event EventHandler<RegistryValueChangeEventArgs> RegistryValueChangeEvent;

        /// <exception cref="System.Security.SecurityException">
        /// Thrown when current user does not have the permission to access the key
        /// to monitor.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the key to monitor does not exist.
        /// </exception>
        public RegistryWatcher(RegistryKey hive, string keyPath, string valueName)
        {
            Hive = hive ?? throw new ArgumentNullException(nameof(hive));
            KeyPath = keyPath ?? throw new ArgumentNullException(nameof(keyPath));
            ValueName = valueName ?? throw new ArgumentNullException(nameof(valueName));

            if (!SupportedHives.Contains(hive))
            {
                throw new ArgumentException($"{hive} not supported", nameof(hive));
            }

            // If you set the platform of this project to x86 and run it on a 64bit
            // machine, you will get the Registry Key under
            // HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node when the key path is
            // HKEY_LOCAL_MACHINE\SOFTWARE
            KeyToMonitor = hive.OpenSubKey(keyPath, true);

            if (KeyToMonitor == null)
            {
                throw new ArgumentException($@"The registry key {Hive.Name}\{KeyPath} does not exist");
            }

            if (Value == null)
            {
                throw new ArgumentException($@"The registry value {ValueName} does not exist in {Hive.Name}\{KeyPath}");
            }

            // Construct the query string.
            string queryString = $@"SELECT * FROM RegistryValueChangeEvent WHERE Hive = '{Hive.Name}' AND KeyPath = '{KeyPath.Replace(@"\", @"\\")}' AND ValueName = '{ValueName}'";

            Query = new EventQuery(queryString);

            EventArrived += RegistryWatcher_EventArrived;

            Start();
        }

        private void RegistryWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            // Raise the event handler.
            RegistryValueChangeEvent?.Invoke(sender, new RegistryValueChangeEventArgs(Value));
        }

        /// <summary>
        /// Dispose the RegistryKey.
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
            if (KeyToMonitor != null)
            {
                KeyToMonitor.Dispose();
            }
        }
    }
}