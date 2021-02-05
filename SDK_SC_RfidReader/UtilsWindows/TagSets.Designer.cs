namespace SDK_SC_RfidReader.UtilsWindows
{
    /// <summary>
    /// 
    /// </summary>
	partial class TagSetsDialog
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
			if(disposing && (components != null))
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.startStopScanButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.tagListDataGrid = new System.Windows.Forms.DataGridView();
            this.TagID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SequentialPresent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SequentialMissing = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PresentTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MissingTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnAxis = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.loopTimer = new System.Windows.Forms.Timer(this.components);
            this.correlationThresholdLabel = new System.Windows.Forms.Label();
            this.correlationThresholdTextBox = new System.Windows.Forms.TextBox();
            this.loopPeriodLabel = new System.Windows.Forms.Label();
            this.loopPeriodTextBox = new System.Windows.Forms.TextBox();
            this.loopCheckBox = new System.Windows.Forms.CheckBox();
            this.assumeUnknownCheckBox = new System.Windows.Forms.CheckBox();
            this.receiveTagsGroupBox = new System.Windows.Forms.GroupBox();
            this.checkBoxDynamic = new System.Windows.Forms.CheckBox();
            this.checkBoxReverse = new System.Windows.Forms.CheckBox();
            this.receiveTagsSynchronouslyDuringScanRadioButton = new System.Windows.Forms.RadioButton();
            this.receiveTagsSynchronouslyAfterScanRadioButton = new System.Windows.Forms.RadioButton();
            this.receiveTagsAsynchronouslyRadioButton = new System.Windows.Forms.RadioButton();
            this.presentTagsLabel = new System.Windows.Forms.Label();
            this.missingTagsLabel = new System.Windows.Forms.Label();
            this.failedTagsDataGrid = new System.Windows.Forms.DataGridView();
            this.TagIdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.failReasonColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resetListButton = new System.Windows.Forms.Button();
            this.statusLoopCheckbox = new System.Windows.Forms.CheckBox();
            this.statusLoopTimer = new System.Windows.Forms.Timer(this.components);
            this.labelCpt = new System.Windows.Forms.Label();
            this.labelaxis = new System.Windows.Forms.Label();
            this.labelKZ = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.labelInfoAxe = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.tagListDataGrid)).BeginInit();
            this.receiveTagsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.failedTagsDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // startStopScanButton
            // 
            this.startStopScanButton.Location = new System.Drawing.Point(12, 12);
            this.startStopScanButton.Name = "startStopScanButton";
            this.startStopScanButton.Size = new System.Drawing.Size(106, 23);
            this.startStopScanButton.TabIndex = 16;
            this.startStopScanButton.Text = "&Start";
            this.startStopScanButton.UseVisualStyleBackColor = true;
            this.startStopScanButton.Click += new System.EventHandler(this.OnStartStopScanClicked);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(779, 461);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 14;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.OnCloseClick);
            // 
            // tagListDataGrid
            // 
            this.tagListDataGrid.AllowUserToAddRows = false;
            this.tagListDataGrid.AllowUserToDeleteRows = false;
            this.tagListDataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.tagListDataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.tagListDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tagListDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TagID,
            this.SequentialPresent,
            this.SequentialMissing,
            this.PresentTotal,
            this.MissingTotal,
            this.ColumnAxis});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.tagListDataGrid.DefaultCellStyle = dataGridViewCellStyle2;
            this.tagListDataGrid.Location = new System.Drawing.Point(251, 12);
            this.tagListDataGrid.Name = "tagListDataGrid";
            this.tagListDataGrid.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.tagListDataGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.tagListDataGrid.RowHeadersVisible = false;
            this.tagListDataGrid.Size = new System.Drawing.Size(603, 443);
            this.tagListDataGrid.TabIndex = 15;
            // 
            // TagID
            // 
            this.TagID.HeaderText = "Tag ID";
            this.TagID.Name = "TagID";
            this.TagID.ReadOnly = true;
            this.TagID.Width = 200;
            // 
            // SequentialPresent
            // 
            this.SequentialPresent.HeaderText = "Present Count";
            this.SequentialPresent.Name = "SequentialPresent";
            this.SequentialPresent.ReadOnly = true;
            // 
            // SequentialMissing
            // 
            this.SequentialMissing.HeaderText = "Missing Count";
            this.SequentialMissing.Name = "SequentialMissing";
            this.SequentialMissing.ReadOnly = true;
            // 
            // PresentTotal
            // 
            this.PresentTotal.HeaderText = "Present Total";
            this.PresentTotal.Name = "PresentTotal";
            this.PresentTotal.ReadOnly = true;
            // 
            // MissingTotal
            // 
            this.MissingTotal.HeaderText = "Missing Total";
            this.MissingTotal.Name = "MissingTotal";
            this.MissingTotal.ReadOnly = true;
            // 
            // ColumnAxis
            // 
            this.ColumnAxis.HeaderText = "Axis";
            this.ColumnAxis.Name = "ColumnAxis";
            this.ColumnAxis.ReadOnly = true;
            // 
            // loopTimer
            // 
            this.loopTimer.Interval = 200;
            this.loopTimer.Tick += new System.EventHandler(this.OnLoopTimerTick);
            // 
            // correlationThresholdLabel
            // 
            this.correlationThresholdLabel.AutoSize = true;
            this.correlationThresholdLabel.Location = new System.Drawing.Point(83, 93);
            this.correlationThresholdLabel.Name = "correlationThresholdLabel";
            this.correlationThresholdLabel.Size = new System.Drawing.Size(107, 13);
            this.correlationThresholdLabel.TabIndex = 21;
            this.correlationThresholdLabel.Text = "Correlation Threshold";
            // 
            // correlationThresholdTextBox
            // 
            this.correlationThresholdTextBox.Location = new System.Drawing.Point(12, 90);
            this.correlationThresholdTextBox.Name = "correlationThresholdTextBox";
            this.correlationThresholdTextBox.Size = new System.Drawing.Size(64, 20);
            this.correlationThresholdTextBox.TabIndex = 19;
            this.correlationThresholdTextBox.TextChanged += new System.EventHandler(this.OnCorrelationThresholdChanged);
            // 
            // loopPeriodLabel
            // 
            this.loopPeriodLabel.AutoSize = true;
            this.loopPeriodLabel.Location = new System.Drawing.Point(83, 67);
            this.loopPeriodLabel.Name = "loopPeriodLabel";
            this.loopPeriodLabel.Size = new System.Drawing.Size(113, 13);
            this.loopPeriodLabel.TabIndex = 20;
            this.loopPeriodLabel.Text = "Loop Period (seconds)";
            // 
            // loopPeriodTextBox
            // 
            this.loopPeriodTextBox.Location = new System.Drawing.Point(12, 64);
            this.loopPeriodTextBox.Name = "loopPeriodTextBox";
            this.loopPeriodTextBox.Size = new System.Drawing.Size(64, 20);
            this.loopPeriodTextBox.TabIndex = 18;
            this.loopPeriodTextBox.Text = "0.200";
            this.loopPeriodTextBox.TextChanged += new System.EventHandler(this.OnLoopPeriodChanged);
            // 
            // loopCheckBox
            // 
            this.loopCheckBox.AutoSize = true;
            this.loopCheckBox.Location = new System.Drawing.Point(12, 41);
            this.loopCheckBox.Name = "loopCheckBox";
            this.loopCheckBox.Size = new System.Drawing.Size(50, 17);
            this.loopCheckBox.TabIndex = 17;
            this.loopCheckBox.Text = "&Loop";
            this.loopCheckBox.UseVisualStyleBackColor = true;
            // 
            // assumeUnknownCheckBox
            // 
            this.assumeUnknownCheckBox.AutoSize = true;
            this.assumeUnknownCheckBox.Location = new System.Drawing.Point(12, 116);
            this.assumeUnknownCheckBox.Name = "assumeUnknownCheckBox";
            this.assumeUnknownCheckBox.Size = new System.Drawing.Size(139, 17);
            this.assumeUnknownCheckBox.TabIndex = 23;
            this.assumeUnknownCheckBox.Text = "&Assume Unknown Tags";
            this.assumeUnknownCheckBox.UseVisualStyleBackColor = true;
            // 
            // receiveTagsGroupBox
            // 
            this.receiveTagsGroupBox.Controls.Add(this.checkBoxDynamic);
            this.receiveTagsGroupBox.Controls.Add(this.checkBoxReverse);
            this.receiveTagsGroupBox.Controls.Add(this.receiveTagsSynchronouslyDuringScanRadioButton);
            this.receiveTagsGroupBox.Controls.Add(this.receiveTagsSynchronouslyAfterScanRadioButton);
            this.receiveTagsGroupBox.Controls.Add(this.receiveTagsAsynchronouslyRadioButton);
            this.receiveTagsGroupBox.Location = new System.Drawing.Point(12, 139);
            this.receiveTagsGroupBox.Name = "receiveTagsGroupBox";
            this.receiveTagsGroupBox.Size = new System.Drawing.Size(218, 125);
            this.receiveTagsGroupBox.TabIndex = 27;
            this.receiveTagsGroupBox.TabStop = false;
            this.receiveTagsGroupBox.Text = "Receive Tags:";
            // 
            // checkBoxDynamic
            // 
            this.checkBoxDynamic.AutoSize = true;
            this.checkBoxDynamic.Location = new System.Drawing.Point(121, 92);
            this.checkBoxDynamic.Name = "checkBoxDynamic";
            this.checkBoxDynamic.Size = new System.Drawing.Size(63, 17);
            this.checkBoxDynamic.TabIndex = 4;
            this.checkBoxDynamic.Text = "Use KR";
            this.checkBoxDynamic.UseVisualStyleBackColor = true;
            // 
            // checkBoxReverse
            // 
            this.checkBoxReverse.AutoSize = true;
            this.checkBoxReverse.Location = new System.Drawing.Point(6, 92);
            this.checkBoxReverse.Name = "checkBoxReverse";
            this.checkBoxReverse.Size = new System.Drawing.Size(73, 17);
            this.checkBoxReverse.TabIndex = 3;
            this.checkBoxReverse.Text = "Unlock all";
            this.checkBoxReverse.UseVisualStyleBackColor = true;
            // 
            // receiveTagsSynchronouslyDuringScanRadioButton
            // 
            this.receiveTagsSynchronouslyDuringScanRadioButton.AutoSize = true;
            this.receiveTagsSynchronouslyDuringScanRadioButton.Location = new System.Drawing.Point(6, 65);
            this.receiveTagsSynchronouslyDuringScanRadioButton.Name = "receiveTagsSynchronouslyDuringScanRadioButton";
            this.receiveTagsSynchronouslyDuringScanRadioButton.Size = new System.Drawing.Size(156, 17);
            this.receiveTagsSynchronouslyDuringScanRadioButton.TabIndex = 2;
            this.receiveTagsSynchronouslyDuringScanRadioButton.TabStop = true;
            this.receiveTagsSynchronouslyDuringScanRadioButton.Text = "Synchronously During Scan";
            this.receiveTagsSynchronouslyDuringScanRadioButton.UseVisualStyleBackColor = true;
            // 
            // receiveTagsSynchronouslyAfterScanRadioButton
            // 
            this.receiveTagsSynchronouslyAfterScanRadioButton.AutoSize = true;
            this.receiveTagsSynchronouslyAfterScanRadioButton.Location = new System.Drawing.Point(6, 42);
            this.receiveTagsSynchronouslyAfterScanRadioButton.Name = "receiveTagsSynchronouslyAfterScanRadioButton";
            this.receiveTagsSynchronouslyAfterScanRadioButton.Size = new System.Drawing.Size(147, 17);
            this.receiveTagsSynchronouslyAfterScanRadioButton.TabIndex = 1;
            this.receiveTagsSynchronouslyAfterScanRadioButton.TabStop = true;
            this.receiveTagsSynchronouslyAfterScanRadioButton.Text = "Synchronously After Scan";
            this.receiveTagsSynchronouslyAfterScanRadioButton.UseVisualStyleBackColor = true;
            // 
            // receiveTagsAsynchronouslyRadioButton
            // 
            this.receiveTagsAsynchronouslyRadioButton.AutoSize = true;
            this.receiveTagsAsynchronouslyRadioButton.Location = new System.Drawing.Point(6, 19);
            this.receiveTagsAsynchronouslyRadioButton.Name = "receiveTagsAsynchronouslyRadioButton";
            this.receiveTagsAsynchronouslyRadioButton.Size = new System.Drawing.Size(99, 17);
            this.receiveTagsAsynchronouslyRadioButton.TabIndex = 0;
            this.receiveTagsAsynchronouslyRadioButton.TabStop = true;
            this.receiveTagsAsynchronouslyRadioButton.Text = "Asynchronously";
            this.receiveTagsAsynchronouslyRadioButton.UseVisualStyleBackColor = true;
            // 
            // presentTagsLabel
            // 
            this.presentTagsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.presentTagsLabel.AutoSize = true;
            this.presentTagsLabel.Location = new System.Drawing.Point(248, 461);
            this.presentTagsLabel.Name = "presentTagsLabel";
            this.presentTagsLabel.Size = new System.Drawing.Size(49, 13);
            this.presentTagsLabel.TabIndex = 28;
            this.presentTagsLabel.Text = "Present: ";
            // 
            // missingTagsLabel
            // 
            this.missingTagsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.missingTagsLabel.AutoSize = true;
            this.missingTagsLabel.Location = new System.Drawing.Point(333, 461);
            this.missingTagsLabel.Name = "missingTagsLabel";
            this.missingTagsLabel.Size = new System.Drawing.Size(45, 13);
            this.missingTagsLabel.TabIndex = 29;
            this.missingTagsLabel.Text = "Missing:";
            // 
            // failedTagsDataGrid
            // 
            this.failedTagsDataGrid.AllowUserToAddRows = false;
            this.failedTagsDataGrid.AllowUserToDeleteRows = false;
            this.failedTagsDataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.failedTagsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.failedTagsDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TagIdColumn,
            this.failReasonColumn});
            this.failedTagsDataGrid.Location = new System.Drawing.Point(12, 270);
            this.failedTagsDataGrid.Name = "failedTagsDataGrid";
            this.failedTagsDataGrid.ReadOnly = true;
            this.failedTagsDataGrid.RowHeadersVisible = false;
            this.failedTagsDataGrid.Size = new System.Drawing.Size(233, 185);
            this.failedTagsDataGrid.TabIndex = 30;
            // 
            // TagIdColumn
            // 
            this.TagIdColumn.Frozen = true;
            this.TagIdColumn.HeaderText = "Tag ID";
            this.TagIdColumn.Name = "TagIdColumn";
            this.TagIdColumn.ReadOnly = true;
            this.TagIdColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.TagIdColumn.Width = 125;
            // 
            // failReasonColumn
            // 
            this.failReasonColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.failReasonColumn.HeaderText = "Failure Reason";
            this.failReasonColumn.Name = "failReasonColumn";
            this.failReasonColumn.ReadOnly = true;
            this.failReasonColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // resetListButton
            // 
            this.resetListButton.Location = new System.Drawing.Point(124, 12);
            this.resetListButton.Name = "resetListButton";
            this.resetListButton.Size = new System.Drawing.Size(106, 23);
            this.resetListButton.TabIndex = 31;
            this.resetListButton.Text = "&Reset List";
            this.resetListButton.UseVisualStyleBackColor = true;
            this.resetListButton.Click += new System.EventHandler(this.OnResetListClick);
            // 
            // statusLoopCheckbox
            // 
            this.statusLoopCheckbox.AutoSize = true;
            this.statusLoopCheckbox.Location = new System.Drawing.Point(124, 41);
            this.statusLoopCheckbox.Name = "statusLoopCheckbox";
            this.statusLoopCheckbox.Size = new System.Drawing.Size(83, 17);
            this.statusLoopCheckbox.TabIndex = 32;
            this.statusLoopCheckbox.Text = "S&tatus Loop";
            this.statusLoopCheckbox.UseVisualStyleBackColor = true;
            // 
            // statusLoopTimer
            // 
            this.statusLoopTimer.Enabled = true;
            this.statusLoopTimer.Tick += new System.EventHandler(this.OnStatusLoopTimerTick);
            // 
            // labelCpt
            // 
            this.labelCpt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelCpt.AutoSize = true;
            this.labelCpt.Location = new System.Drawing.Point(413, 461);
            this.labelCpt.Name = "labelCpt";
            this.labelCpt.Size = new System.Drawing.Size(38, 13);
            this.labelCpt.TabIndex = 33;
            this.labelCpt.Text = "Count:";
            // 
            // labelaxis
            // 
            this.labelaxis.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelaxis.AutoSize = true;
            this.labelaxis.Location = new System.Drawing.Point(488, 461);
            this.labelaxis.Name = "labelaxis";
            this.labelaxis.Size = new System.Drawing.Size(32, 13);
            this.labelaxis.TabIndex = 34;
            this.labelaxis.Text = "Axis :";
            // 
            // labelKZ
            // 
            this.labelKZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelKZ.AutoSize = true;
            this.labelKZ.Location = new System.Drawing.Point(537, 461);
            this.labelKZ.Name = "labelKZ";
            this.labelKZ.Size = new System.Drawing.Size(27, 13);
            this.labelKZ.TabIndex = 35;
            this.labelKZ.Text = "KZ :";
            // 
            // labelTime
            // 
            this.labelTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(584, 461);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(36, 13);
            this.labelTime.TabIndex = 36;
            this.labelTime.Text = "Time :";
            // 
            // labelInfoAxe
            // 
            this.labelInfoAxe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelInfoAxe.AutoSize = true;
            this.labelInfoAxe.Location = new System.Drawing.Point(251, 478);
            this.labelInfoAxe.Name = "labelInfoAxe";
            this.labelInfoAxe.Size = new System.Drawing.Size(28, 13);
            this.labelInfoAxe.TabIndex = 37;
            this.labelInfoAxe.Text = "Info:";
            // 
            // TagSetsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(866, 496);
            this.Controls.Add(this.labelInfoAxe);
            this.Controls.Add(this.labelTime);
            this.Controls.Add(this.labelKZ);
            this.Controls.Add(this.labelaxis);
            this.Controls.Add(this.labelCpt);
            this.Controls.Add(this.statusLoopCheckbox);
            this.Controls.Add(this.resetListButton);
            this.Controls.Add(this.failedTagsDataGrid);
            this.Controls.Add(this.missingTagsLabel);
            this.Controls.Add(this.presentTagsLabel);
            this.Controls.Add(this.receiveTagsGroupBox);
            this.Controls.Add(this.assumeUnknownCheckBox);
            this.Controls.Add(this.correlationThresholdLabel);
            this.Controls.Add(this.correlationThresholdTextBox);
            this.Controls.Add(this.loopPeriodLabel);
            this.Controls.Add(this.loopPeriodTextBox);
            this.Controls.Add(this.loopCheckBox);
            this.Controls.Add(this.startStopScanButton);
            this.Controls.Add(this.tagListDataGrid);
            this.Controls.Add(this.closeButton);
            this.Name = "TagSetsDialog";
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.tagListDataGrid)).EndInit();
            this.receiveTagsGroupBox.ResumeLayout(false);
            this.receiveTagsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.failedTagsDataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.DataGridView tagListDataGrid;
        private System.Windows.Forms.Timer loopTimer;
		private System.Windows.Forms.Label correlationThresholdLabel;
		private System.Windows.Forms.TextBox correlationThresholdTextBox;
		private System.Windows.Forms.Label loopPeriodLabel;
		private System.Windows.Forms.TextBox loopPeriodTextBox;
        private System.Windows.Forms.CheckBox loopCheckBox;
		private System.Windows.Forms.Button startStopScanButton;
		//private System.Windows.Forms.CheckBox latchUnlockedCheckBox;
		private System.Windows.Forms.CheckBox assumeUnknownCheckBox;
		//private System.Windows.Forms.Label doorStateLabel;
		private System.Windows.Forms.GroupBox receiveTagsGroupBox;
		private System.Windows.Forms.RadioButton receiveTagsSynchronouslyDuringScanRadioButton;
		private System.Windows.Forms.RadioButton receiveTagsSynchronouslyAfterScanRadioButton;
		private System.Windows.Forms.RadioButton receiveTagsAsynchronouslyRadioButton;
		private System.Windows.Forms.Label presentTagsLabel;
		private System.Windows.Forms.Label missingTagsLabel;
		private System.Windows.Forms.DataGridView failedTagsDataGrid;
		private System.Windows.Forms.Button resetListButton;
		private System.Windows.Forms.DataGridViewTextBoxColumn TagIdColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn failReasonColumn;
		private System.Windows.Forms.CheckBox statusLoopCheckbox;
		private System.Windows.Forms.Timer statusLoopTimer;
        private System.Windows.Forms.CheckBox checkBoxDynamic;
        private System.Windows.Forms.CheckBox checkBoxReverse;
        private System.Windows.Forms.Label labelCpt;
        private System.Windows.Forms.Label labelaxis;
        private System.Windows.Forms.Label labelKZ;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn TagID;
        private System.Windows.Forms.DataGridViewTextBoxColumn SequentialPresent;
        private System.Windows.Forms.DataGridViewTextBoxColumn SequentialMissing;
        private System.Windows.Forms.DataGridViewTextBoxColumn PresentTotal;
        private System.Windows.Forms.DataGridViewTextBoxColumn MissingTotal;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnAxis;
        private System.Windows.Forms.Label labelInfoAxe;
	}
}