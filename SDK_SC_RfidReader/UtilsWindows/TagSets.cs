using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


using SDK_SC_RfidReader.DeviceBase;

namespace SDK_SC_RfidReader.UtilsWindows
{
	public partial class TagSetsDialog : Form
	{
        private readonly DeviceRfidBoard device;		

		private readonly Hashtable tagList = new Hashtable();
		private bool bLoopTimerExpired = false;
		private bool bScanRequested = false;
		private bool bScanActive = false;
		private bool bAsynchronousTagUpdates = false;
		private bool bKnownTagsCleared = false;
		private uint inventorySequence = 0; // The sequence number of the inventory pass.
		private bool bCancelLooping = false;
		private bool bScanningActive = false;
        private int cptCount;
        private byte checkKZout = 0;
        DateTime startTime;
        DateTime stopTime;
        private int Axis = 0;
        private int[] nbTagPerAxis;
		private new class Tag
		{
			public readonly UInt64 id;
			public readonly DataGridViewRow row;
			public uint inventorySequence = 0; // When the tag was last read.
			public bool bTagAdded = false;
			public bool bPresent = true;
			public uint presentCount = 0; // How many sequential inventories detected this tag as present.
			public uint missingCount = 0; // How many sequential inventories detected this tag as missing.
			public uint presentTotal = 0; // How many total inventories detected this tag as present.
			public uint missingTotal = 0; // How many total inventories detected this tag as missing.

			public Tag(UInt64 id, DataGridViewRow row)
			{
				this.id = id;
				this.row = row;
			}
		};
        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        public TagSetsDialog(DeviceRfidBoard device)
		{
			this.device = device;
			

			MyDebug.Assert(this.device != null);

			InitializeComponent();
			this.Text = string.Format("Tag Management [{0:X8}]", device.DeviceId);
			correlationThresholdLabel.Text = string.Format("Correlation Threshold ({0}-{1})",
                DeviceRfidBoard.MIN_CorrelationThreshold, DeviceRfidBoard.MAX_CorrelationThreshold);

			// Set the Tag ID column to sort in ascending order.
			tagListDataGrid.Columns[0].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
            loopPeriodTextBox.Text = (string)Application.UserAppDataRegistry.GetValue(GetType().Name + "LoopPeriod", "0.5");
            loopCheckBox.Checked = ((int)Application.UserAppDataRegistry.GetValue(GetType().Name + "LoopEnabled", 0) != 0);
            assumeUnknownCheckBox.Checked = ((int)Application.UserAppDataRegistry.GetValue(GetType().Name + "AssumeUnknownTags", 0) != 0);
            switch ((int)Application.UserAppDataRegistry.GetValue(GetType().Name + "ReceiveTags", 0))
			{
				case 0:
				default:
					receiveTagsAsynchronouslyRadioButton.Checked = true;
					break;
				case 1:
					receiveTagsSynchronouslyAfterScanRadioButton.Checked = true;
					break;
				case 2:
					receiveTagsSynchronouslyDuringScanRadioButton.Checked = true;
					break;
			}

			base.OnLoad(e);

			// Get the latch status and display it.
			PbRspGetStatus status = (PbRspGetStatus)device.getStatus();
			
			// Get the device's current correlation threshold. This value is saved in Flash ROM on the
			// device and may be different per device.
			byte correlationThreshold = device.getCorrelationThreshold();
			Debug.Assert(correlationThreshold > 0);
			correlationThresholdTextBox.Text = correlationThreshold.ToString();

			
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
		protected override void OnClosing(CancelEventArgs e)
		{
			

			double fInterval;
			if(double.TryParse(loopPeriodTextBox.Text, out fInterval))
                Application.UserAppDataRegistry.SetValue(GetType().Name + "LoopPeriod", loopPeriodTextBox.Text);
            Application.UserAppDataRegistry.SetValue(GetType().Name + "LoopEnabled", loopCheckBox.Checked ? 1 : 0);
            Application.UserAppDataRegistry.SetValue(GetType().Name + "AssumeUnknownTags", assumeUnknownCheckBox.Checked ? 1 : 0);
			if(receiveTagsSynchronouslyAfterScanRadioButton.Checked)
                Application.UserAppDataRegistry.SetValue(GetType().Name + "ReceiveTags", 1);
			else if(receiveTagsSynchronouslyDuringScanRadioButton.Checked)
                Application.UserAppDataRegistry.SetValue(GetType().Name + "ReceiveTags", 2);
			else
                Application.UserAppDataRegistry.SetValue(GetType().Name + "ReceiveTags", 0);

			base.OnClosing(e);
		}

		private void TagScanActive(bool bActive)
		{
			if(bActive)
			{
				bScanningActive = true;
				Invoke((MethodInvoker)delegate {startStopScanButton.Text = "&Stop";});
			}
			else
			{
				bScanningActive = false;
                Invoke((MethodInvoker)delegate { startStopScanButton.Text = "&Start"; });
			}
		}

        private Tag findOrAddTag(UInt64 tagID, bool useSPCE2)
        {
            Tag tag = (Tag)tagList[tagID];
            if (tag == null)
            {
                int rowIndex;
                if (useSPCE2)
                {
                    string codeOct = SerialRFID.SerialNumberAsString(tagID);
                    if (codeOct.StartsWith("3"))
                         rowIndex = tagListDataGrid.Rows.Add(SerialRFID.SerialNumberAsAlphaString(tagID,TagType.TT_SPCE2_RO), 0, 0, 0, 0, Axis);
                    else
                        rowIndex = tagListDataGrid.Rows.Add(SerialRFID.SerialNumberAsAlphaString(tagID, TagType.TT_SPCE2_RW), 0, 0, 0, 0, Axis);
                        
                    
                }
                else
                    rowIndex = tagListDataGrid.Rows.Add(SerialRFID.SerialNumberAsString(tagID), 0, 0, 0, 0, Axis);
                tag = new Tag(tagID, tagListDataGrid.Rows[rowIndex]);
                tagListDataGrid.FirstDisplayedScrollingRowIndex = rowIndex;
                tagList.Add(tag.id, tag);
            }
            else
            {
                tagListDataGrid.Rows[tag.row.Index].Cells[5].Value = Axis;
            }

            return tag;
        }

		static double[] Fades = {0.0, 0.0, 0.3, 0.5, 0.6, 0.7, 0.75, 0.8, 0.85, 0.9, 0.95};
		private Color FadeColor(Color initialColor, Color finalColor, uint stepOffset)
		{
			if(stepOffset >= Fades.Length)
				return finalColor;
			int red   = (int)(initialColor.R + Fades[stepOffset] * (finalColor.R - initialColor.R));
			int green = (int)(initialColor.G + Fades[stepOffset] * (finalColor.G - initialColor.G));
			int blue  = (int)(initialColor.B + Fades[stepOffset] * (finalColor.B - initialColor.B));
			return Color.FromArgb(red, green, blue);
		}

	

        private void updateNbTagPerAxis()
        {
            string str = string.Empty;

            for (int loop = 1; loop <= 9; loop++)
            {
                string tmp = " Axis " + (loop).ToString() + ": " + nbTagPerAxis[loop].ToString() + "  -  ";
                str += tmp;
            }
            Invoke((MethodInvoker)delegate { labelInfoAxe.Text = str; });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncEventMessage"></param>
        public void handleAsyncEvent(AsyncEventMessage asyncEventMessage)
		{
			switch(asyncEventMessage.asyncEventType)
			{
				//case AsyncEventType.PBET_TagAdded
                case AsyncEventType.PBET_TagAddedR8:
				{
					PBAE_RfidTagAdded tagAddedMessage = (PBAE_RfidTagAdded)Utilities.MarshalToStruct(
						asyncEventMessage.serialMessage, typeof(PBAE_RfidTagAdded));
                    Tag tag = findOrAddTag(SerialRFID.SerialNumber(tagAddedMessage.serialNumber),false);
					tag.inventorySequence = inventorySequence;
					tag.bTagAdded = true;

                    nbTagPerAxis[Axis]++;
                    updateNbTagPerAxis();
                       cptCount++;
                       Invoke((MethodInvoker)delegate {labelCpt.Text = "Count: " + cptCount.ToString();});
                       checkKZout = 0; // raz cpt  test sortie
                       Invoke((MethodInvoker)delegate { labelKZ.Text = "KZ : " + checkKZout.ToString(); });
					break;
				}

				//case AsyncEventType.PBET_TagRemoved:
                case AsyncEventType.PBET_TagAddedSPCE2_RO:
                case AsyncEventType.PBET_TagAddedSPCE2_RW:
				{
                    PBAE_RfidTagAdded tagAddedMessage = (PBAE_RfidTagAdded)Utilities.MarshalToStruct(
                         asyncEventMessage.serialMessage, typeof(PBAE_RfidTagAdded));
                    Tag tag = findOrAddTag(SerialRFID.SerialNumber(tagAddedMessage.serialNumber),true);
                    tag.inventorySequence = inventorySequence;
                    tag.bTagAdded = true;

                    nbTagPerAxis[Axis]++;
                    updateNbTagPerAxis();
                    cptCount++;
                    Invoke((MethodInvoker)delegate { labelCpt.Text = "Count: " + cptCount.ToString(); });
                    checkKZout = 0; // raz cpt  test sortie
                    Invoke((MethodInvoker)delegate { labelKZ.Text = "KZ : " + checkKZout.ToString(); });
                    break;
				}

                case AsyncEventType.PBET_BackDoorInfo:
                {
                    PBAE_BackDoorInfo backDoorPacket = (PBAE_BackDoorInfo)Utilities.MarshalToStruct(
                      asyncEventMessage.serialMessage, typeof(PBAE_BackDoorInfo));
                    if (backDoorPacket.backDoorEventType == (byte)BackDoorEventType.BDET_AxisChange)

                        if ((backDoorPacket.value1 < 10) && (backDoorPacket.value2 > 20))
                        {
                            Axis = backDoorPacket.value1;
                            Invoke((MethodInvoker)delegate {labelaxis.Text = "Axis : " + backDoorPacket.value1.ToString();});
                            Invoke((MethodInvoker)delegate { labelaxis.Refresh(); });
                            stopTime = DateTime.Now;
                            TimeSpan duration = stopTime - startTime;
                            string time = duration.ToString();
                            double rate = cptCount / duration.TotalSeconds;
                            if (time.Length > 12)
                                Invoke((MethodInvoker)delegate { labelTime.Text = "Time : " + time.Substring(0, 12) + " - Tags/s : " + String.Format("{0:0.00}", rate); ; });
                        }

                    if ((backDoorPacket.value1 == 10) && (backDoorPacket.value2 == 1))
                        checkKZout++;
                    Invoke((MethodInvoker)delegate { labelKZ.Text = "KZ : " + checkKZout.ToString(); });
                    break;
                }

				case AsyncEventType.PBET_RfidScanStateChanged:
				{
					PBAE_RfidScanStateChanged scanStateChangedMessage = (PBAE_RfidScanStateChanged)Utilities.MarshalToStruct(
						asyncEventMessage.serialMessage, typeof(PBAE_RfidScanStateChanged));

					// Switch on the new scan state.
					switch((ScanStatusType)scanStateChangedMessage.scanStatus)
					{
						case ScanStatusType.SS_TagScanStarted: // Cabinet scan has begun.
						{
							//KB000: This could happen due to a Pyxibus bug where message with broken ACK is delivered twice.
							// MyDebug.Assert(bScanActive);

                            nbTagPerAxis = new int[10];
                            updateNbTagPerAxis();
                            startTime = DateTime.Now;
                            checkKZout = 1;
                            Invoke((MethodInvoker)delegate { labelKZ.Text = "KZ : " + checkKZout.ToString(); });
							if(!bScanActive)
							{
								bScanRequested = false;
								bScanActive = true;
                               /* if(receiveTagsSynchronouslyDuringScanRadioButton.Checked)
                                {
	                                bool bFirstTag = true;
	                                uint tagIndex;
	                                uint tagCount;
	                                UInt64 tagID;
	                                while(device.getNextTag(bFirstTag, out tagID, out tagIndex, out tagCount))
	                                {
		                                bFirstTag = false;
		                                Tag tag = findOrAddTag(tagID);
	                                }
                                }*/
							}
							break;
						}

						case ScanStatusType.SS_TagScanCanceledByHost: // Cabinet scan canceled by host.
							//KB000: This could happen due to a Pyxibus bug where message with broken ACK is delivered twice.
							// MyDebug.Assert(bScanActive);
                        checkKZout = 255;
                        Invoke((MethodInvoker)delegate { labelKZ.Text = "KZ : " + checkKZout.ToString(); });
							if(bScanActive)
							{
								bScanActive = false;
								bCancelLooping = true;
								TagScanActive(false);
							}
							break;

						case ScanStatusType.SS_TagScanCanceledByDoorOpen: // Cabinet scan canceled due to door opening.
							//KB000: This could happen due to a Pyxibus bug where message with broken ACK is delivered twice.
							// MyDebug.Assert(bScanActive);
							if(bScanActive)
							{
								bScanActive = false;
								bCancelLooping = true;
								MessageBox.Show("Tag scan canceled by opening door.");
								TagScanActive(false);
							}
							break;

						case ScanStatusType.SS_TagScanFailedByUnrecoverableError: // Cabinet scan failed due to unrecoverable error.
							//KB000: This could happen due to a Pyxibus bug where message with broken ACK is delivered twice.
							// MyDebug.Assert(bScanActive);
							if(bScanActive)
							{
								bScanActive = false;
								bCancelLooping = true;
								MessageBox.Show("Tag scan failed due to unrecoverable error: " +
									((UnrecoverableErrorType)scanStateChangedMessage.info).ToString());
								TagScanActive(false);
							}
							break;
                        case ScanStatusType.SS_TagScanSendPourcent:
                            break;

						case ScanStatusType.SS_TagScanCompleted: // Cabinet scan has completed
						{
							//KB000: This could happen due to a Pyxibus bug where message with broken ACK is delivered twice.
							// MyDebug.Assert(bScanActive);
                            stopTime = DateTime.Now;
                            TimeSpan duration = stopTime - startTime;
                            string time = duration.ToString();
                            double rate = cptCount / duration.TotalSeconds;
                            if (time.Length > 12)
                                Invoke((MethodInvoker)delegate { labelTime.Text = "Time : " + time.Substring(0, 12) + " - Tags/s : " + String.Format("{0:0.00}", rate); ; });
                        
							if(bScanActive)
							{
							
									foreach(Tag tag in tagList.Values)
									{
										if(bKnownTagsCleared)
										{
											tag.bPresent = (tag.inventorySequence == inventorySequence);
											Debug.Assert(!tag.bPresent || tag.bTagAdded); // When known tags are cleared, the only tag reports should be tag-added.
										}
										else if(tag.inventorySequence == inventorySequence)
										{
											// The tag reported a change in the presence state.
											tag.bPresent = tag.bTagAdded;
										}
									}
								
                                uint presentTags = 0;
								uint missingTags = 0;
								foreach(Tag tag in tagList.Values)
								{
									if(tag.bPresent)
									{
										tag.missingCount = 0;
										tag.presentCount++;
										tag.presentTotal++;
										presentTags++;
									}
									else
									{
										tag.missingCount++;
										tag.missingTotal++;
										tag.presentCount = 0;
										missingTags++;
									}

									tag.row.Cells[1].Value = tag.presentCount.ToString();
									tag.row.Cells[2].Value = tag.missingCount.ToString();
									tag.row.Cells[3].Value = tag.presentTotal.ToString();
									tag.row.Cells[4].Value = tag.missingTotal.ToString();

									// Update the row color to give a visual indication of sequential present/missing state.
									if(!tag.bPresent)
										tag.row.DefaultCellStyle.BackColor = FadeColor(Color.Red, Color.Gray, tag.missingCount);
									else if(tag.presentTotal == tag.presentCount)
										tag.row.DefaultCellStyle.BackColor = Color.White;
									else
										tag.row.DefaultCellStyle.BackColor = FadeColor(Color.Green, Color.White, tag.presentCount);
								}

								Invoke((MethodInvoker)delegate {presentTagsLabel.Text = "Present: " + presentTags.ToString();});
                                Invoke((MethodInvoker)delegate { missingTagsLabel.Text = "Missing: " + missingTags.ToString(); });

								

								bScanActive = false;
								// If too many tags were detected, show a warning.
								if((scanStateChangedMessage.info & (byte)TagScanCompleteInfoFlagsType.TSCF_TooManyTags) != 0)
								{
									bCancelLooping = true;
									MessageBox.Show("Too many tags detected.");
									TagScanActive(false);
								}

								// If looping, start again.
								else if(loopCheckBox.Checked && !bCancelLooping)
								{
									if(bLoopTimerExpired)
										doInventory();
								}
								else
									TagScanActive(false);
							}
							break;
						}
					}

					break;
				}

				
                 
			}
		}

		private bool doInventory()
		{
			bLoopTimerExpired = false;

            foreach (Tag tag in tagList.Values)  // reset all due to asynchronous event with no remove
                tag.bTagAdded = false;

			// Get the Tag-Scan command options from the corresponding checkboxes.
			bAsynchronousTagUpdates = receiveTagsAsynchronouslyRadioButton.Checked;
			bKnownTagsCleared = assumeUnknownCheckBox.Checked;

			inventorySequence++;
			if(bKnownTagsCleared)
				device.clearKnownTagsBeforeTagScan();
			if(device.startTagScan(true, bAsynchronousTagUpdates , checkBoxReverse.Checked,checkBoxDynamic.Checked))
			{
				// Tag scan has started. Scan events are handled in handleAsyncEvent().
				bScanRequested = true;
				return true;
			}

			bCancelLooping = true;

			// Attempt to get more information about the failure.
			string failureReason = device.tagScanFailureReason();
			if(failureReason.Length > 0)
				MessageBox.Show("Unable to start inventory; " + failureReason);
			else
				MessageBox.Show("Start inventory failed.");
			return false;
		}

		private void OnStartStopScanClicked(object sender, EventArgs e)
		{
			startStopScanButton.Focus();
			if(bScanningActive)
			{
				device.control(false, true);
				bCancelLooping = true;
				TagScanActive(false);
			}
			else
			{
				bCancelLooping = false;
                cptCount = 0;
                labelCpt.Text = "Count: 0";
                presentTagsLabel.Text = "Present: 0"; 
                missingTagsLabel.Text = "Missing: 0";
				if(doInventory())
				{
					TagScanActive(true);
					loopTimer.Enabled = true;
					loopTimer.Start();
				}
			}
            failedTagsDataGrid.Rows.Clear();
		}

		private void OnLoopPeriodChanged(object sender, EventArgs e)
		{
			int interval = 200;
			double fInterval;
			if(double.TryParse(loopPeriodTextBox.Text, out fInterval))
			{
				fInterval *= 1000.0; // Convert to milliseconds.
				if(fInterval > int.MaxValue)
					interval = int.MaxValue;
				else if(fInterval < 10)
					interval = 10;
				else
					interval = (int)fInterval;
				loopTimer.Interval = interval;
			}
		}

		private void OnCorrelationThresholdChanged(object sender, EventArgs e)
		{
			int threshold;
			if(int.TryParse(correlationThresholdTextBox.Text, out threshold))
			{
				byte correlationThreshold;
				if(threshold < DeviceRfidBoard.MIN_CorrelationThreshold)
                    correlationThreshold = DeviceRfidBoard.MIN_CorrelationThreshold;
                else if (threshold > DeviceRfidBoard.MAX_CorrelationThreshold)
                    correlationThreshold = DeviceRfidBoard.MAX_CorrelationThreshold;
				else
					correlationThreshold = (byte)threshold;
				device.setCorrelationThreshold(correlationThreshold);
			}
		}

		private void OnLoopTimerTick(object sender, EventArgs e)
		{
			if(loopCheckBox.Checked)
			{
				if(!bScanActive && !bScanRequested && !bCancelLooping)
					doInventory();
				else
					bLoopTimerExpired = true;
			}
			else
				loopTimer.Stop();
		}
	

		private void OnCloseClick(object sender, EventArgs e)
		{
			Close();
		}

		private void OnResetListClick(object sender, EventArgs e)
		{
			device.clearKnownTagsBeforeTagScan();
			failedTagsDataGrid.Rows.Clear();
			tagListDataGrid.Rows.Clear();
			tagList.Clear();
		}

		private void OnStatusLoopTimerTick(object sender, EventArgs e)
		{
			if(statusLoopCheckbox.Checked)
				device.getStatus();
		}
	}
}