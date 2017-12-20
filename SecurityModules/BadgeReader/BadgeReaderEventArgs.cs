using System;

namespace SecurityModules.BadgeReader
{
    public class BadgeReaderEventArgs : EventArgs
    {
        public string BadgeNumber { get; private set; }
        public RadioType RadioType { get; private set; }
        public bool IsMaster { get; set; }

        public BadgeReaderEventArgs(string badgeNumber, RadioType radioType)
        {
            BadgeNumber = badgeNumber;
            RadioType = radioType;
            IsMaster = true; // true by default. Manually set  to false in Slave reader event handler
        }
    }
}
