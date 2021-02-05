namespace SDK_SC_RfidReader.UtilsWindows
{
    partial class DoorAndLightDialog
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBarLight = new System.Windows.Forms.TrackBar();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.doorStateLabel = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLight)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.trackBarLight);
            this.groupBox1.Controls.Add(this.button8);
            this.groupBox1.Controls.Add(this.button9);
            this.groupBox1.Controls.Add(this.doorStateLabel);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(12, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(245, 186);
            this.groupBox1.TabIndex = 43;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Door / lock";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 147);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 44;
            this.label1.Text = "Light";
            // 
            // trackBarLight
            // 
            this.trackBarLight.Location = new System.Drawing.Point(70, 135);
            this.trackBarLight.Maximum = 300;
            this.trackBarLight.Name = "trackBarLight";
            this.trackBarLight.Size = new System.Drawing.Size(148, 45);
            this.trackBarLight.TabIndex = 43;
            this.trackBarLight.TickFrequency = 50;
            this.trackBarLight.Scroll += new System.EventHandler(this.trackBarLight_Scroll);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(119, 55);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(86, 32);
            this.button8.TabIndex = 42;
            this.button8.Text = "OPEN Slave";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(119, 93);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(86, 32);
            this.button9.TabIndex = 41;
            this.button9.Text = " CLOSE Slave";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // doorStateLabel
            // 
            this.doorStateLabel.Location = new System.Drawing.Point(17, 22);
            this.doorStateLabel.Name = "doorStateLabel";
            this.doorStateLabel.Size = new System.Drawing.Size(201, 20);
            this.doorStateLabel.TabIndex = 40;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(16, 55);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(86, 32);
            this.button2.TabIndex = 36;
            this.button2.Text = "OPEN master";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(16, 93);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 32);
            this.button1.TabIndex = 35;
            this.button1.Text = "CLOSE Master";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(28, 226);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(73, 35);
            this.button3.TabIndex = 44;
            this.button3.Text = "Alarm On";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(131, 226);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(73, 35);
            this.button4.TabIndex = 45;
            this.button4.Text = "Alarm Off";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // DoorAndLightDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(272, 283);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.groupBox1);
            this.Name = "DoorAndLightDialog";
            this.Text = "DoorAndLightDialog";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.TextBox doorStateLabel;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar trackBarLight;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}