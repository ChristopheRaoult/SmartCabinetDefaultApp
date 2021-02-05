namespace SDK_SC_RfidReader
{
    /// <summary>
    /// Class debug reader to display a small form to dispalyu reader status
    /// </summary>
    partial class StatusForm
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
            this.labelReader = new System.Windows.Forms.Label();
            this.labelConnected = new System.Windows.Forms.Label();
            this.labelInScan = new System.Windows.Forms.Label();
            this.labelLock = new System.Windows.Forms.Label();
            this.labelDoor = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.labelErr = new System.Windows.Forms.Label();
            this.labelmes = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelReader
            // 
            this.labelReader.AutoSize = true;
            this.labelReader.Location = new System.Drawing.Point(12, 9);
            this.labelReader.Name = "labelReader";
            this.labelReader.Size = new System.Drawing.Size(42, 13);
            this.labelReader.TabIndex = 0;
            this.labelReader.Text = "Reader";
            // 
            // labelConnected
            // 
            this.labelConnected.AutoSize = true;
            this.labelConnected.Location = new System.Drawing.Point(12, 24);
            this.labelConnected.Name = "labelConnected";
            this.labelConnected.Size = new System.Drawing.Size(59, 13);
            this.labelConnected.TabIndex = 1;
            this.labelConnected.Text = "Connected";
            // 
            // labelInScan
            // 
            this.labelInScan.AutoSize = true;
            this.labelInScan.Location = new System.Drawing.Point(12, 39);
            this.labelInScan.Name = "labelInScan";
            this.labelInScan.Size = new System.Drawing.Size(41, 13);
            this.labelInScan.TabIndex = 2;
            this.labelInScan.Text = "InScan";
            // 
            // labelLock
            // 
            this.labelLock.AutoSize = true;
            this.labelLock.Location = new System.Drawing.Point(12, 55);
            this.labelLock.Name = "labelLock";
            this.labelLock.Size = new System.Drawing.Size(31, 13);
            this.labelLock.TabIndex = 3;
            this.labelLock.Text = "Lock";
            // 
            // labelDoor
            // 
            this.labelDoor.AutoSize = true;
            this.labelDoor.Location = new System.Drawing.Point(12, 70);
            this.labelDoor.Name = "labelDoor";
            this.labelDoor.Size = new System.Drawing.Size(30, 13);
            this.labelDoor.TabIndex = 4;
            this.labelDoor.Text = "Door";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // labelErr
            // 
            this.labelErr.AutoSize = true;
            this.labelErr.Location = new System.Drawing.Point(12, 93);
            this.labelErr.Name = "labelErr";
            this.labelErr.Size = new System.Drawing.Size(29, 13);
            this.labelErr.TabIndex = 5;
            this.labelErr.Text = "Error";
            // 
            // labelmes
            // 
            this.labelmes.AutoSize = true;
            this.labelmes.Location = new System.Drawing.Point(12, 106);
            this.labelmes.Name = "labelmes";
            this.labelmes.Size = new System.Drawing.Size(53, 13);
            this.labelmes.TabIndex = 6;
            this.labelmes.Text = "Message:";
            // 
            // StatusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(233, 128);
            this.Controls.Add(this.labelmes);
            this.Controls.Add(this.labelErr);
            this.Controls.Add(this.labelDoor);
            this.Controls.Add(this.labelLock);
            this.Controls.Add(this.labelInScan);
            this.Controls.Add(this.labelConnected);
            this.Controls.Add(this.labelReader);
            this.Name = "StatusForm";
            this.Text = "StatusForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelReader;
        private System.Windows.Forms.Label labelConnected;
        private System.Windows.Forms.Label labelInScan;
        private System.Windows.Forms.Label labelLock;
        private System.Windows.Forms.Label labelDoor;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label labelErr;
        private System.Windows.Forms.Label labelmes;
    }
}