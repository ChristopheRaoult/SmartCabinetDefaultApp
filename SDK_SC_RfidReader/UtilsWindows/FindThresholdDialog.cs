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
    public partial class FindThresholdDialog : Form
    {
        readonly private DeviceRfidBoard deviceBoard;

        uint[] countsPresent = new uint[256];
        uint[] countsMissing = new uint[256];
        Point chartBase;
        Point chartSize;
        bool bSamplingRequested = false;
        bool bSamplingActive = false;

        int CarrierPeriod = 0;

        Dictionary<int, int> freqTab = new Dictionary<int, int>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceBoard"></param>
        public FindThresholdDialog(DeviceRfidBoard deviceBoard)
        {
            this.deviceBoard = deviceBoard;

            freqTab.Add(338, 118300);
            freqTab.Add(336, 119100);
            freqTab.Add(334, 119700);
            freqTab.Add(332, 120500);
            freqTab.Add(330, 121200);
            freqTab.Add(328, 121900);
            freqTab.Add(326, 122700);
            freqTab.Add(324, 123500);
            freqTab.Add(322, 124200);
            freqTab.Add(320, 125000);
            freqTab.Add(318, 125800);
            freqTab.Add(316, 126600);


            InitializeComponent();

            autoUpdateCheckBox.Checked = true;
            sampleCountTextBox.Text = "64";
            cycleCheckBox.Checked = true;

            GetFreq();

            byte correlationThreshold = deviceBoard.getCorrelationThreshold();           
            correlationThresholdTextBox.Text = correlationThreshold.ToString();

        }

        private void clearCounts()
        {
            for (uint i = 0; i < countsPresent.Length; i++)
                countsPresent[i] = 0;
            for (uint i = 0; i < countsMissing.Length; i++)
                countsMissing[i] = 0;
        }

        private void startSampling()
        {
            bSamplingRequested = true;
        
            if (!bSamplingActive)
            {
                ushort sampleCount = 0;

                if (autoUpdateCheckBox.Checked && (!ushort.TryParse(sampleCountTextBox.Text, out sampleCount) || (sampleCount == 0)))
                    MessageBox.Show("Invalid sample count: '" + sampleCountTextBox.Text + "'.");
                else if (deviceBoard.sampleCorrelationSeries(true, sampleCount))
                    bSamplingActive = true;
                else
                    MyDebug.Assert(false);
            }
        }

        private void stopSampling()
        {
            bSamplingRequested = false;
            if (bSamplingActive)
                deviceBoard.sampleCorrelationSeries(false, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="backDoorPacket"></param>
        public void ProcessData(PBAE_BackDoorInfo backDoorPacket)
        {
            if (cycleCheckBox.Checked)
                clearCounts();

            deviceBoard.getCorrelationCounts(false, backDoorPacket.value1, countsPresent);
            deviceBoard.getCorrelationCounts(true, backDoorPacket.value2, countsMissing);
            bSamplingActive = false;
            Invalidate();
            if (bSamplingRequested)
                startSampling();
        }

        private uint MaxSampleCount(uint[] samples)
        {
            // Find maximum sample value.
            uint maxCount = 0;
            for (uint i = 0; i < 256; i++)
            {
                if (samples[i] > maxCount)
                    maxCount = samples[i];
            }
            return maxCount;
        }

        private void ShowSamples(Graphics g, Pen pen, uint[] samples, uint maxCount,
            out double mean, out double sigma, bool showSigma, int sigmaYOffset)
        {
            if (maxCount == 0)
            {
                // Set out parameters to something.
                mean = 0;
                sigma = 128;
            }
            else
            {
                int minimum;
                int maximum;
                findMean(samples, out mean, out sigma, out minimum, out maximum);

                // Draw graph.
                double xScale = (double)chartSize.X / 256;
                double yScale = (double)chartSize.Y / maxCount;

                int lastX = chartBase.X;
                int lastY = chartBase.Y;
                for (int sample = 0; sample < 256; sample++)
                {
                    int nextX = chartBase.X + (int)(xScale * sample);
                    int nextY = chartBase.Y - (int)(yScale * samples[sample]);
                    if ((nextY == chartBase.Y) && (samples[sample] > 0))
                        nextY = chartBase.Y - 1; // Non-zero values always show as non-zero.
                    g.DrawLine(pen, lastX, lastY, nextX, nextY);
                    lastX = nextX;
                    lastY = nextY;
                }

                if (showSigma)
                {
                    // Show the mean, standard deviation, and range.
                    int meanX = chartBase.X + (int)(xScale * (mean + 0.5));
                    int sigmaY = chartBase.Y - (chartSize.Y * sigmaYOffset / 100);
                    int sigmaWid = (int)(sigma + 0.5);
                    int sigmaHgt = chartSize.Y / 10;
                    if (sigmaHgt > 6)
                        sigmaHgt = 6;

                    // Draw sigma box.
                    g.DrawLine(pen, meanX - sigmaWid, sigmaY - sigmaHgt, meanX + sigmaWid, sigmaY - sigmaHgt);
                    g.DrawLine(pen, meanX + sigmaWid, sigmaY - sigmaHgt, meanX + sigmaWid, sigmaY + sigmaHgt);
                    g.DrawLine(pen, meanX + sigmaWid, sigmaY + sigmaHgt, meanX - sigmaWid, sigmaY + sigmaHgt);
                    g.DrawLine(pen, meanX - sigmaWid, sigmaY + sigmaHgt, meanX - sigmaWid, sigmaY - sigmaHgt);

                    // Draw range line.
                    int minX = chartBase.X + (int)(xScale * minimum);
                    int maxX = chartBase.X + (int)(xScale * maximum);
                    g.DrawLine(pen, meanX - sigmaWid, sigmaY, minX, sigmaY);
                    g.DrawLine(pen, meanX + sigmaWid, sigmaY, maxX, sigmaY);
                    g.DrawLine(pen, minX, sigmaY - sigmaHgt / 2, minX, sigmaY + sigmaHgt / 2);
                    g.DrawLine(pen, maxX, sigmaY - sigmaHgt / 2, maxX, sigmaY + sigmaHgt / 2);
                }
            }
        }

        private void UpdateChart()
        {
            // Request the correlation counts. The results will be sent via asynchronous events. ShowSamples()
            // will be called when they all arrive.
            deviceBoard.getCorrelationCounts();
        }

        private void updateControls()
        {
            sampleCountTextBox.Visible = autoUpdateCheckBox.Checked;
            updateButton.Visible = !autoUpdateCheckBox.Checked;
            updateButton.Visible = bSamplingRequested && !autoUpdateCheckBox.Checked;

            if (bSamplingRequested)
                startStopSamplingButton.Text = "&Stop Sampling";
            else
                startStopSamplingButton.Text = "&Start Sampling";
        }

        /// Calculate tick step size. This value is intended to provide nice spacing on the Y-Axis
        /// tick marks. The labeled values should also be nice intervals.
        private uint BestTickStep(uint maxCount, uint height)
        {
            uint targetSpacing = 100;
            uint idealTickStep = targetSpacing * maxCount / height;
            uint[] tickSteps = new uint[6];
            tickSteps[0] = (uint)Math.Pow(10, (uint)Math.Log10(idealTickStep));
            tickSteps[1] = (uint)Math.Pow(10, 1 + (uint)Math.Log10(idealTickStep));
            tickSteps[2] = 2 * (uint)Math.Pow(10, (uint)Math.Log10(idealTickStep / 2));
            tickSteps[3] = 2 * (uint)Math.Pow(10, 1 + (uint)Math.Log10(idealTickStep / 2));
            tickSteps[4] = 5 * (uint)Math.Pow(10, (uint)Math.Log10(idealTickStep / 5));
            tickSteps[5] = 5 * (uint)Math.Pow(10, 1 + (uint)Math.Log10(idealTickStep / 5));
            uint tickStep = tickSteps[0];
            uint bestError = uint.MaxValue;
            for (uint i = 0; i < tickSteps.Length; i++)
            {
                uint error;
                if (tickSteps[i] > idealTickStep)
                    error = tickSteps[i] - idealTickStep;
                else
                    error = idealTickStep - tickSteps[i];
                if (error < bestError)
                {
                    tickStep = tickSteps[i];
                    bestError = error;
                }
            }

            return tickStep;
        }

        private void findMean(uint[] samples, out double mean, out double sigma, out int minimum, out int maximum)
        {
            double sum = 0.0;
            double sumSquare = 0.0;
            uint sampleCount = 0;
            minimum = 255;
            maximum = 0;

            for (int sample = 0; sample < 256; sample++)
            {
                if (samples[sample] > 0)
                {
                    if (sample < minimum)
                        minimum = sample;
                    if (sample > maximum)
                        maximum = sample;

                    sampleCount += samples[sample];
                    sum += samples[sample] * sample;
                    sumSquare += samples[sample] * sample * sample;
                }
            }

            mean = sum / sampleCount;
            double nMeanSquare = sampleCount * (mean * mean);
            sigma = Math.Sqrt((sumSquare - nMeanSquare) / sampleCount);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected override void OnPaint(PaintEventArgs args)
        {
            base.OnPaint(args);

            // Determine maximum count values.
            uint maxPresentCount = MaxSampleCount(countsPresent);
            uint maxMissingCount = MaxSampleCount(countsMissing);
            uint maxCount = (maxPresentCount > maxMissingCount) ? maxPresentCount : maxMissingCount;
            int countMaxDigits = 1 + (int)(Math.Log10(maxCount));

            Graphics g = args.Graphics;
            Font font = new Font(FontFamily.GenericMonospace, 8, FontStyle.Bold);
            int digitWidth = (int)(g.MeasureString("0", font).Width + 0.5);

            // Center the chart window in the dialog.
            int topOffset = 120;
            int bottomSpace = 80;
            int leftSpace = 10;
            int rightSpace = 20;
            int windowX = leftSpace;
            int windowY = Height - bottomSpace;
            int windowWid = Width - rightSpace - leftSpace;
            int windowHgt = Height - topOffset - bottomSpace;

            // Calculate position and size of the chart.
            int chartLeftSpace = countMaxDigits * digitWidth;
            chartBase = new Point(windowX + chartLeftSpace, windowY - 15);
            chartSize = new Point(windowWid - (chartLeftSpace + 5), windowHgt - 22);

            // Draw and outline chart window.
            g.FillRectangle(Brushes.WhiteSmoke, windowX, windowY - windowHgt, windowWid, windowHgt);
            g.DrawRectangle(Pens.Black, windowX, windowY - windowHgt, windowWid, windowHgt);

            // Draw the chart axes.
            g.DrawLine(Pens.Black, chartBase.X, chartBase.Y, chartBase.X, chartBase.Y - chartSize.Y);
            g.DrawLine(Pens.Black, chartBase.X, chartBase.Y, chartBase.X + chartSize.X, chartBase.Y);

            // Draw horizontal ticks and labels.
            const int tickLength = 5;
            uint tick;
            for (tick = 0; tick <= 256; tick += 32)
            {
                int tickX = (int)(chartBase.X + (double)tick / 256 * chartSize.X);
                g.DrawLine(Pens.Black, tickX, chartBase.Y, tickX, chartBase.Y + tickLength);
                Chart.DrawText(g, font, tickX, chartBase.Y, tick.ToString());
            }

            if (maxCount > 0)
            {
                // Calculate tick step size. This value is intended to provide nice spacing on the Y-Axis
                // tick marks. The labeled values should also be nice intervals.
                uint tickStep = BestTickStep(maxCount, (uint)chartSize.Y);

                // Draw vertical ticks and labels.
                tick = 0;
                while (true)
                {
                    int tickY = (int)(chartBase.Y - (double)tick / maxCount * chartSize.Y);
                    g.DrawLine(Pens.Black, chartBase.X - tickLength, tickY, chartBase.X, tickY);
                    Chart.DrawText(g, font, chartBase.X, tickY, tick.ToString());
                    if (tick == maxCount)
                        break;
                    tick += tickStep;
                    if (tick > maxCount - (tickStep / 5))
                        tick = maxCount;
                }

                // Update the charts and mean and standard deviation labels.
                double mean;
                double sigma;

                ShowSamples(g, Pens.Red, countsMissing, maxCount, out mean, out sigma, true, 30);
                meanNoResponseLabel.Text = string.Format("Mean: {0:0.00}", mean);
                sigmaNoResponseLabel.Text = string.Format("Sigma: {0:0.00}", sigma);

                ShowSamples(g, Pens.Green, countsPresent, maxCount, out mean, out sigma, true, 25);
                meanResponseLabel.Text = string.Format("Mean: {0:0.00}", mean);
                sigmaResponseLabel.Text = string.Format("Sigma: {0:0.00}", sigma);
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void startStopSamplingButton_Click(object sender, EventArgs e)
        {
            if (bSamplingRequested)
                stopSampling();
            else
                startSampling();
            updateControls();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            UpdateChart();
        }

        private void autoUpdateCheckBox_Click(object sender, EventArgs e)
        {
            if (bSamplingRequested)
                stopSampling();
            updateControls();
        }

        private void rbOff_CheckedChanged(object sender, EventArgs e)
        {
            clearCounts();
            stopSampling();
            updateControls();

            rbAxe1.Enabled = true;
            rbAxe2.Enabled = true;
            rbAxe3.Enabled = true;
            rbAxe4.Enabled = true;
            rbAxe4.Enabled = true;
            rbAxe5.Enabled = true;
            rbAxe6.Enabled = true;
            rbAxe7.Enabled = true;
            rbAxe8.Enabled = true;
            rbAxe9.Enabled = true;
         
            gbFrequency.Enabled = true;

        }
        private void disableRadio()
        {
            gbFrequency.Enabled = false;
            rbAxe1.Enabled = false;
            rbAxe2.Enabled = false;
            rbAxe3.Enabled = false;
            rbAxe4.Enabled = false;
            rbAxe4.Enabled = false;
            rbAxe5.Enabled = false;
            rbAxe6.Enabled = false;
            rbAxe7.Enabled = false;
            rbAxe8.Enabled = false;
            rbAxe9.Enabled = false;  
        }

        private void rbAxe1_CheckedChanged(object sender, EventArgs e)
        {
            byte AsciiRelaisValue;
            byte relaymum = 1;

            disableRadio();                  


            stopSampling();
            System.Threading.Thread.Sleep(400);
            AsciiRelaisValue = (byte)(relaymum + 0x30);
            deviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
           
            System.Threading.Thread.Sleep(500);
            deviceBoard.sendSwitchCommand(1, AsciiRelaisValue);

            clearCounts();
            updateControls();
            startSampling();
        }

        private void rbAxe2_CheckedChanged(object sender, EventArgs e)
        {
            byte AsciiRelaisValue;
            byte relaymum = 2;

            disableRadio(); 
            stopSampling();
            System.Threading.Thread.Sleep(400);
            AsciiRelaisValue = (byte)(relaymum + 0x30);
            deviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
            
            System.Threading.Thread.Sleep(500);
            deviceBoard.sendSwitchCommand(1, AsciiRelaisValue);

            clearCounts();
            updateControls();
            startSampling();
        }

        private void rbAxe3_CheckedChanged(object sender, EventArgs e)
        {
            byte AsciiRelaisValue;
            byte relaymum = 3;

            disableRadio(); 
            stopSampling();
            System.Threading.Thread.Sleep(400);
            AsciiRelaisValue = (byte)(relaymum + 0x30);
            deviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
           
            System.Threading.Thread.Sleep(500);
            deviceBoard.sendSwitchCommand(1, AsciiRelaisValue);

            clearCounts();
            updateControls();
            startSampling();
        }

        private void rbAxe4_CheckedChanged(object sender, EventArgs e)
        {
            byte AsciiRelaisValue;
            byte relaymum = 4;

            disableRadio(); 
            stopSampling();
            System.Threading.Thread.Sleep(400);
            AsciiRelaisValue = (byte)(relaymum + 0x30);
            deviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
            
            System.Threading.Thread.Sleep(500);
            deviceBoard.sendSwitchCommand(1, AsciiRelaisValue);

            clearCounts();
            updateControls();
            startSampling();
        }

        private void rbAxe5_CheckedChanged(object sender, EventArgs e)
        {
            byte AsciiRelaisValue;
            byte relaymum = 5;

            disableRadio(); 
            stopSampling();
            System.Threading.Thread.Sleep(400);
            AsciiRelaisValue = (byte)(relaymum + 0x30);
            deviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
           
            System.Threading.Thread.Sleep(500);
            deviceBoard.sendSwitchCommand(1, AsciiRelaisValue);

            clearCounts();
            updateControls();
            startSampling();
        }

        private void rbAxe6_CheckedChanged(object sender, EventArgs e)
        {
            byte AsciiRelaisValue;
            byte relaymum = 6;

            disableRadio(); 
            stopSampling();
            System.Threading.Thread.Sleep(400);
            AsciiRelaisValue = (byte)(relaymum + 0x30);
            deviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
           
            System.Threading.Thread.Sleep(500);
            deviceBoard.sendSwitchCommand(1, AsciiRelaisValue);

            clearCounts();
            updateControls();
            startSampling();
        }

        private void rbAxe7_CheckedChanged(object sender, EventArgs e)
        {
            byte AsciiRelaisValue;
            byte relaymum = 7;
            System.Threading.Thread.Sleep(400);
            stopSampling();
            disableRadio(); 
            AsciiRelaisValue = (byte)(relaymum + 0x30);
            deviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
           
            System.Threading.Thread.Sleep(500);
            deviceBoard.sendSwitchCommand(1, AsciiRelaisValue);

            clearCounts();
            updateControls();
            startSampling();
        }

        private void rbAxe8_CheckedChanged(object sender, EventArgs e)
        {
            byte AsciiRelaisValue;
            byte relaymum = 8;

            disableRadio(); 
            stopSampling();
            System.Threading.Thread.Sleep(400);
            AsciiRelaisValue = (byte)(relaymum + 0x30);
            deviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
           
            System.Threading.Thread.Sleep(500);
            deviceBoard.sendSwitchCommand(1, AsciiRelaisValue);

            clearCounts();
            updateControls();
            startSampling();
        }

        private void rbAxe9_CheckedChanged(object sender, EventArgs e)
        {
            byte AsciiRelaisValue;
            byte relaymum = 9;

            disableRadio(); 
            stopSampling();
            System.Threading.Thread.Sleep(400);
            AsciiRelaisValue = (byte)(relaymum + 0x30);
            deviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
           
            System.Threading.Thread.Sleep(500);
            deviceBoard.sendSwitchCommand(1, AsciiRelaisValue);

            clearCounts();
            updateControls();
            startSampling();
        }

        private void GetFreq()
        {
            UInt16 period, Vant;
            int freq;
            deviceBoard.GetCarrierFrequency(out period, out Vant);
            if (freqTab.ContainsKey(period))
                freq = freqTab[period];
            else
                freq = -1;
            CarrierPeriod = period;
            string str = string.Format("F0 : {0} Hz - ({1})", (int)freq, period);
            labelFreq.Text = str;
            string str2 = string.Format("Vant : {0} ", Vant);
            labelVant.Text = str2;
        }

        private void saveToRomButton_Click(object sender, EventArgs e)
        {
            deviceBoard.saveBridgeDutyCyclesToROM();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            deviceBoard.FindGoodFrequency();
            GetFreq();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (CarrierPeriod < 338)
            {
                deviceBoard.DecreaseCarrierFrequency();
                GetFreq();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (CarrierPeriod > 318)
            {
                deviceBoard.IncreaseCarrierFrequency();
                GetFreq();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte level = 0;

            if (byte.TryParse(correlationThresholdTextBox.Text, out level))
            {
                deviceBoard.setCorrelationThreshold(level);
                deviceBoard.saveCorrelationThresholdToROM();
            }
        }

        private void FindThresholdDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            clearCounts();
            stopSampling();
            updateControls();
        }
    }
}
