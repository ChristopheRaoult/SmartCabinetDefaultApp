using System;
using System.IO.Ports;
using System.Linq;

namespace SecurityModules.BadgeReader
{
    public class BadgeReader
    {
        public event BadgeReaderEventHandler BadgeReaderEvent;
        public delegate void BadgeReaderEventHandler(object sender, BadgeReaderEventArgs args);

        private const char StartOfFrameLF = (char)0x02;
        private const char EndOfFrameLF = (char)0x03;
        private const char EndOfFrameHF = (char)0x0D;
        private string _inboundBuffer = "";

        private SerialPort _serialPort;
        private readonly RadioType _radioType;

        public BadgeReader(RadioType radioType, string comPortName)
        {
            _radioType = radioType;
            OpenSerialPort(comPortName);
        }

        ~BadgeReader()
        {
            CloseSerialPort();
        }

        public void OpenSerialPort(string strCom)
        {
            string[] ports = SerialPort.GetPortNames();


            if (!ports.Any(port => port.Equals(strCom)))
            {
                return;
            }

            // if it's open, close / destroy the current one, before.
            CloseSerialPort();

            _serialPort = new SerialPort(strCom, 9600, Parity.None, 8, StopBits.One);

            try
            {
                _serialPort.Open();
            }
            catch (UnauthorizedAccessException)
            {
                // should not happen if the COM port is correct and not already in use
                return;
            }

            if (_serialPort.IsOpen)
            {
                _serialPort.DataReceived += OnDataReceived;
            }
        }

        public void CloseSerialPort()
        {
            if (_serialPort != null)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.DataReceived -= OnDataReceived;
                    _serialPort.Close();
                }
            }

            _serialPort = null;
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var badgeID = String.Empty;
            int frameEnd = -1;

            switch (_radioType)
            {
                case RadioType.HF:
                    do
                    {
                        _inboundBuffer += _serialPort.ReadExisting();
                        frameEnd = _inboundBuffer.IndexOf(EndOfFrameHF, 0);

                        if (frameEnd != -1)
                        {
                            // A full message is available.
                            string message = _inboundBuffer.Substring(0, frameEnd - 2);
                            _inboundBuffer = _inboundBuffer.Substring(frameEnd + 1);
                            badgeID = message.Substring(0, 10);
                        }
                    } while (frameEnd != -1);
                    break;

                case RadioType.LF:
                    int frameStart;

                    do
                    {
                        _inboundBuffer += _serialPort.ReadExisting();
                        frameStart = _inboundBuffer.IndexOf(StartOfFrameLF);

                        if (frameStart == -1)
                        {
                            _inboundBuffer = "";
                        }

                        else
                        {
                            _inboundBuffer = _inboundBuffer.Substring(frameStart);

                            frameEnd = _inboundBuffer.IndexOf(EndOfFrameLF, 0);

                            if (frameEnd != -1)
                            {
                                string message = _inboundBuffer.Substring(1, frameEnd - 1);
                                _inboundBuffer = _inboundBuffer.Substring(frameEnd + 1);
                                badgeID = message.Substring(0, 12);
                            }
                        }
                    } while (frameStart != -1 && frameEnd != -1);
                    break;
            }

            if (String.IsNullOrEmpty(badgeID))
            {
                return;
            }

            var args = new BadgeReaderEventArgs(badgeID, _radioType);

            var handler = BadgeReaderEvent;

            if (handler != null)
            {
                handler(this, args);
            }
        }
    }

    public enum RadioType
    {
        LF = 0x00,
        HF = 0x01,
    }
}
