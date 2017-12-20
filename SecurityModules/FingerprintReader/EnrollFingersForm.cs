using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using DPFP.Gui;
using LTWDatabase.Entities;
using SecurityModules.FingerprintReader;

namespace LTW_DeviceManagement.forms
{
    public partial class EnrollFingersForm : KryptonForm
    {
        private static readonly Dictionary<FingerIndex, int> FingerIndexToMaskValue = new Dictionary<FingerIndex, int>
        {
            { FingerIndex.LeftPinky, 1 },
            { FingerIndex.LeftRing, 2 },
            { FingerIndex.LeftMiddle, 4 },
            { FingerIndex.LeftIndex, 8 },
            { FingerIndex.LeftThumb, 16 },
            { FingerIndex.RightThumb, 32 },
            { FingerIndex.RightIndex, 64 },
            { FingerIndex.RightMiddle, 128 },
            { FingerIndex.RightRing, 256 },
            { FingerIndex.RightPinky, 512 }
        };

        // DigitalPersona .NET SDK uses different indexes. 
        // We use 0 to 9 for left pinky to right pinky. They use 1 to 5 for right hand (from thumb to pinky) and 6 to 10 for left hand (from thumb to pinky).
        private static readonly Dictionary<int, FingerIndex> DpfpFingerNbrToFingerIndex = new Dictionary
            <int, FingerIndex>
        {
            {10, FingerIndex.LeftPinky},
            {9, FingerIndex.LeftRing},
            {8, FingerIndex.LeftMiddle},
            {7, FingerIndex.LeftIndex},
            {6, FingerIndex.LeftThumb},

            {1, FingerIndex.RightThumb},
            {2 ,FingerIndex.RightIndex},
            {3, FingerIndex.RightMiddle},
            {4, FingerIndex.RightRing},
            {5, FingerIndex.RightPinky}
        };

        private GrantedUser _currentUser;

        public EnrollFingersForm(GrantedUser user)
        {
            InitializeComponent();

            var serialNumbers = FingerprintReader.GetPluggedReadersSerialNumbers();

            if (serialNumbers.Count == 0)
            {
                KryptonMessageBox.Show("No fingerprint reader available.", "Module not Found!");
                Close();
                return;
            }

            enrollmentControl.ReaderSerialNumber = serialNumbers[0];
            _currentUser = user;

            labelUsername.Text = _currentUser.Login;

            foreach (var fp in _currentUser.Fingerprints)
            {
                enrollmentControl.EnrolledFingerMask += FingerIndexToMaskValue[(FingerIndex)fp.Index];
            }
        }

        private void enrollmentControl_OnEnroll(object control, int fingerNbr, DPFP.Template template, ref EventHandlerStatus eventHandlerStatus)
        {
            if (eventHandlerStatus == EventHandlerStatus.Failure)
            {
                KryptonMessageBox.Show("Finger enrollment failed.", "Enrollment Failure", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var fingerIndex = DpfpFingerNbrToFingerIndex[fingerNbr];

            string base64EncodedTemplate = FingerprintReader.EncodeBase64Template(template);

            using (var db = new LTWModelContext(LocalDatabase.GetConnectionString()))
            {
                db.Fingerprint.Add(new Fingerprint
                {
                    Index = (int) fingerIndex,
                    GrantedUserId = _currentUser.GrantedUserId,
                    Template = base64EncodedTemplate
                });

                db.SaveChanges();
            }
        }

        private void enrollmentControl_OnDelete(object control, int fingerNbr, ref EventHandlerStatus eventHandlerStatus)
        {
            if (eventHandlerStatus == EventHandlerStatus.Failure)
            {
                KryptonMessageBox.Show("Finger deletion failed.", "Deletion Failure", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var fingerIndex = DpfpFingerNbrToFingerIndex[fingerNbr];

            using (var db = new LTWModelContext(LocalDatabase.GetConnectionString()))
            {
                var fingerprint =
                    db.Fingerprint.FirstOrDefault(
                        fp => fp.Index == (int) fingerIndex && fp.GrantedUserId == _currentUser.GrantedUserId);

                if (fingerprint == null)
                {
                    return;
                }

                db.Fingerprint.Remove(fingerprint);
                db.SaveChanges();
            }
        }
    }
}
