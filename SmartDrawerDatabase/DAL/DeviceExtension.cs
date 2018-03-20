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
        public static Device AddNewdDevice(this DbSet<Device> devices, string serialNumber, string name,string location, int deviceTypeId, string newIpAddress)
        {
            var device = new Device
            {
                DeviceSerial = serialNumber,
                DeviceName = name,
                DeviceLocation = location,
                DeviceTypeId = deviceTypeId,
                IpAddress = newIpAddress
            };

            return devices.Add(device);
        }
        public static Device GetBySerialNumber(this DbSet<Device> devices, string serialNumber)
        {
            return devices.FirstOrDefault(d => d.DeviceSerial == serialNumber);
        }
        public static Device GetByRfidSerialNumber(this DbSet<Device> devices, string rfidSerial)
        {
            return devices.FirstOrDefault(d => d.RfidSerial == rfidSerial);
        }
    }
}
