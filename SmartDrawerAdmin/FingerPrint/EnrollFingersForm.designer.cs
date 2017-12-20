namespace SmartDrawerAdmin.Fingerprint
{
    partial class EnrollFingersForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnrollFingersForm));
            this.enrollmentControl = new DPFP.Gui.Enrollment.EnrollmentControl();
            this.labelUsername = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // enrollmentControl
            // 
            resources.ApplyResources(this.enrollmentControl, "enrollmentControl");
            this.enrollmentControl.EnrolledFingerMask = 0;
            this.enrollmentControl.MaxEnrollFingerCount = 10;
            this.enrollmentControl.Name = "enrollmentControl";
            this.enrollmentControl.ReaderSerialNumber = "00000000-0000-0000-0000-000000000000";
            this.enrollmentControl.OnDelete += new DPFP.Gui.Enrollment.EnrollmentControl._OnDelete(this.enrollmentControl_OnDelete);
            this.enrollmentControl.OnEnroll += new DPFP.Gui.Enrollment.EnrollmentControl._OnEnroll(this.enrollmentControl_OnEnroll);
            // 
            // labelUsername
            // 
            resources.ApplyResources(this.labelUsername, "labelUsername");
            this.labelUsername.Name = "labelUsername";
            // 
            // EnrollFingersForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelUsername);
            this.Controls.Add(this.enrollmentControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnrollFingersForm";
            this.ResumeLayout(false);

        }

        #endregion

        private DPFP.Gui.Enrollment.EnrollmentControl enrollmentControl;
        private System.Windows.Forms.Label labelUsername;
    }
}