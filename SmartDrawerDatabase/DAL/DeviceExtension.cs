using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    public static class DeviceExtension
    {
        public static Device AddNewdDevice(this DbSet<Device> devices, string serialNumber, string name,
           DeviceType deviceType, string newIpAddress)
        {
            var device = new Device
            {
                SerialNumber = serialNumber,
                Name = name,
                DeviceType = deviceType,
                IpAddress = newIpAddress
            };

            return devices.Add(device);
        }
        public static Device GetByRfidSerialNumber(this DbSet<Device> devices, string rfidSerialNumber)
        {
            return devices.SingleOrDefault(d => d.RfidSerial == rfidSerialNumber);
        }
    }
}
