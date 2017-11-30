using SmartDrawerDatabase.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.Model.DeviceModel
{
    public class InventoryEventArgs
    {
        public Inventory Inventory { get; private set; }
        public List<string> UnknownTags { get; private set; }

        public InventoryEventArgs(Inventory entityInventory, List<string> unknownTags)
        {
            Inventory = entityInventory;
            UnknownTags = unknownTags;
        }
    }
}
