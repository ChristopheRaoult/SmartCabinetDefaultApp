using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SDK_SC_RfidReader.UtilsWindows
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ConversionsDialog : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public ConversionsDialog()
        {
            InitializeComponent();
        }

        private void OnDBmTextChanged(object sender, EventArgs e)
        {
            double dBm;
            if (double.TryParse(srcDBmTextBox.Text, out dBm))
            {
                double uTesla = Math.Pow(10.0, ((dBm + 36.0) / 20.0)) * (4.0 * Math.PI);
                dstMicroTeslaTextBox.Text = uTesla.ToString();
            }
            else
                dstMicroTeslaTextBox.Text = "?";
        }

        private void OnMicroTeslaChanged(object sender, EventArgs e)
        {
            double uTesla;
            if (double.TryParse(srcMicroTeslaTextBox.Text, out uTesla))
            {
                double dBm = Math.Log10(uTesla / (4.0 * Math.PI)) * 20.0 - 36.0;
                dstDBmTextBox.Text = dBm.ToString();
            }
            else
                dstDBmTextBox.Text = "?";
        }
    }
}
