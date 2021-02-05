namespace SDK_SC_RfidReader.UtilsWindows
{
    /// <summary>
    /// 
    /// </summary>
    partial class FindThresholdDialog
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
            this.autoUpdateCheckBox = new System.Windows.Forms.CheckBox();
            this.sampleCountTextBox = new System.Windows.Forms.TextBox();
            this.startStopSamplingButton = new System.Windows.Forms.Button();
            this.updateButton = new System.Windows.Forms.Button();
            this.meanNoResponseLabel = new System.Windows.Forms.Label();
            this.sigmaNoResponseLabel = new System.Windows.Forms.Label();
            this.meanResponseLabel = new System.Windows.Forms.Label();
            this.sigmaResponseLabel = new System.Windows.Forms.Label();
            this.cycleCheckBox = new System.Windows.Forms.CheckBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbAxe4 = new System.Windows.Forms.RadioButton();
            this.rbAxe9 = new System.Windows.Forms.RadioButton();
            this.rbOff = new System.Windows.Forms.RadioButton();
            this.rbAxe8 = new System.Windows.Forms.RadioButton();
            this.rbAxe1 = new System.Windows.Forms.RadioButton();
            this.rbAxe7 = new System.Windows.Forms.RadioButton();
            this.rbAxe2 = new System.Windows.Forms.RadioButton();
            this.rbAxe6 = new System.Windows.Forms.RadioButton();
            this.rbAxe3 = new System.Windows.Forms.RadioButton();
            this.rbAxe5 = new System.Windows.Forms.RadioButton();
            this.gbFrequency = new System.Windows.Forms.GroupBox();
            this.saveToRomButton = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.labelVant = new System.Windows.Forms.Label();
            this.labelFreq = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.correlationThresholdTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.gbFrequency.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // autoUpdateCheckBox
            // 
            this.autoUpdateCheckBox.AutoSize = true;
            this.autoUpdateCheckBox.Location = new System.Drawing.Point(16, 371);
            this.autoUpdateCheckBox.Name = "autoUpdateCheckBox";
            this.autoUpdateCheckBox.Size = new System.Drawing.Size(92, 17);
            this.autoUpdateCheckBox.TabIndex = 0;
            this.autoUpdateCheckBox.Text = "&Auto Update :";
            this.autoUpdateCheckBox.UseVisualStyleBackColor = true;
            this.autoUpdateCheckBox.Click += new System.EventHandler(this.autoUpdateCheckBox_Click);
            // 
            // sampleCountTextBox
            // 
            this.sampleCountTextBox.Location = new System.Drawing.Point(109, 368);
            this.sampleCountTextBox.Name = "sampleCountTextBox";
            this.sampleCountTextBox.Size = new System.Drawing.Size(55, 20);
            this.sampleCountTextBox.TabIndex = 1;
            // 
            // startStopSamplingButton
            // 
            this.startStopSamplingButton.Location = new System.Drawing.Point(170, 367);
            this.startStopSamplingButton.Name = "startStopSamplingButton";
            this.startStopSamplingButton.Size = new System.Drawing.Size(75, 23);
            this.startStopSamplingButton.TabIndex = 2;
            this.startStopSamplingButton.Text = "Start !";
            this.startStopSamplingButton.UseVisualStyleBackColor = true;
            this.startStopSamplingButton.Visible = false;
            this.startStopSamplingButton.Click += new System.EventHandler(this.startStopSamplingButton_Click);
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(251, 366);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(75, 23);
            this.updateButton.TabIndex = 3;
            this.updateButton.Text = "&Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Visible = false;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // meanNoResponseLabel
            // 
            this.meanNoResponseLabel.AutoSize = true;
            this.meanNoResponseLabel.Location = new System.Drawing.Point(333, 366);
            this.meanNoResponseLabel.Name = "meanNoResponseLabel";
            this.meanNoResponseLabel.Size = new System.Drawing.Size(46, 13);
            this.meanNoResponseLabel.TabIndex = 4;
            this.meanNoResponseLabel.Text = "Mean: ?";
            // 
            // sigmaNoResponseLabel
            // 
            this.sigmaNoResponseLabel.AutoSize = true;
            this.sigmaNoResponseLabel.Location = new System.Drawing.Point(333, 383);
            this.sigmaNoResponseLabel.Name = "sigmaNoResponseLabel";
            this.sigmaNoResponseLabel.Size = new System.Drawing.Size(48, 13);
            this.sigmaNoResponseLabel.TabIndex = 5;
            this.sigmaNoResponseLabel.Text = "Sigma: ?";
            // 
            // meanResponseLabel
            // 
            this.meanResponseLabel.AutoSize = true;
            this.meanResponseLabel.Location = new System.Drawing.Point(412, 365);
            this.meanResponseLabel.Name = "meanResponseLabel";
            this.meanResponseLabel.Size = new System.Drawing.Size(46, 13);
            this.meanResponseLabel.TabIndex = 6;
            this.meanResponseLabel.Text = "Mean: ?";
            // 
            // sigmaResponseLabel
            // 
            this.sigmaResponseLabel.AutoSize = true;
            this.sigmaResponseLabel.Location = new System.Drawing.Point(415, 383);
            this.sigmaResponseLabel.Name = "sigmaResponseLabel";
            this.sigmaResponseLabel.Size = new System.Drawing.Size(48, 13);
            this.sigmaResponseLabel.TabIndex = 7;
            this.sigmaResponseLabel.Text = "Sigma: ?";
            // 
            // cycleCheckBox
            // 
            this.cycleCheckBox.AutoSize = true;
            this.cycleCheckBox.Location = new System.Drawing.Point(489, 371);
            this.cycleCheckBox.Name = "cycleCheckBox";
            this.cycleCheckBox.Size = new System.Drawing.Size(52, 17);
            this.cycleCheckBox.TabIndex = 8;
            this.cycleCheckBox.Text = "&Cycle";
            this.cycleCheckBox.UseVisualStyleBackColor = true;
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(547, 367);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 9;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
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
            this.groupBox1.Location = new System.Drawing.Point(16, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(606, 48);
            this.groupBox1.TabIndex = 40;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Channel";
            // 
            // rbAxe4
            // 
            this.rbAxe4.AutoSize = true;
            this.rbAxe4.Location = new System.Drawing.Point(224, 19);
            this.rbAxe4.Name = "rbAxe4";
            this.rbAxe4.Size = new System.Drawing.Size(52, 17);
            this.rbAxe4.TabIndex = 33;
            this.rbAxe4.Text = "Axe 4";
            this.rbAxe4.UseVisualStyleBackColor = true;
            this.rbAxe4.Click += new System.EventHandler(this.rbAxe4_CheckedChanged);
            // 
            // rbAxe9
            // 
            this.rbAxe9.AutoSize = true;
            this.rbAxe9.Location = new System.Drawing.Point(508, 19);
            this.rbAxe9.Name = "rbAxe9";
            this.rbAxe9.Size = new System.Drawing.Size(52, 17);
            this.rbAxe9.TabIndex = 38;
            this.rbAxe9.Text = "Axe 9";
            this.rbAxe9.UseVisualStyleBackColor = true;
            this.rbAxe9.Click += new System.EventHandler(this.rbAxe9_CheckedChanged);
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
            this.rbOff.Click += new System.EventHandler(this.rbOff_CheckedChanged);
            // 
            // rbAxe8
            // 
            this.rbAxe8.AutoSize = true;
            this.rbAxe8.Location = new System.Drawing.Point(456, 19);
            this.rbAxe8.Name = "rbAxe8";
            this.rbAxe8.Size = new System.Drawing.Size(52, 17);
            this.rbAxe8.TabIndex = 37;
            this.rbAxe8.Text = "Axe 8";
            this.rbAxe8.UseVisualStyleBackColor = true;
            this.rbAxe8.Click += new System.EventHandler(this.rbAxe8_CheckedChanged);
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
            this.rbAxe1.Click += new System.EventHandler(this.rbAxe1_CheckedChanged);
            // 
            // rbAxe7
            // 
            this.rbAxe7.AutoSize = true;
            this.rbAxe7.Location = new System.Drawing.Point(398, 19);
            this.rbAxe7.Name = "rbAxe7";
            this.rbAxe7.Size = new System.Drawing.Size(52, 17);
            this.rbAxe7.TabIndex = 36;
            this.rbAxe7.Text = "Axe 7";
            this.rbAxe7.UseVisualStyleBackColor = true;
            this.rbAxe7.Click += new System.EventHandler(this.rbAxe7_CheckedChanged);
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
            this.rbAxe2.Click += new System.EventHandler(this.rbAxe2_CheckedChanged);
            // 
            // rbAxe6
            // 
            this.rbAxe6.AutoSize = true;
            this.rbAxe6.Location = new System.Drawing.Point(340, 19);
            this.rbAxe6.Name = "rbAxe6";
            this.rbAxe6.Size = new System.Drawing.Size(52, 17);
            this.rbAxe6.TabIndex = 35;
            this.rbAxe6.Text = "Axe 6";
            this.rbAxe6.UseVisualStyleBackColor = true;
            this.rbAxe6.Click += new System.EventHandler(this.rbAxe6_CheckedChanged);
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
            this.rbAxe3.Click += new System.EventHandler(this.rbAxe3_CheckedChanged);
            // 
            // rbAxe5
            // 
            this.rbAxe5.AutoSize = true;
            this.rbAxe5.Location = new System.Drawing.Point(282, 19);
            this.rbAxe5.Name = "rbAxe5";
            this.rbAxe5.Size = new System.Drawing.Size(52, 17);
            this.rbAxe5.TabIndex = 34;
            this.rbAxe5.Text = "Axe 5";
            this.rbAxe5.UseVisualStyleBackColor = true;
            this.rbAxe5.Click += new System.EventHandler(this.rbAxe5_CheckedChanged);
            // 
            // gbFrequency
            // 
            this.gbFrequency.Controls.Add(this.saveToRomButton);
            this.gbFrequency.Controls.Add(this.button7);
            this.gbFrequency.Controls.Add(this.button6);
            this.gbFrequency.Controls.Add(this.button5);
            this.gbFrequency.Controls.Add(this.labelVant);
            this.gbFrequency.Controls.Add(this.labelFreq);
            this.gbFrequency.Location = new System.Drawing.Point(16, 66);
            this.gbFrequency.Name = "gbFrequency";
            this.gbFrequency.Size = new System.Drawing.Size(404, 48);
            this.gbFrequency.TabIndex = 41;
            this.gbFrequency.TabStop = false;
            this.gbFrequency.Text = "Frequency";
            // 
            // saveToRomButton
            // 
            this.saveToRomButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveToRomButton.Location = new System.Drawing.Point(199, 19);
            this.saveToRomButton.Name = "saveToRomButton";
            this.saveToRomButton.Size = new System.Drawing.Size(64, 23);
            this.saveToRomButton.TabIndex = 53;
            this.saveToRomButton.Text = "Save F0";
            this.saveToRomButton.UseVisualStyleBackColor = true;
            this.saveToRomButton.Click += new System.EventHandler(this.saveToRomButton_Click);
            // 
            // button7
            // 
            this.button7.Enabled = false;
            this.button7.Location = new System.Drawing.Point(135, 19);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(64, 23);
            this.button7.TabIndex = 52;
            this.button7.Text = "Good F0";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Visible = false;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(71, 19);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(64, 23);
            this.button6.TabIndex = 51;
            this.button6.Text = "- F0";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(7, 19);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(64, 23);
            this.button5.TabIndex = 50;
            this.button5.Text = "+ F0";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // labelVant
            // 
            this.labelVant.AutoSize = true;
            this.labelVant.Location = new System.Drawing.Point(272, 29);
            this.labelVant.Name = "labelVant";
            this.labelVant.Size = new System.Drawing.Size(38, 13);
            this.labelVant.TabIndex = 49;
            this.labelVant.Text = "Vant : ";
            // 
            // labelFreq
            // 
            this.labelFreq.AutoSize = true;
            this.labelFreq.Location = new System.Drawing.Point(272, 16);
            this.labelFreq.Name = "labelFreq";
            this.labelFreq.Size = new System.Drawing.Size(80, 13);
            this.labelFreq.TabIndex = 46;
            this.labelFreq.Text = "F0 : 119600 Hz";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.correlationThresholdTextBox);
            this.groupBox2.Location = new System.Drawing.Point(426, 66);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(195, 48);
            this.groupBox2.TabIndex = 42;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Threshold";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(87, 16);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(102, 23);
            this.button1.TabIndex = 54;
            this.button1.Text = "Save To Rom";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // correlationThresholdTextBox
            // 
            this.correlationThresholdTextBox.Location = new System.Drawing.Point(12, 19);
            this.correlationThresholdTextBox.Name = "correlationThresholdTextBox";
            this.correlationThresholdTextBox.Size = new System.Drawing.Size(64, 20);
            this.correlationThresholdTextBox.TabIndex = 43;
            this.correlationThresholdTextBox.Text = "60";
            // 
            // FindThresholdDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 402);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.gbFrequency);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.cycleCheckBox);
            this.Controls.Add(this.sigmaResponseLabel);
            this.Controls.Add(this.meanResponseLabel);
            this.Controls.Add(this.sigmaNoResponseLabel);
            this.Controls.Add(this.meanNoResponseLabel);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.startStopSamplingButton);
            this.Controls.Add(this.sampleCountTextBox);
            this.Controls.Add(this.autoUpdateCheckBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FindThresholdDialog";
            this.Text = "FindThresholdDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FindThresholdDialog_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbFrequency.ResumeLayout(false);
            this.gbFrequency.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox autoUpdateCheckBox;
        private System.Windows.Forms.TextBox sampleCountTextBox;
        private System.Windows.Forms.Button startStopSamplingButton;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Label meanNoResponseLabel;
        private System.Windows.Forms.Label sigmaNoResponseLabel;
        private System.Windows.Forms.Label meanResponseLabel;
        private System.Windows.Forms.Label sigmaResponseLabel;
        private System.Windows.Forms.CheckBox cycleCheckBox;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbAxe4;
        private System.Windows.Forms.RadioButton rbAxe9;
        private System.Windows.Forms.RadioButton rbOff;
        private System.Windows.Forms.RadioButton rbAxe8;
        private System.Windows.Forms.RadioButton rbAxe1;
        private System.Windows.Forms.RadioButton rbAxe7;
        private System.Windows.Forms.RadioButton rbAxe2;
        private System.Windows.Forms.RadioButton rbAxe6;
        private System.Windows.Forms.RadioButton rbAxe3;
        private System.Windows.Forms.RadioButton rbAxe5;
        private System.Windows.Forms.GroupBox gbFrequency;
        private System.Windows.Forms.Button saveToRomButton;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label labelVant;
        private System.Windows.Forms.Label labelFreq;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox correlationThresholdTextBox;
    }
}