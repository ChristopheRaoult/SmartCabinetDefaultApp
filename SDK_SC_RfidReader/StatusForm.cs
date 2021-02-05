using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SDK_SC_RfidReader
{
    public partial class StatusForm : Form
    {
        RfidReader rd;
        /// <summary>
        /// Constructeur form debug reader
        /// </summary>
        /// <param name="rd">object reader to debug</param>
        public StatusForm(RfidReader rd)
        {
            InitializeComponent();
            this.rd = rd;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelReader.Text = "Reader : " + rd.SerialNumber;
            labelConnected.Text = "Connected : " + rd.IsConnected.ToString();
            labelInScan.Text = "InScan : " + rd.IsInScan.ToString();
            labelLock.Text = "Lock : " + rd.Lock_Status.ToString();
            labelDoor.Text = "Door : " + rd.Door_Status.ToString();

            if (rd.ErrBoard != null)
            {
                labelErr.Text = "Last Err : " + rd.ErrBoard.dt.ToString("G");
                labelmes.Text = "Message : " + rd.ErrBoard.message.ToString();
            }

            this.Refresh();
        }
    }
}
