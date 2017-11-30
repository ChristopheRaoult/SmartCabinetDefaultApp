using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.Model.DeviceModel
{
    public class GpioCardLib
    {
        public delegate void NotifyGpioCardHandlerDelegate(string arg);
        public event NotifyGpioCardHandlerDelegate NotifyGpioEvent;
        public string SerialPortCom { get; private set; }
        private int[] _InStatus = new int[9];
        public int[] InStatus { get { return _InStatus; } }
        private int[] _OutStatus = new int[17];
        public int[] OutStatus { get { return _OutStatus; } }
        public SerialPort serialPort;
        private string inboundBuffer = "";
        private bool _disposed = false;

        public bool IsConnected
        {
            get
            {
                if (serialPort == null)
                    return false;
                else
                    return serialPort.IsOpen;
            }
        }
        public GpioCardLib()
        {

            for (int loop = 0; loop < 9; loop++)
            {
                _InStatus[loop] = -1;

            }
            for (int loop = 0; loop < 17; loop++)
            {
                _OutStatus[loop] = -1;
            }
        }
        ~GpioCardLib()
        {
            if (serialPort != null)
            {
                CloseSerialPort();
            }
        }


        private readonly AutoResetEvent ReceiveEvent = new AutoResetEvent(false);
        public List<string> GetDevicePortCom()
        {
            List<string> comPortList = new List<string>(SerialPort.GetPortNames());
            const string VID = "04D8";
            const string PID = "000A";
            string pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);
            Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
            List<string> comports = new List<string>();
            RegistryKey rk1 = Registry.LocalMachine;
            RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
            foreach (String s3 in rk2.GetSubKeyNames())
            {
                RegistryKey rk3 = rk2.OpenSubKey(s3);
                //KB filter on FTDIBUS only
                if (!rk3.Name.Equals(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Enum\USB"))
                    continue;
                foreach (String s in rk3.GetSubKeyNames())
                {
                    if (_rx.Match(s).Success)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey(s);
                        foreach (String s2 in rk4.GetSubKeyNames())
                        {
                            RegistryKey rk5 = rk4.OpenSubKey(s2);
                            string prt = (string)rk5.GetValue("FriendlyName");
                            if (!string.IsNullOrEmpty(prt))
                            {
                                int nStart = prt.IndexOf('(');
                                int nEnd = prt.IndexOf(')') - 1;
                                string Com = prt.Substring(nStart + 1, nEnd - nStart);
                                if (comPortList.Contains(Com))
                                    comports.Add(Com);
                            }
                        }
                    }
                }
            }
            return comports;
        }
        public bool OpenSerialPort(string PortCom)
        {
            bool bRet = true;
            try
            {
                serialPort = null;
                serialPort = new SerialPort(PortCom, 9600, Parity.None, 8, StopBits.One);
                serialPort.Open();
                serialPort.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
                GetInValues();
                GetOutValues();
                SerialPortCom = PortCom;
                return bRet;
            }
            catch
            {

                return false;
            }

        }
        public bool CloseSerialPort()
        {
            try
            {
                serialPort.DataReceived -= new SerialDataReceivedEventHandler(OnDataReceived);

                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }

                serialPort = null;
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch
            {
                return false;
                // throw new Exception();
            }
        }
        protected void sendMessage(string message)
        {
            try
            {
                serialPort.Write(string.Format("{0}", message));
            }
            catch
            {
                throw new Exception();
            }
        }
        public void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (true)
            {
                if (serialPort.IsOpen)
                    inboundBuffer += serialPort.ReadExisting();

                int length = inboundBuffer.Length;
                int frameStart = inboundBuffer.IndexOf("s");
                if (frameStart != 0)
                {
                    if (frameStart < 0)
                    {
                        inboundBuffer = "";
                        break;
                    }
                    inboundBuffer = inboundBuffer.Substring(frameStart);
                    frameStart = 0;
                }
                int frameEnd = inboundBuffer.IndexOf("e", 0);
                if (frameEnd < 0)
                    break;
                // A full message is available.                

                string message = inboundBuffer.Substring(1, frameEnd - 1);
                inboundBuffer = inboundBuffer.Substring(frameEnd + 1);
                if (message == "ske") break; // c'est un ack

                if (message.StartsWith("I"))
                    processInValues(message);
                if (message.StartsWith("J"))
                    processOutValues(message);

            }
        }
        private void processInValues(string input)
        {
            string bckInput = input;
            input = input.Substring(1); //remove command Letter
            if (IsNumeric(input))
            {
                _InStatus[1] = ((int.Parse(input) & 0x01) == 0x01) ? 1 : 0;
                _InStatus[2] = ((int.Parse(input) & 0x02) == 0x02) ? 1 : 0;
                _InStatus[3] = ((int.Parse(input) & 0x04) == 0x04) ? 1 : 0;
                _InStatus[4] = ((int.Parse(input) & 0x08) == 0x08) ? 1 : 0;
                _InStatus[5] = ((int.Parse(input) & 0x10) == 0x10) ? 1 : 0;
                _InStatus[6] = ((int.Parse(input) & 0x20) == 0x20) ? 1 : 0;
                _InStatus[7] = ((int.Parse(input) & 0x40) == 0x40) ? 1 : 0;
                _InStatus[8] = ((int.Parse(input) & 0x80) == 0x80) ? 1 : 0;
            }

            if (NotifyGpioEvent != null)
            {
                NotifyGpioEvent(bckInput);
            }

        }
        private void processOutValues(string input)
        {
            string bckInput = input;
            input = input.Substring(1); //remove command Letter
            if (IsNumeric(input))
            {
                _OutStatus[1] = ((int.Parse(input) & 0x0001) == 0x0001) ? 1 : 0;
                _OutStatus[2] = ((int.Parse(input) & 0x0002) == 0x0002) ? 1 : 0;
                _OutStatus[3] = ((int.Parse(input) & 0x0004) == 0x0004) ? 1 : 0;
                _OutStatus[4] = ((int.Parse(input) & 0x0008) == 0x0008) ? 1 : 0;
                _OutStatus[5] = ((int.Parse(input) & 0x0010) == 0x0010) ? 1 : 0;
                _OutStatus[6] = ((int.Parse(input) & 0x0020) == 0x0020) ? 1 : 0;
                _OutStatus[7] = ((int.Parse(input) & 0x0040) == 0x0040) ? 1 : 0;
                _OutStatus[8] = ((int.Parse(input) & 0x0080) == 0x0080) ? 1 : 0;
                _OutStatus[9] = ((int.Parse(input) & 0x0100) == 0x0100) ? 1 : 0;
                _OutStatus[10] = ((int.Parse(input) & 0x0200) == 0x0200) ? 1 : 0;
                _OutStatus[11] = ((int.Parse(input) & 0x0400) == 0x0400) ? 1 : 0;
                _OutStatus[12] = ((int.Parse(input) & 0x0800) == 0x0800) ? 1 : 0;
                _OutStatus[13] = ((int.Parse(input) & 0x1000) == 0x1000) ? 1 : 0;
                _OutStatus[14] = ((int.Parse(input) & 0x2000) == 0x2000) ? 1 : 0;
                _OutStatus[15] = ((int.Parse(input) & 0x4000) == 0x4000) ? 1 : 0;
                _OutStatus[16] = ((int.Parse(input) & 0x8000) == 0x8000) ? 1 : 0;
            }
            if (NotifyGpioEvent != null)
            {
                NotifyGpioEvent(bckInput);
            }

        }
        public void SetOut(int OutNumber)
        {
            _OutStatus[OutNumber] = 1;
            string cmd = "sG" + OutNumber + "e";
            sendMessage(cmd);
        }
        public void ClearOut(int OutNumber)
        {
            _OutStatus[OutNumber] = 0;
            string cmd = "sH" + OutNumber + "e";
            sendMessage(cmd);
        }
        public void GetInValues()
        {
            string cmd = "sIe";
            sendMessage(cmd);
        }
        public void GetOutValues()
        {
            string cmd = "sJe";
            sendMessage(cmd);
        }
        private bool IsNumeric(string valueToCheck)
        {
            Regex regex = new Regex(@"^[-+]?\d*[.,]?\d*$");
            return regex.IsMatch(valueToCheck);
        }
    }
}
