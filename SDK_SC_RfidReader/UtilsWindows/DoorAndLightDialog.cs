using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using SDK_SC_RfidReader.DeviceBase;

namespace SDK_SC_RfidReader.UtilsWindows
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DoorAndLightDialog : Form
    {
        readonly private DeviceRfidBoard deviceBoard;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceBoard"></param>
        public DoorAndLightDialog(DeviceRfidBoard deviceBoard)
        {
            this.deviceBoard = deviceBoard;
                       InitializeComponent();

            PbRspGetStatus status = (PbRspGetStatus)deviceBoard.getStatus();
            if ((status.doorStatus & (int)DoorStatusFlags.DSF_DoorIsOpen) != 0)
                doorStateLabel.Text = "Door Open";
            else
                doorStateLabel.Text = "Door Closed";


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncEventMessage"></param>
        public void handleAsyncEvent(AsyncEventMessage asyncEventMessage)
        {

            switch (asyncEventMessage.asyncEventType)
            {
                case AsyncEventType.PBET_DrawerOpened:
                    Invoke((MethodInvoker)delegate { doorStateLabel.Text = "Door Open"; });

                    break;

                case AsyncEventType.PBET_DrawerClosed:
                     Invoke((MethodInvoker)delegate {doorStateLabel.Text = "Door Closed";});
                    break;
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            deviceBoard.LockDoor(DoorValue.DV_Master, false);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            deviceBoard.LockDoor(DoorValue.DV_Master, true);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            deviceBoard.LockDoor(DoorValue.DV_Slave, false);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            deviceBoard.LockDoor(DoorValue.DV_Slave, true);
        }

        private void trackBarLight_Scroll(object sender, EventArgs e)
        {
            deviceBoard.SetLightDuty((ushort)trackBarLight.Value);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            deviceBoard.setInfraRedSensor(true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            deviceBoard.setInfraRedSensor(false);
        }
    }
}
