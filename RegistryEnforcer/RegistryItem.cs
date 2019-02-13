using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistryEnforcer
{
    [Serializable]
    public class RegistryItemList : List<RegistryItem>
    {

    }

    [Serializable]
    public class RegistryItem
    {
        public string KeyPath { get; set; }
        public string ValueName { get; set; }
        public int OverrideValue { get; set; }

        public RegistryItem() : this("", "", 0) { }

        public RegistryItem(string keyPath, string valueName, int overrideValue)
        {
            KeyPath = keyPath;
            ValueName = valueName;
            OverrideValue = overrideValue;
        }
    }
}
