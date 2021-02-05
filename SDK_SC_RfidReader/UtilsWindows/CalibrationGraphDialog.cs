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
    public partial class CalibrationGraphDialog : Form
    {
        const uint SignalValueCount = 256;
        private int[] voltageSignal = new int[SignalValueCount];
        private int[] currentSignal = new int[SignalValueCount];
        readonly private DeviceRfidBoard deviceBoard;

        bool bHalfBridge;
        uint fullBridgeDutyCycle;
        uint halfBridgeDutyCycle;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceBoard"></param>
        public CalibrationGraphDialog(DeviceRfidBoard deviceBoard)
        {
            this.deviceBoard = deviceBoard;

            deviceBoard.getBridgeState(out bHalfBridge, out fullBridgeDutyCycle, out halfBridgeDutyCycle);
            
                      

            InitializeComponent();

            dutyCycleTrackBar.Maximum = DeviceRfidBoard.MAX_DutyCycle;
            dutyCycleTrackBar.Value = (int)(bHalfBridge ? halfBridgeDutyCycle : fullBridgeDutyCycle);
            dutyCycleTextBox.Text = dutyCycleTrackBar.Value.ToString();
            if (bHalfBridge)
                halfBridgeRadioButton.Checked = true;
            else
                fullBridgeRadioButton.Checked = true;


            cbxVoltage.Checked = true;
            cbxCurrent.Checked = false;

            if (deviceBoard.HardwareVersion.StartsWith("6.")) // SBR 4 axes
            {
                rbAxe4.Enabled = true;
            }
            if (deviceBoard.HardwareVersion.StartsWith("2.")) // JSC 9 axes
            {
                rbAxe4.Enabled = true;
                rbAxe5.Enabled = true;
                rbAxe6.Enabled = true;
                rbAxe7.Enabled = true;
                rbAxe8.Enabled = true;
                rbAxe9.Enabled = true;
            }

        }

        private void UpdateDutyCycle()
        {
            dutyCycleTrackBar.Visible = true;
            dutyCycleLabel.Visible = true;
            dutyCycleTextBox.Visible = true;

            uint bridgeDutyCycle = bHalfBridge ? halfBridgeDutyCycle : fullBridgeDutyCycle;
            dutyCycleTrackBar.Value = (int)bridgeDutyCycle;
            dutyCycleTextBox.Text = dutyCycleTrackBar.Value.ToString();

            deviceBoard.setBridgeState(bHalfBridge, fullBridgeDutyCycle, halfBridgeDutyCycle);
            //updateBridgeStateDelegate(bHalfBridge, fullBridgeDutyCycle, halfBridgeDutyCycle);
        }

        private void UpdateDutyCycle(uint bridgeDutyCycle)
        {
            if (bridgeDutyCycle > DeviceRfidBoard.MAX_DutyCycle)
                bridgeDutyCycle = DeviceRfidBoard.MAX_DutyCycle;
            if (bHalfBridge)
                halfBridgeDutyCycle = bridgeDutyCycle;
            else
                fullBridgeDutyCycle = bridgeDutyCycle;

            UpdateDutyCycle();
        }
        private void OnDutyCycleTrackBarScroll(object sender, EventArgs e)
        {
            UpdateDutyCycle((uint)dutyCycleTrackBar.Value);
        }

        private void OnDutyCycleTextChanged(object sender, EventArgs e)
        {
            uint dutyCycle;
            if (uint.TryParse(dutyCycleTextBox.Text, out dutyCycle))
                UpdateDutyCycle(dutyCycle);
        }

        private void OnSaveToRomClick(object sender, EventArgs e)
        {
            deviceBoard.saveBridgeDutyCyclesToROM();
        }

        private void OnHalfFullBridgeClick(object sender, EventArgs e)
        {
            bHalfBridge = halfBridgeRadioButton.Checked;
            UpdateDutyCycle();
        }


        private void calibrateTimer_Tick(object sender, EventArgs e)
        {
            deviceBoard.calibrate(true); // The calibrate mode must be pinged within every five seconds to continue.
            UpdateSignal();
        }

        private void CalibrationGraphDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            calibrateTimer.Enabled = false;
            deviceBoard.calibrate(false);
            base.OnClosing(e);
        }

        private void collectSignalSamples(int[] voltageSignal, int[] currentSignal)
        {
            sbyte[] values = new sbyte[32];
            bool bLastChunk = false;
            for (uint pass = 0; pass < 2; pass++)
            {
                int[] signal = (pass == 0) ? voltageSignal : currentSignal;
                if (signal == null)
                    continue;

                uint sampleIndex = 0;
                while (sampleIndex < signal.Length)
                {
                    if (sampleIndex + values.Length >= signal.Length)
                        bLastChunk = (pass == 1) || (currentSignal == null);

                    // Request the carrier voltage/current signal.
                    bool bGetVoltage = (pass == 0);
                    if (!deviceBoard.getCarrierSignal((ushort)sampleIndex, !bLastChunk,
                        pass == 0, values))
                    {
                        MyDebug.Fail("Response failed.");
                    }

                    for (uint i = 0; i < 32; i++)
                    {
                     
                            signal[sampleIndex++] = values[i];                       
                        if (sampleIndex >= signal.Length)
                            break;
                    }
                }             

            }
        }
        private void UpdateSignal()
        {
           
           
             if (cbxCurrent.Checked)
                    collectSignalSamples(voltageSignal, currentSignal);
             else
                    collectSignalSamples(voltageSignal, null);
                  
            chartPanel.Invalidate();
         }

        private bool bfirst = true;
        private void chartPanel_Paint(object sender, PaintEventArgs args)
        {
            base.OnPaint(args);
            // Skip 1st draw
           

            Chart chart = new Chart(args.Graphics, (Panel)sender);

            double xScale;
            double yScale;
            double zeroX;
            double zeroY;

            int maxSig = 0;
            int max;
            foreach (int val in voltageSignal)
            {
                if (Math.Abs(val) > maxSig) maxSig = Math.Abs(val);
            }
            if (maxSig < 118)
                max = maxSig + 10;
            else
                max = 128;

            chart.DrawChartFrame(0, voltageSignal.Length, 32, false, -max, max, (int) (max/2.0), true,
                out xScale, out yScale, out zeroX, out zeroY);

            // Display voltage maximum and minimum.
            int minVoltage;
            int maxVoltage;
            Chart.GetMinMax(voltageSignal, out minVoltage, out maxVoltage);
            minMaxLabel.Text = string.Format("Min, Max, P-P: {0}, {1}, {2}", minVoltage, maxVoltage, maxVoltage - minVoltage);

            if (bfirst)
            {
                bfirst = false;
                return;
            }
            // Draw the voltage and current signal chart of the carrier signal.
            if (cbxVoltage.Checked)
                Chart.DrawGraph(false, maxVoltage, args.Graphics, Pens.Green, voltageSignal, chart.BaseX, zeroY, xScale, yScale);
            if (cbxCurrent.Checked)
                Chart.DrawGraph(true, maxVoltage, args.Graphics, Pens.Red, currentSignal, chart.BaseX, zeroY, xScale, yScale); 

        }

        private void butClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void rbOff_CheckedChanged(object sender, EventArgs e)
        {
            calibrateTimer.Enabled = false;
            deviceBoard.calibrate(false);
        }

        private void rbAxe1_CheckedChanged(object sender, EventArgs e)
        {
            byte AsciiRelaisValue;
            byte relaymum = 1;

            calibrateTimer.Enabled = false;
            deviceBoard.calibrate(false);
            AsciiRelaisValue = (byte)(relaymum + 0x30);
            deviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
            System.Threading.Thread.Sleep(400);
            System.Threading.Thread.Sleep(500);
            deviceBoard.sendSwitchCommand(1, AsciiRelaisValue);

            deviceBoard.calibrate(true);
            UpdateSignal();
            calibrateTimer.Enabled = true;
        }

        private void rbAxe2_CheckedChanged(object sender, EventArgs e)
        {
            byte AsciiRelaisValue;
            byte relaymum = 2;

            calibrateTimer.Enabled = false;
            deviceBoard.calibrate(false);
            AsciiRelaisValue = (byte)(relaymum + 0x30);
            deviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
            System.Threading.Thread.Sleep(400);
            System.Threading.Thread.Sleep(500);
            deviceBoard.sendSwitchCommand(1, AsciiRelaisValue);

            deviceBoard.calibrate(true);
            UpdateSignal();
            calibrateTimer.Enabled = true;
        }

        private void rbAxe3_CheckedChanged(object sender, EventArgs e)
        {
            byte AsciiRelaisValue;
            byte relaymum = 3;

            calibrateTimer.Enabled = false;
            deviceBoard.calibrate(false);
            AsciiRelaisValue = (byte)(relaymum + 0x30);
            deviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
            System.Threading.Thread.Sleep(400);
            System.Threading.Thread.Sleep(500);
            deviceBoard.sendSwitchCommand(1, AsciiRelaisValue);

            deviceBoard.calibrate(true);
            UpdateSignal();
            calibrateTimer.Enabled = true;
        }

        private void rbAxe4_CheckedChanged(object sender, EventArgs e)
        {
            byte AsciiRelaisValue;
            byte relaymum = 4;

            calibrateTimer.Enabled = false;
            deviceBoard.calibrate(false);
            AsciiRelaisValue = (byte)(relaymum + 0x30);
            deviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
            System.Threading.Thread.Sleep(400);
            System.Threading.Thread.Sleep(500);
            deviceBoard.sendSwitchCommand(1, AsciiRelaisValue);

            deviceBoard.calibrate(true);
            UpdateSignal();
            calibrateTimer.Enabled = true;
        }

        private void rbAxe5_CheckedChanged(object sender, EventArgs e)
        {
            byte AsciiRelaisValue;
            byte relaymum = 5;

            calibrateTimer.Enabled = false;
            deviceBoard.calibrate(false);
            AsciiRelaisValue = (byte)(relaymum + 0x30);
            deviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
            System.Threading.Thread.Sleep(400);
            System.Threading.Thread.Sleep(500);
            deviceBoard.sendSwitchCommand(1, AsciiRelaisValue);

            deviceBoard.calibrate(true);
            UpdateSignal();
            calibrateTimer.Enabled = true;
        }

        private void rbAxe6_CheckedChanged(object sender, EventArgs e)
        {
            byte AsciiRelaisValue;
            byte relaymum = 6;

            calibrateTimer.Enabled = false;
            deviceBoard.calibrate(false);
            AsciiRelaisValue = (byte)(relaymum + 0x30);
            deviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
            System.Threading.Thread.Sleep(400);
            System.Threading.Thread.Sleep(500);
            deviceBoard.sendSwitchCommand(1, AsciiRelaisValue);

            deviceBoard.calibrate(true);
            UpdateSignal();
            calibrateTimer.Enabled = true;
        }

        private void rbAxe7_CheckedChanged(object sender, EventArgs e)
        {
            byte AsciiRelaisValue;
            byte relaymum = 7;

            calibrateTimer.Enabled = false;
            deviceBoard.calibrate(false);
            AsciiRelaisValue = (byte)(relaymum + 0x30);
            deviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
            System.Threading.Thread.Sleep(400);
            System.Threading.Thread.Sleep(500);
            deviceBoard.sendSwitchCommand(1, AsciiRelaisValue);

            deviceBoard.calibrate(true);
            UpdateSignal();
            calibrateTimer.Enabled = true;
        }

        private void rbAxe8_CheckedChanged(object sender, EventArgs e)
        {
            byte AsciiRelaisValue;
            byte relaymum = 8;

            calibrateTimer.Enabled = false;
            deviceBoard.calibrate(false);
            AsciiRelaisValue = (byte)(relaymum + 0x30);
            deviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
            System.Threading.Thread.Sleep(400);
            System.Threading.Thread.Sleep(500);
            deviceBoard.sendSwitchCommand(1, AsciiRelaisValue);

            deviceBoard.calibrate(true);
            UpdateSignal();
            calibrateTimer.Enabled = true;
        }

        private void rbAxe9_CheckedChanged(object sender, EventArgs e)
        {
            byte AsciiRelaisValue;
            byte relaymum = 9;

            calibrateTimer.Enabled = false;
            deviceBoard.calibrate(false);
            AsciiRelaisValue = (byte)(relaymum + 0x30);
            deviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
            System.Threading.Thread.Sleep(400);
            System.Threading.Thread.Sleep(500);           
            deviceBoard.sendSwitchCommand(1, AsciiRelaisValue);

            deviceBoard.calibrate(true);
            UpdateSignal();
            calibrateTimer.Enabled = true;
        }

        


    }

    class Chart
    {
        readonly Graphics graphics;
        readonly Font font;
        readonly Point chartBase;
        readonly Point chartSize;

        public Chart(Graphics graphics, Panel panel)
        {
            this.graphics = graphics;

            font = new Font(FontFamily.GenericMonospace, 8, FontStyle.Bold);
            int digitWidth = (int)(graphics.MeasureString("0", font).Width + 0.5);

            // Calculate position and size of the chart.
            int chartLeftSpace = 3 * digitWidth;
            chartBase = new Point(chartLeftSpace, panel.Height - 15);
            chartSize = new Point(panel.Width - (chartLeftSpace + 5), panel.Height - 20);

            // Clear and outline chart panel.
            graphics.FillRectangle(Brushes.WhiteSmoke, 0, 0, panel.Width, panel.Height);
            graphics.DrawRectangle(Pens.Black, 0, 0, panel.Width - 1, panel.Height - 1);
        }

        public int BaseX { get { return chartBase.X; } }
        public int BaseY { get { return chartBase.Y; } }
        public int SizeX { get { return chartSize.X; } }
        public int SizeY { get { return chartSize.Y; } }

        public static void DrawText(Graphics graphics, Font font, int x, int y, string text)
        {
            SizeF size = graphics.MeasureString(text, font);
            graphics.DrawString(text, font, Brushes.Black, x - size.Width, y);
        }

        public void DrawVLine(Pen pen, int x, double zeroX, double zeroY, double xScale, double yScale)
        {
            int lineX = (int)(zeroX + (double)x * xScale);
            graphics.DrawLine(pen, lineX, chartBase.Y, lineX, chartBase.Y - chartSize.Y);
        }

        public void DrawChartFrame(
            int minX, int maxX, int tickIntervalX, bool bShowXLines, int minY, int maxY, int tickIntervalY, bool bShowYLines,
            out double xScale, out double yScale, out double zeroX, out double zeroY)
        {
            xScale = (double)chartSize.X / (maxX - minX);
            yScale = (double)chartSize.Y / (maxY - minY);
            zeroX = chartBase.X - xScale * minX;
            zeroY = chartBase.Y + yScale * minY;

            // Draw horizontal ticks and labels.
            const int tickLength = 5;
            int tick = (minX / tickIntervalX) * tickIntervalX;
            while (tick <= maxX)
            {
                int tickX = (int)(zeroX + (double)tick * xScale);
                if (bShowXLines)
                    graphics.DrawLine(Pens.LightGray, tickX, chartBase.Y, tickX, chartBase.Y - chartSize.Y);
                graphics.DrawLine(Pens.Black, tickX, chartBase.Y, tickX, chartBase.Y + tickLength);
                DrawText(graphics, font, tickX, chartBase.Y, tick.ToString());
                tick += tickIntervalX;
            }

            // Draw vertical ticks and labels.
            tick = (minY / tickIntervalY) * tickIntervalY;
            while (tick <= maxY)
            {
                int tickY = (int)(zeroY - yScale * (double)tick);
                if (bShowYLines)
                    graphics.DrawLine(Pens.LightGray, chartBase.X, tickY, chartBase.X + chartSize.X, tickY);
                graphics.DrawLine(Pens.Black, chartBase.X, tickY, chartBase.X - tickLength, tickY);
                DrawText(graphics, font, chartBase.X, tickY, tick.ToString());
                tick += tickIntervalY;
            }

            // Draw the chart axes.
            graphics.DrawLine(Pens.Black, chartBase.X, chartBase.Y, chartBase.X, chartBase.Y - chartSize.Y);
            graphics.DrawLine(Pens.Black, chartBase.X, chartBase.Y, chartBase.X + chartSize.X, chartBase.Y);
        }      

       


        public static void DrawGraph(bool bCurrent,int max , Graphics graphics, Pen pen, int[] signal, double zeroX, double zeroY, double xScale, double yScale)
        {
            try
            {
                int kf = 10;
                int nbEchan = 30;
                Point[] points = new Point[nbEchan];

                for (int signalIndex = 0; signalIndex < nbEchan; signalIndex++)
                {
                    int signalX = (int)(zeroX + kf * xScale * signalIndex);
                    int signalY = (int)(zeroY - yScale * signal[signalIndex]);
                    points[signalIndex] = new Point(signalX, signalY);
                }
                graphics.DrawCurve(Pens.Red, points, 0, nbEchan, 0.9f);
            }
            catch 
            {
                int lastX = 0;
                int lastY = 0;
                for (int signalIndex = 0; signalIndex < signal.Length; signalIndex++)
                {
                    int signalX = (int)(zeroX + xScale * signalIndex);
                    int signalY = (int)(zeroY - yScale * signal[signalIndex]);

                    if (signalIndex > 0)
                        graphics.DrawLine(pen, lastX, lastY, signalX, signalY);

                    lastX = signalX;
                    lastY = signalY;
                }
            }

          
        }

       

       

        public void DrawSamples(Graphics graphics, Pen pen, double[] samples, double xScale, double yScale)
        {
            for (int sampleIndex = 0; sampleIndex < samples.Length; sampleIndex++)
            {
                if (samples[sampleIndex] != 0)
                {
                    int sampleX = chartBase.X + (int)(xScale * sampleIndex);
                    int sampleY = chartBase.Y - (int)(yScale * samples[sampleIndex]);
                    graphics.DrawLine(pen, sampleX - 5, sampleY, sampleX + 5, sampleY);
                    graphics.DrawLine(pen, sampleX, sampleY - 5, sampleX, sampleY + 5);
                }
            }
        }

        public static void GetMinMax(int[] signal, out int minVoltage, out int maxVoltage)
        {
            minVoltage = int.MaxValue;
            maxVoltage = int.MinValue;
            foreach (int value in signal)
            {
                if (value < minVoltage)
                    minVoltage = value;
                if (value > maxVoltage)
                    maxVoltage = value;
            }
        }

       /* public void DrawResponseGraph(int[] tagResponseSignal, Correlation correlationCalculator, int AntennaCarrier)
        {
            const int minY = -135;
            const int maxY = 135;

            double xScale;
            double yScale;
            double zeroX;
            double zeroY;
            DrawChartFrame(0, tagResponseSignal.Length, 50, false, minY, maxY, 64, true,
                out xScale, out yScale, out zeroX, out zeroY);

            // Draw cycle lines to help visualize phase shifts.
            int cycle = 1;
            while (true)
            {
                Correlation cor = new Correlation(AntennaCarrier);
                int cycleX = (int)(zeroX + xScale * ((double)cycle * cor.ReferenceSignalPeriod / Correlation.SamplePeriod));
                if (cycleX > BaseX + SizeX)
                    break;
                graphics.DrawLine(Pens.LightGray, cycleX, BaseY - 1, cycleX, BaseY - SizeY);
                cycle++;
            }

            // Draw phase shift graph.
            int[] phaseShiftSeries = correlationCalculator.PhaseShiftSeries;
            double yPhaseScale = yScale * 128.0 / 180.0;
            DrawGraph(graphics, Pens.HotPink, phaseShiftSeries, zeroX, zeroY, xScale, yPhaseScale);

            // Draw average phase shift graph.
            int[] phaseShiftAverageSeries = correlationCalculator.PhaseShiftAverageSeries;
            DrawGraph(graphics, Pens.Blue, phaseShiftAverageSeries, zeroX, zeroY, xScale, yPhaseScale);

            // Draw signal points.
            DrawGraph(graphics, Pens.Green, tagResponseSignal, zeroX, zeroY, xScale, yScale);
        }*/

        /*public void DrawIterativeCorrelationsGraph(Correlation correlationCalculator)
        {
            double xScale;
            double yScale;
            double zeroX;
            double zeroY;
            DrawChartFrame(0, Correlation.SampleCount, 50, false, -135, 135, 64, true,
                out xScale, out yScale, out zeroX, out zeroY);

            // Draw iterative correlation phase charts.
            Pen[] colors = new Pen[] { Pens.Red, Pens.Orange, Pens.Green, Pens.Blue };
            for (uint phase = 0; phase < 4; phase++)
                DrawGraph(graphics, colors[phase], correlationCalculator.GetPhaseCorrelations(phase), zeroX, zeroY, xScale, yScale);

            // Draw iterations totals correlation chart.
            DrawGraph(graphics, Pens.Black, correlationCalculator.GetCorrelationTotals(), BaseX, BaseY, xScale, yScale);
        }*/
    }

    class Correlation
    {
        public const int SampleCount = 435;      
        public const int SamplePeriod = 96; // Trigger ADC2 every 2.4us.
       
        const uint SPARE_Samples = 3; // Extra samples needed in ReferenceSignal for correlation.
        const uint SIN_Elements = 256;
        const int AveragePower = 5;
        const int AverageItems = (1 << AveragePower);

        public int AntennaCarrierPeriod = 334; // 40MHz / AntennaCarrierPeriod = 119.760kHz.
        //public int ReferenceSignalPeriod = (2 * AntennaCarrierPeriod);
        public uint ReferenceSignalPeriod;
        static readonly int[] ReferenceSignal = new int[SampleCount + SPARE_Samples];

        private int[][] phaseCorrelations = new int[4][] { new int[SampleCount], new int[SampleCount], new int[SampleCount], new int[SampleCount] };
        private int[] correlationsTotal = new int[SampleCount];
        private int[] phaseShiftSeries = new int[SampleCount];
        private int[] phaseShiftAverageSeries = new int[SampleCount];

        /*static Correlation(int AntennaCarrierPeriod)
        {
            GenerateReferenceSignal(AntennaCarrierPeriod);
        }*/

        public Correlation(int AntennaCarrierPeriod)
        {
            this.AntennaCarrierPeriod = AntennaCarrierPeriod;
            ReferenceSignalPeriod = (uint)(2 * AntennaCarrierPeriod);
        }

        public int[] GetPhaseCorrelations(uint phase)
        {
            MyDebug.Assert(phase < 4);
            return phaseCorrelations[phase];
        }

        public int[] GetCorrelationTotals()
        {
            return correlationsTotal;
        }

        public int[] PhaseShiftSeries
        {
            get { return phaseShiftSeries; }
        }

        public int[] PhaseShiftAverageSeries
        {
            get { return phaseShiftAverageSeries; }
        }

        public int FinalPhaseShift
        {
            get { return phaseShiftSeries[phaseShiftSeries.Length - 1]; }
        }

        public int FinalPhaseShiftAverage
        {
            get { return phaseShiftAverageSeries[phaseShiftAverageSeries.Length - 1]; }
        }

        public uint Calculate(int[] signal)
        {
            int totalAccumulator = 0;

            // For each phase, calculate the accumulated correlation for each sample in the tag
            // response signal.
            for (uint phase = 0; phase < 4; phase++)
            {
                int accumulator = 0;
                for (uint sampleIndex = 0; sampleIndex < signal.Length; sampleIndex++)
                {
                    accumulator += signal[sampleIndex] * ReferenceSignal[sampleIndex + phase];
                    phaseCorrelations[phase][sampleIndex] = accumulator;
                }
                totalAccumulator += (accumulator < 0) ? -accumulator : accumulator;
            }

            // Add the absolute values of the iterative correlation values at each sample in the tag
            // response signal to calculate a total correlation value for each sample.
            for (uint sampleIndex = 0; sampleIndex < SampleCount; sampleIndex++)
            {
                correlationsTotal[sampleIndex] = 0;
                for (uint phase = 0; phase < 4; phase++)
                {
                    int phaseCorrelation = phaseCorrelations[phase][sampleIndex];
                    if (phaseCorrelation < 0)
                        phaseCorrelation = -phaseCorrelation;
                    correlationsTotal[sampleIndex] += phaseCorrelation;
                    phaseCorrelations[phase][sampleIndex] >>= 15;
                }
                correlationsTotal[sampleIndex] >>= 15;
            }

            // Calculate the tag response phase shift for each sample. Use the zero crossing points to
            // determine the phase shift.
            bool bWasAbove = (signal[0] > 0);
            double expectedPeriod = (double)ReferenceSignalPeriod / (double)SamplePeriod;
            uint previousIndex = 0;
            double phaseShiftAngle = 0;
            double previousPhaseShiftAngle = 0;
            int currentSample = 0;
            int previousSample = 0;
            int phaseShiftSum = 0;
            for (uint sampleIndex = 1; sampleIndex < signal.Length; sampleIndex++)
            {
                previousSample = currentSample;
                currentSample = signal[sampleIndex];
                if (currentSample <= -128)
                    currentSample = -127;
                bool bAbove = (currentSample > 0);
                double crossing;
                if (currentSample == 0)
                    crossing = (double)sampleIndex;
                else if (bAbove != bWasAbove)
                {
                    double previous = (double)previousSample;
                    double current = (double)currentSample;
                    crossing = (double)(sampleIndex - 1) + previous / (previous - current);
                }
                else
                    continue;

                bWasAbove = !bWasAbove;

                double halfCycleIfRising = bWasAbove ? expectedPeriod / 2 : 0.0;
                double expectedCrossing = Math.Round((crossing - halfCycleIfRising) / expectedPeriod) * expectedPeriod + halfCycleIfRising;
                phaseShiftAngle = 360.0 * (crossing - expectedCrossing) / expectedPeriod;

                // Interpolate for the points since the last crossing.
                double pitch = 0.0;
                if ((previousIndex > 0) && (sampleIndex > previousIndex))
                {
                    if ((Math.Abs(phaseShiftAngle) > 160) && (Math.Abs(previousPhaseShiftAngle) > 160) &&
                        ((phaseShiftAngle > 0) != (previousPhaseShiftAngle > 0)))
                    {
                        phaseShiftAngle += (phaseShiftAngle > 0) ? -360.0 : 360.0;
                    }
                    pitch = (phaseShiftAngle - previousPhaseShiftAngle) / (double)(sampleIndex - previousIndex);
                }
                while (previousIndex <= sampleIndex)
                {
                    double phaseShift = phaseShiftAngle - pitch * (sampleIndex - previousIndex);
                    if (Math.Abs(phaseShift) > 190.0)
                    {
                        double adjust = ((phaseShift < 0) ? 360.0 : -360.0);
                        phaseShift += adjust;
                        phaseShiftAngle += adjust;
                        previousPhaseShiftAngle += adjust;
                    }
                    int phaseShiftInt = (int)Math.Round(phaseShift);
                    phaseShiftSeries[previousIndex] = phaseShiftInt;
                    if (previousIndex == 0)
                        phaseShiftSum = AverageItems * phaseShiftInt;
                    phaseShiftSum += phaseShiftInt;
                    if (previousIndex > AverageItems)
                        phaseShiftSum -= phaseShiftSeries[previousIndex - AverageItems];
                    else
                        phaseShiftSum -= phaseShiftSeries[0];
                    phaseShiftAverageSeries[previousIndex++] = (phaseShiftSum >> AveragePower);
                }
                previousPhaseShiftAngle = phaseShiftAngle;
            }

            while (previousIndex < SampleCount)
            {
                phaseShiftSeries[previousIndex] = (int)Math.Round(phaseShiftAngle);
                phaseShiftAverageSeries[previousIndex] = phaseShiftAverageSeries[previousIndex - 1];
                previousIndex++;
            }

            // The firmware limits the correlation to 254.
            uint correlation = (uint)totalAccumulator >> 15;
            if (correlation > 254)
                correlation = 254;

            return correlation;
        }

        /// <summary>
        /// Calculate the final phase shift average using only integer calculations. This is intended to be
        /// ported to firmware.
        /// </summary>
        /// <param name="signal">The array of signal samples.</param>
        /// <returns>The average of the final AverageItems samples.</returns>
        public int CalculateFinalPhaseShiftAverage(int[] signal)
        {
            // Calculate the tag response phase shift for each sample using the zero crossing points.
            int FixedPointFactor = SamplePeriod * 16;
            int crossingCount = 0;
            int expectedPeriod =(int) (FixedPointFactor * ReferenceSignalPeriod / SamplePeriod);
            int phaseShiftSum = 0;
            int startAverageIndex = signal.Length - AverageItems;
            int sampleIndex = (int) (startAverageIndex - (ReferenceSignalPeriod / SamplePeriod));
            int currentSample = signal[sampleIndex - 1];
            int previousSample;
            bool bWasAbove = (currentSample > 0);
            int firstPhaseShiftAngle = 0;
            for (; sampleIndex < signal.Length; sampleIndex++)
            {
                previousSample = currentSample;
                currentSample = signal[sampleIndex];
                if (currentSample <= -128)
                    currentSample = -127;
                bool bAbove = (currentSample > 0);
                if ((currentSample == 0) || (bAbove != bWasAbove))
                {
                    int crossing = FixedPointFactor * sampleIndex;
                    if (currentSample != 0)
                        crossing -= (FixedPointFactor * currentSample) / (currentSample - previousSample);

                    bWasAbove = !bWasAbove;

                    // Add the phases.
                    if (sampleIndex >= startAverageIndex)
                    {
                        int halfCycleIfRising = bWasAbove ? expectedPeriod / 2 : 0;
                        int expectedCrossing = ((crossing - halfCycleIfRising) / expectedPeriod) * expectedPeriod + halfCycleIfRising;
                        int phaseShiftAngle = (360 * (crossing - expectedCrossing) + (expectedPeriod / 2)) / expectedPeriod;
                        if (crossingCount == 0)
                            firstPhaseShiftAngle = phaseShiftAngle;
                        else if (firstPhaseShiftAngle < phaseShiftAngle)
                        {
                            if (phaseShiftAngle - firstPhaseShiftAngle > 300)
                                phaseShiftAngle -= 360;
                        }
                        else
                        {
                            if (firstPhaseShiftAngle - phaseShiftAngle > 300)
                                phaseShiftAngle += 360;
                        }
                        phaseShiftSum += phaseShiftAngle;
                        crossingCount++;
                    }
                }
            }

            int phaseShiftAverage = (phaseShiftSum / crossingCount) % 360;
            if (phaseShiftAverage > 180)
                phaseShiftAverage -= 360;

            return phaseShiftAverage;
        }

        //-------------------------------------------------------------------------------------------------
        // Generate a reference signal. The hardware originally generated this by putting a 60kHz sine wave
        // through the filtering circuit, but that portion of the hardware has been eliminated to simplify
        // the circuit and reduce noise in the filtering circuitry. The reference signal values are
        // determined by the following factors:
        // o Carrier period: AntennaCarrierPeriod 334
        // o Sample period:  SamplePeriod 96
        // The reference signal period is twice AntennaCarrierPeriod.
        private static readonly int[] SinTable = new int[]
		{
			  0,   1,   1,   2,   3,   4,   4,   5,   6,   7,   7,   8,   9,  10,  10,  11,
			 12,  12,  13,  14,  15,  15,  16,  17,  18,  18,  19,  20,  21,  21,  22,  23,
			 23,  24,  25,  26,  26,  27,  28,  28,  29,  30,  31,  31,  32,  33,  33,  34,
			 35,  36,  36,  37,  38,  38,  39,  40,  40,  41,  42,  42,  43,  44,  45,  45,
			 46,  47,  47,  48,  49,  49,  50,  51,  51,  52,  53,  53,  54,  55,  55,  56,
			 57,  57,  58,  59,  59,  60,  60,  61,  62,  62,  63,  64,  64,  65,  65,  66,
			 67,  67,  68,  68,  69,  70,  70,  71,  71,  72,  73,  73,  74,  74,  75,  76,
			 76,  77,  77,  78,  78,  79,  79,  80,  81,  81,  82,  82,  83,  83,  84,  84,
			 85,  85,  86,  86,  87,  87,  88,  88,  89,  89,  90,  90,  91,  91,  92,  92,
			 93,  93,  94,  94,  95,  95,  96,  96,  96,  97,  97,  98,  98,  99,  99,  99,
			100, 100, 101, 101, 101, 102, 102, 103, 103, 103, 104, 104, 104, 105, 105, 105,
			106, 106, 107, 107, 107, 108, 108, 108, 108, 109, 109, 109, 110, 110, 110, 111,
			111, 111, 111, 112, 112, 112, 112, 113, 113, 113, 113, 114, 114, 114, 114, 115,
			115, 115, 115, 115, 116, 116, 116, 116, 116, 117, 117, 117, 117, 117, 117, 118,
			118, 118, 118, 118, 118, 118, 118, 119, 119, 119, 119, 119, 119, 119, 119, 119,
			119, 119, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120, 120,
			120
		};

        /// <summary>
        /// Generate a sine wave.
        /// </summary>
        /// <param name="signal">Array to place reference signal values</param>
        /// <param name="startAngle">Phase of the first signal value</param>
        /// <param name="scale">The length of a period in signal values</param>
        public  void GenerateSignal(int[] signal, uint startAngle, double scale)
        {
            uint sampleIndex;
            uint sampleAngle = (startAngle % 360) * ReferenceSignalPeriod / 360; // This over ReferenceSignalPeriod is the fraction of 360 degrees.
            for (sampleIndex = 0; sampleIndex < signal.Length; sampleIndex++)
            {
                uint sinIndex = sampleAngle * 4 * SIN_Elements / ReferenceSignalPeriod;
                bool bNegative = false;
                if (sinIndex > 2 * SIN_Elements)
                {
                    bNegative = true;
                    sinIndex -= 2 * SIN_Elements;
                }
                if (sinIndex > SIN_Elements)
                    sinIndex = 2 * SIN_Elements - sinIndex;
                int sinValue = SinTable[sinIndex];
                sinValue = (int)Math.Round((double)sinValue * scale);
                if (bNegative)
                    sinValue = -Math.Min(128, sinValue);
                else
                    sinValue = Math.Min(127, sinValue);
                signal[sampleIndex] = sinValue;

                sampleAngle += SamplePeriod;
                if (sampleAngle > ReferenceSignalPeriod)
                    sampleAngle -= ReferenceSignalPeriod;
            }
        }

        /// <summary>
        /// Generate the reference signal used to calculate correlation values. This routine matches
        /// the reference signal generation in the firmware.
        /// </summary>
        private  void GenerateReferenceSignal()
        {
            uint sampleIndex;
            uint sampleAngle = 0; // This over ReferenceSignalPeriod is the fraction of 360 degrees.
            for (sampleIndex = 0; sampleIndex < ReferenceSignal.Length; sampleIndex++)
            {
                uint sinIndex = sampleAngle * 4 * SIN_Elements / ReferenceSignalPeriod;
                bool bNegative = false;
                if (sinIndex > 2 * SIN_Elements)
                {
                    bNegative = true;
                    sinIndex -= 2 * SIN_Elements;
                }
                if (sinIndex > SIN_Elements)
                    sinIndex = 2 * SIN_Elements - sinIndex;
                int sinValue = SinTable[sinIndex];
                ReferenceSignal[sampleIndex] = (bNegative ? -sinValue : sinValue);

                sampleAngle += SamplePeriod;
                if (sampleAngle > ReferenceSignalPeriod)
                    sampleAngle -= ReferenceSignalPeriod;
            }
        }
    }
}
