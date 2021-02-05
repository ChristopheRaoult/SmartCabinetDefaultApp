namespace SDK_SC_RfidReader.UtilsWindows
{
    partial class ConversionsDialog
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
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dstDBmTextBox = new System.Windows.Forms.TextBox();
            this.srcMicroTeslaTextBox = new System.Windows.Forms.TextBox();
            this.uTeslaLabel = new System.Windows.Forms.Label();
            this.startDBmLabel = new System.Windows.Forms.Label();
            this.dstMicroTeslaTextBox = new System.Windows.Forms.TextBox();
            this.srcDBmTextBox = new System.Windows.Forms.TextBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(144, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 13);
            this.label4.TabIndex = 43;
            this.label4.Text = "-->";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(144, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 13);
            this.label3.TabIndex = 42;
            this.label3.Text = "-->";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(166, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 41;
            this.label2.Text = "dBm";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 40;
            this.label1.Text = "uTesla";
            // 
            // dstDBmTextBox
            // 
            this.dstDBmTextBox.Location = new System.Drawing.Point(169, 89);
            this.dstDBmTextBox.Name = "dstDBmTextBox";
            this.dstDBmTextBox.ReadOnly = true;
            this.dstDBmTextBox.Size = new System.Drawing.Size(126, 20);
            this.dstDBmTextBox.TabIndex = 37;
            // 
            // srcMicroTeslaTextBox
            // 
            this.srcMicroTeslaTextBox.Location = new System.Drawing.Point(12, 89);
            this.srcMicroTeslaTextBox.Name = "srcMicroTeslaTextBox";
            this.srcMicroTeslaTextBox.Size = new System.Drawing.Size(126, 20);
            this.srcMicroTeslaTextBox.TabIndex = 34;
            this.srcMicroTeslaTextBox.TextChanged += new System.EventHandler(this.OnMicroTeslaChanged);
            // 
            // uTeslaLabel
            // 
            this.uTeslaLabel.AutoSize = true;
            this.uTeslaLabel.Location = new System.Drawing.Point(166, 17);
            this.uTeslaLabel.Name = "uTeslaLabel";
            this.uTeslaLabel.Size = new System.Drawing.Size(39, 13);
            this.uTeslaLabel.TabIndex = 39;
            this.uTeslaLabel.Text = "uTesla";
            // 
            // startDBmLabel
            // 
            this.startDBmLabel.AutoSize = true;
            this.startDBmLabel.Location = new System.Drawing.Point(12, 17);
            this.startDBmLabel.Name = "startDBmLabel";
            this.startDBmLabel.Size = new System.Drawing.Size(28, 13);
            this.startDBmLabel.TabIndex = 38;
            this.startDBmLabel.Text = "dBm";
            // 
            // dstMicroTeslaTextBox
            // 
            this.dstMicroTeslaTextBox.Location = new System.Drawing.Point(169, 34);
            this.dstMicroTeslaTextBox.Name = "dstMicroTeslaTextBox";
            this.dstMicroTeslaTextBox.ReadOnly = true;
            this.dstMicroTeslaTextBox.Size = new System.Drawing.Size(126, 20);
            this.dstMicroTeslaTextBox.TabIndex = 36;
            // 
            // srcDBmTextBox
            // 
            this.srcDBmTextBox.Location = new System.Drawing.Point(12, 34);
            this.srcDBmTextBox.Name = "srcDBmTextBox";
            this.srcDBmTextBox.Size = new System.Drawing.Size(126, 20);
            this.srcDBmTextBox.TabIndex = 33;
            this.srcDBmTextBox.TextChanged += new System.EventHandler(this.OnDBmTextChanged);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(221, 115);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 35;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // ConversionsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 147);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dstDBmTextBox);
            this.Controls.Add(this.srcMicroTeslaTextBox);
            this.Controls.Add(this.uTeslaLabel);
            this.Controls.Add(this.startDBmLabel);
            this.Controls.Add(this.dstMicroTeslaTextBox);
            this.Controls.Add(this.srcDBmTextBox);
            this.Controls.Add(this.closeButton);
            this.Name = "ConversionsDialog";
            this.Text = "ConversionsDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox dstDBmTextBox;
        private System.Windows.Forms.TextBox srcMicroTeslaTextBox;
        private System.Windows.Forms.Label uTeslaLabel;
        private System.Windows.Forms.Label startDBmLabel;
        private System.Windows.Forms.TextBox dstMicroTeslaTextBox;
        private System.Windows.Forms.TextBox srcDBmTextBox;
        private System.Windows.Forms.Button closeButton;
    }
}