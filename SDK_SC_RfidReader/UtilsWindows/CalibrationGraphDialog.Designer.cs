namespace SDK_SC_RfidReader.UtilsWindows
{
    partial class CalibrationGraphDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.chartPanel = new System.Windows.Forms.Panel();
            this.cbxVoltage = new System.Windows.Forms.CheckBox();
            this.cbxCurrent = new System.Windows.Forms.CheckBox();
            this.butClose = new System.Windows.Forms.Button();
            this.calibrateTimer = new System.Windows.Forms.Timer(this.components);
            this.minMaxLabel = new System.Windows.Forms.Label();
            this.dutyCycleTrackBar = new System.Windows.Forms.TrackBar();
            this.dutyCycleTextBox = new System.Windows.Forms.TextBox();
            this.dutyCycleLabel = new System.Windows.Forms.Label();
            this.saveToRomButton = new System.Windows.Forms.Button();
            this.halfBridgeRadioButton = new System.Windows.Forms.RadioButton();
            this.fullBridgeRadioButton = new System.Windows.Forms.RadioButton();
            this.rbOff = new System.Windows.Forms.RadioButton();
            this.rbAxe1 = new System.Windows.Forms.RadioButton();
            this.rbAxe2 = new System.Windows.Forms.RadioButton();
            this.rbAxe3 = new System.Windows.Forms.RadioButton();
            this.rbAxe4 = new System.Windows.Forms.RadioButton();
            this.rbAxe5 = new System.Windows.Forms.RadioButton();
            this.rbAxe6 = new System.Windows.Forms.RadioButton();
            this.rbAxe7 = new System.Windows.Forms.RadioButton();
            this.rbAxe8 = new System.Windows.Forms.RadioButton();
            this.rbAxe9 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dutyCycleTrackBar)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chartPanel
            // 
            this.chartPanel.Location = new System.Drawing.Point(12, 126);
            this.chartPanel.Name = "chartPanel";
            this.chartPanel.Size = new System.Drawing.Size(570, 295);
            this.chartPanel.TabIndex = 0;
            this.chartPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.chartPanel_Paint);
            // 
            // cbxVoltage
            // 
            this.cbxVoltage.AutoSize = true;
            this.cbxVoltage.Location = new System.Drawing.Point(13, 435);
            this.cbxVoltage.Name = "cbxVoltage";
            this.cbxVoltage.Size = new System.Drawing.Size(62, 17);
            this.cbxVoltage.TabIndex = 1;
            this.cbxVoltage.Text = "&Voltage";
            this.cbxVoltage.UseVisualStyleBackColor = true;
            // 
            // cbxCurrent
            // 
            this.cbxCurrent.AutoSize = true;
            this.cbxCurrent.Location = new System.Drawing.Point(82, 435);
            this.cbxCurrent.Name = "cbxCurrent";
            this.cbxCurrent.Size = new System.Drawing.Size(60, 17);
            this.cbxCurrent.TabIndex = 2;
            this.cbxCurrent.Text = "&Current";
            this.cbxCurrent.UseVisualStyleBackColor = true;
            // 
            // butClose
            // 
            this.butClose.Location = new System.Drawing.Point(507, 435);
            this.butClose.Name = "butClose";
            this.butClose.Size = new System.Drawing.Size(75, 23);
            this.butClose.TabIndex = 3;
            this.butClose.Text = "Close";
            this.butClose.UseVisualStyleBackColor = true;
            this.butClose.Click += new System.EventHandler(this.butClose_Click);
            // 
            // calibrateTimer
            // 
            this.calibrateTimer.Interval = 1000;
            this.calibrateTimer.Tick += new System.EventHandler(this.calibrateTimer_Tick);
            // 
            // minMaxLabel
            // 
            this.minMaxLabel.AutoSize = true;
            this.minMaxLabel.Location = new System.Drawing.Point(148, 436);
            this.minMaxLabel.Name = "minMaxLabel";
            this.minMaxLabel.Size = new System.Drawing.Size(76, 13);
            this.minMaxLabel.TabIndex = 4;
            this.minMaxLabel.Text = "Min, Max, P-P:";
            // 
            // dutyCycleTrackBar
            // 
            this.dutyCycleTrackBar.Location = new System.Drawing.Point(27, 12);
            this.dutyCycleTrackBar.Maximum = 167;
            this.dutyCycleTrackBar.Name = "dutyCycleTrackBar";
            this.dutyCycleTrackBar.Size = new System.Drawing.Size(229, 45);
            this.dutyCycleTrackBar.TabIndex = 5;
            this.dutyCycleTrackBar.TickFrequency = 10;
            this.dutyCycleTrackBar.Value = 167;
            this.dutyCycleTrackBar.Scroll += new System.EventHandler(this.OnDutyCycleTrackBarScroll);
            // 
            // dutyCycleTextBox
            // 
            this.dutyCycleTextBox.Location = new System.Drawing.Point(342, 12);
            this.dutyCycleTextBox.Name = "dutyCycleTextBox";
            this.dutyCycleTextBox.Size = new System.Drawing.Size(53, 20);
            this.dutyCycleTextBox.TabIndex = 25;
            this.dutyCycleTextBox.TextChanged += new System.EventHandler(this.OnDutyCycleTextChanged);
            // 
            // dutyCycleLabel
            // 
            this.dutyCycleLabel.AutoSize = true;
            this.dutyCycleLabel.Location = new System.Drawing.Point(275, 15);
            this.dutyCycleLabel.Name = "dutyCycleLabel";
            this.dutyCycleLabel.Size = new System.Drawing.Size(61, 13);
            this.dutyCycleLabel.TabIndex = 24;
            this.dutyCycleLabel.Text = "Duty Cycle:";
            // 
            // saveToRomButton
            // 
            this.saveToRomButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveToRomButton.Location = new System.Drawing.Point(421, 15);
            this.saveToRomButton.Name = "saveToRomButton";
            this.saveToRomButton.Size = new System.Drawing.Size(106, 23);
            this.saveToRomButton.TabIndex = 26;
            this.saveToRomButton.Text = "Save to ROM";
            this.saveToRomButton.UseVisualStyleBackColor = true;
            // 
            // halfBridgeRadioButton
            // 
            this.halfBridgeRadioButton.AutoSize = true;
            this.halfBridgeRadioButton.Location = new System.Drawing.Point(125, 49);
            this.halfBridgeRadioButton.Name = "halfBridgeRadioButton";
            this.halfBridgeRadioButton.Size = new System.Drawing.Size(77, 17);
            this.halfBridgeRadioButton.TabIndex = 28;
            this.halfBridgeRadioButton.Text = "&Half Bridge";
            this.halfBridgeRadioButton.UseVisualStyleBackColor = true;
            this.halfBridgeRadioButton.CheckedChanged += new System.EventHandler(this.OnHalfFullBridgeClick);
            // 
            // fullBridgeRadioButton
            // 
            this.fullBridgeRadioButton.AutoSize = true;
            this.fullBridgeRadioButton.Location = new System.Drawing.Point(45, 49);
            this.fullBridgeRadioButton.Name = "fullBridgeRadioButton";
            this.fullBridgeRadioButton.Size = new System.Drawing.Size(74, 17);
            this.fullBridgeRadioButton.TabIndex = 27;
            this.fullBridgeRadioButton.Text = "&Full Bridge";
            this.fullBridgeRadioButton.UseVisualStyleBackColor = true;
            this.fullBridgeRadioButton.CheckedChanged += new System.EventHandler(this.OnHalfFullBridgeClick);
            // 
            // rbOff
            // 
            this.rbOff.AutoSize = true;
            this.rbOff.Checked = true;
            this.rbOff.Location = new System.Drawing.Point(5, 19);
            this.rbOff.Name = "rbOff";
            this.rbOff.Size = new System.Drawing.Size(39, 17);
            this.rbOff.TabIndex = 29;
            this.rbOff.TabStop = true;
            this.rbOff.Text = "Off";
            this.rbOff.UseVisualStyleBackColor = true;
            this.rbOff.CheckedChanged += new System.EventHandler(this.rbOff_CheckedChanged);
            // 
            // rbAxe1
            // 
            this.rbAxe1.AutoSize = true;
            this.rbAxe1.Location = new System.Drawing.Point(50, 19);
            this.rbAxe1.Name = "rbAxe1";
            this.rbAxe1.Size = new System.Drawing.Size(52, 17);
            this.rbAxe1.TabIndex = 30;
            this.rbAxe1.Text = "Axe 1";
            this.rbAxe1.UseVisualStyleBackColor = true;
            this.rbAxe1.CheckedChanged += new System.EventHandler(this.rbAxe1_CheckedChanged);
            // 
            // rbAxe2
            // 
            this.rbAxe2.AutoSize = true;
            this.rbAxe2.Location = new System.Drawing.Point(108, 19);
            this.rbAxe2.Name = "rbAxe2";
            this.rbAxe2.Size = new System.Drawing.Size(52, 17);
            this.rbAxe2.TabIndex = 31;
            this.rbAxe2.Text = "Axe 2";
            this.rbAxe2.UseVisualStyleBackColor = true;
            this.rbAxe2.CheckedChanged += new System.EventHandler(this.rbAxe2_CheckedChanged);
            // 
            // rbAxe3
            // 
            this.rbAxe3.AutoSize = true;
            this.rbAxe3.Location = new System.Drawing.Point(166, 19);
            this.rbAxe3.Name = "rbAxe3";
            this.rbAxe3.Size = new System.Drawing.Size(52, 17);
            this.rbAxe3.TabIndex = 32;
            this.rbAxe3.Text = "Axe 3";
            this.rbAxe3.UseVisualStyleBackColor = true;
            this.rbAxe3.CheckedChanged += new System.EventHandler(this.rbAxe3_CheckedChanged);
            // 
            // rbAxe4
            // 
            this.rbAxe4.AutoSize = true;
            this.rbAxe4.Enabled = false;
            this.rbAxe4.Location = new System.Drawing.Point(224, 19);
            this.rbAxe4.Name = "rbAxe4";
            this.rbAxe4.Size = new System.Drawing.Size(52, 17);
            this.rbAxe4.TabIndex = 33;
            this.rbAxe4.Text = "Axe 4";
            this.rbAxe4.UseVisualStyleBackColor = true;
            this.rbAxe4.CheckedChanged += new System.EventHandler(this.rbAxe4_CheckedChanged);
            // 
            // rbAxe5
            // 
            this.rbAxe5.AutoSize = true;
            this.rbAxe5.Enabled = false;
            this.rbAxe5.Location = new System.Drawing.Point(282, 19);
            this.rbAxe5.Name = "rbAxe5";
            this.rbAxe5.Size = new System.Drawing.Size(52, 17);
            this.rbAxe5.TabIndex = 34;
            this.rbAxe5.Text = "Axe 5";
            this.rbAxe5.UseVisualStyleBackColor = true;
            this.rbAxe5.CheckedChanged += new System.EventHandler(this.rbAxe5_CheckedChanged);
            // 
            // rbAxe6
            // 
            this.rbAxe6.AutoSize = true;
            this.rbAxe6.Enabled = false;
            this.rbAxe6.Location = new System.Drawing.Point(340, 19);
            this.rbAxe6.Name = "rbAxe6";
            this.rbAxe6.Size = new System.Drawing.Size(52, 17);
            this.rbAxe6.TabIndex = 35;
            this.rbAxe6.Text = "Axe 6";
            this.rbAxe6.UseVisualStyleBackColor = true;
            this.rbAxe6.CheckedChanged += new System.EventHandler(this.rbAxe6_CheckedChanged);
            // 
            // rbAxe7
            // 
            this.rbAxe7.AutoSize = true;
            this.rbAxe7.Enabled = false;
            this.rbAxe7.Location = new System.Drawing.Point(398, 19);
            this.rbAxe7.Name = "rbAxe7";
            this.rbAxe7.Size = new System.Drawing.Size(52, 17);
            this.rbAxe7.TabIndex = 36;
            this.rbAxe7.Text = "Axe 7";
            this.rbAxe7.UseVisualStyleBackColor = true;
            this.rbAxe7.CheckedChanged += new System.EventHandler(this.rbAxe7_CheckedChanged);
            // 
            // rbAxe8
            // 
            this.rbAxe8.AutoSize = true;
            this.rbAxe8.Enabled = false;
            this.rbAxe8.Location = new System.Drawing.Point(456, 19);
            this.rbAxe8.Name = "rbAxe8";
            this.rbAxe8.Size = new System.Drawing.Size(52, 17);
            this.rbAxe8.TabIndex = 37;
            this.rbAxe8.Text = "Axe 8";
            this.rbAxe8.UseVisualStyleBackColor = true;
            this.rbAxe8.CheckedChanged += new System.EventHandler(this.rbAxe8_CheckedChanged);
            // 
            // rbAxe9
            // 
            this.rbAxe9.AutoSize = true;
            this.rbAxe9.Enabled = false;
            this.rbAxe9.Location = new System.Drawing.Point(508, 19);
            this.rbAxe9.Name = "rbAxe9";
            this.rbAxe9.Size = new System.Drawing.Size(52, 17);
            this.rbAxe9.TabIndex = 38;
            this.rbAxe9.Text = "Axe 9";
            this.rbAxe9.UseVisualStyleBackColor = true;
            this.rbAxe9.CheckedChanged += new System.EventHandler(this.rbAxe9_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbAxe4);
            this.groupBox1.Controls.Add(this.rbAxe9);
            this.groupBox1.Controls.Add(this.rbOff);
            this.groupBox1.Controls.Add(this.rbAxe8);
            this.groupBox1.Controls.Add(this.rbAxe1);
            this.groupBox1.Controls.Add(this.rbAxe7);
            this.groupBox1.Controls.Add(this.rbAxe2);
            this.groupBox1.Controls.Add(this.rbAxe6);
            this.groupBox1.Controls.Add(this.rbAxe3);
            this.groupBox1.Controls.Add(this.rbAxe5);
            this.groupBox1.Location = new System.Drawing.Point(13, 72);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(569, 48);
            this.groupBox1.TabIndex = 39;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Channel";
            // 
            // CalibrationGraphDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 470);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.halfBridgeRadioButton);
            this.Controls.Add(this.fullBridgeRadioButton);
            this.Controls.Add(this.saveToRomButton);
            this.Controls.Add(this.dutyCycleTextBox);
            this.Controls.Add(this.dutyCycleLabel);
            this.Controls.Add(this.dutyCycleTrackBar);
            this.Controls.Add(this.minMaxLabel);
            this.Controls.Add(this.butClose);
            this.Controls.Add(this.cbxCurrent);
            this.Controls.Add(this.cbxVoltage);
            this.Controls.Add(this.chartPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CalibrationGraphDialog";
            this.Text = "Calibration Graph";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CalibrationGraphDialog_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dutyCycleTrackBar)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel chartPanel;
        private System.Windows.Forms.CheckBox cbxVoltage;
        private System.Windows.Forms.CheckBox cbxCurrent;
        private System.Windows.Forms.Button butClose;
        private System.Windows.Forms.Timer calibrateTimer;
        private System.Windows.Forms.Label minMaxLabel;
        private System.Windows.Forms.TrackBar dutyCycleTrackBar;
        private System.Windows.Forms.TextBox dutyCycleTextBox;
        private System.Windows.Forms.Label dutyCycleLabel;
        private System.Windows.Forms.Button saveToRomButton;
        private System.Windows.Forms.RadioButton halfBridgeRadioButton;
        private System.Windows.Forms.RadioButton fullBridgeRadioButton;
        private System.Windows.Forms.RadioButton rbOff;
        private System.Windows.Forms.RadioButton rbAxe1;
        private System.Windows.Forms.RadioButton rbAxe2;
        private System.Windows.Forms.RadioButton rbAxe3;
        private System.Windows.Forms.RadioButton rbAxe4;
        private System.Windows.Forms.RadioButton rbAxe5;
        private System.Windows.Forms.RadioButton rbAxe6;
        private System.Windows.Forms.RadioButton rbAxe7;
        private System.Windows.Forms.RadioButton rbAxe8;
        private System.Windows.Forms.RadioButton rbAxe9;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}