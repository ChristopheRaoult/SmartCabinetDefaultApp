using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DPFP;
using DPFP.Capture;
using DPFP.Processing;
using DPFP.Verification;

namespace SecurityModules.FingerprintReader
{
    /// <summary>
    /// Used to raise FPReader events (in order to allow other classes to suscribe to them).
    /// </summary>
    /// <param name="sender">event sender</param>
    /// <param name="args">FingerArgs</param>
    public delegate void FingerprintReaderEventHandler(object sender, FingerprintReaderEventArgs args);

    public class FingerprintReader : DPFP.Capture.EventHandler
    {
        public event FingerprintReaderEventHandler FingerprintReaderEvent;

        private static bool _registrationInProgress;
        private static int _fingerCount;
        private const int FingerPressRequired = 4;

        public string SerialNumber { get; private set; }
        private readonly Capture _capturer;
        private readonly Enrollment _createRegTemplate;
        private readonly Verification _verify;
        private readonly FeatureSet[] _regFeatures;
        private FeatureSet _verFeatures;

        /// <summary>
        /// Last template created by enrolling process
        /// </summary>
        public Template LastTemplate { get; private set; }

        /// <summary>
        /// Last sample scanned by the reader. To be compared when a finger is detected.
        /// </summary>
        private Sample _lastSample;

        /// <summary>
        /// If a _capturer has been defined, then a reader is usable. Return true. Otherwise, return false.
        /// </summary>
        public bool Available
        {
            get { return (_capturer != null); }
        }

        /// <summary>
        /// Default ctor. Uses all available fingerprint readers.
        /// </summary>
        public FingerprintReader()
        {
            _regFeatures = new FeatureSet[FingerPressRequired];

            for (int i = 0; i < FingerPressRequired; i++)
            {
                _regFeatures[i] = new FeatureSet();
            }

            _verFeatures = new FeatureSet();
            _createRegTemplate = new Enrollment();

            _capturer = new Capture(Priority.Low)
                // no serial number => uses all readers. Priority.Low : fire events even if program has not the focus
            {
                EventHandler = this
            };

            _verify = new Verification();
        }


        /// <summary>
        /// Special ctor. Uses the fingerprint reader identified by given serialNumber.
        /// </summary>
        /// <param name="serialNumber">Serial number of the fingerprint reader to be used</param>
        public FingerprintReader(string serialNumber)
        {
            _regFeatures = new FeatureSet[FingerPressRequired];

            for (int i = 0; i < FingerPressRequired; i++)
            {
                _regFeatures[i] = new FeatureSet();
            }

            _verFeatures = new FeatureSet();
            _createRegTemplate = new Enrollment();

            var readers = new ReadersCollection();

            for (int i = 0; i < readers.Count; i++)
            {
                if (!readers[i].SerialNumber.Equals(serialNumber))
                {
                    continue;
                }

                _capturer = new Capture(readers[i].SerialNumber, Priority.Low)
                {
                    EventHandler = this
                };

                _verify = new Verification();
                SerialNumber = serialNumber;
                break;
            }
        }

        /// <summary>
        /// Get ReadersCollection from DPFP API in order to provide a list of serial numbers (of the plugged FP readers).
        /// </summary>
        /// <returns>List (string) of serial numbers.</returns>
        public static List<string> GetPluggedReadersSerialNumbers()
        {
            var readersCollection = new ReadersCollection();
            return readersCollection.Select(entry => entry.Value.SerialNumber).ToList();
        }

        public static string EncodeBase64Template(Template template)
        {
            if (template == null)
            {
                throw new ArgumentNullException();
            }

            var memoryStream = new MemoryStream();
            template.Serialize(memoryStream);
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public static Template DecodeBase64Template(string base64Template)
        {
            return new Template(new MemoryStream(Convert.FromBase64String(base64Template)));
        }

        /// <summary>
        /// Reset state variables and start capture thread
        /// </summary>
        /// <returns>True if Capture successfully started. False otherwise.</returns>
        public bool StartCapture()
        {
            _registrationInProgress = false;
            _fingerCount = 0;
            _createRegTemplate.Clear();

            try
            {
                if (_capturer != null)
                {
                    _capturer.StartCapture();
                    return true;
                }
            }
            catch (DPFP.Error.SDKException)
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Update state of Enrollment "mode" is On or Off.
        /// Changing the Enrollment Mode state allows controlling the behavior of the FP reader:
        /// Indeed, if enrollment is under process, sent events are different.
        /// </summary>
        /// <param name="state">If true, enrollment is on. If false, enrollment is off.</param>
        public void SetEnrollmentMode(bool state)
        {
            _registrationInProgress = state;
        }

        protected FeatureSet ExtractFeatures(Sample sample, DataPurpose purpose)
        {
            var extractor = new FeatureExtraction();
            var feedback = CaptureFeedback.None;
            var features = new FeatureSet();
            extractor.CreateFeatureSet(sample, purpose, ref feedback, ref features);

            return (feedback == CaptureFeedback.Good) ? features : null;
        }

        /// <summary>
        /// Used to authenticate a user's fingerprint (by testing his finger' template).
        /// </summary>
        /// <param name="templateToCheck">A DigitalPersona SDK Template instance, corresponding to a user's finger.</param>
        /// <returns>True if template match with last sample. False otherwise.</returns>
        public bool DoesTemplateMatch(Template templateToCheck)
        {
            var rslt = new Verification.Result();
            _verFeatures = ExtractFeatures(_lastSample, DataPurpose.Verification);
            _verify.Verify(_verFeatures, templateToCheck, ref rslt);

            return rslt.Verified;
        }

        /// <summary>
        /// Used to authenticate a user's fingerprint (by testing his finger' template).
        /// </summary>
        /// <param name="templateToCheck">A base64-encoded DigitalPersona SDK Template, corresponding to a user's finger.</param>
        /// <returns>True if template match with last sample. False otherwise.</returns>
        public bool DoesTemplateMatch(string templateToCheck)
        {
            var template = DecodeBase64Template(templateToCheck);

            if (template == null)
            {
                return false;
            }

            var rslt = new Verification.Result();
            _verFeatures = ExtractFeatures(_lastSample, DataPurpose.Verification);
            _verify.Verify(_verFeatures, template, ref rslt);

            return rslt.Verified;
        }

        /// <summary>
        /// Handle "Fingerprint reading Complete" event.
        /// If Enrollment (registration) is under process, consider the just-read sample as a new sample for the enrollment.
        /// Otherwise, save the sample as the "last read sample" in order to allow user to compare it with a fingerprints list.
        /// </summary>
        /// <param name="capture">Capture instance</param>
        /// <param name="info"></param>
        /// <param name="sample">Just-read fingerprint sample</param>
        public void OnComplete(object capture, string info, Sample sample)
        {
            if (!_registrationInProgress) // common authentication
            {
                // Notify that reading is complete. Suscriber has to confirm authentication by using DoesTemplateMatch method.
                _lastSample = sample;
                var args =
                    new FingerprintReaderEventArgs(FingerprintReaderEventArgs.EventTypeValue.FPReaderReadingComplete);
                FireEvent(args);
            }

            else
            {
                try
                {
                    _regFeatures[_fingerCount] = ExtractFeatures(sample, DataPurpose.Enrollment);

                    if (_regFeatures[_fingerCount] == null)
                    {
                        return;
                    }

                    ++_fingerCount;

                    _createRegTemplate.AddFeatures(_regFeatures[_fingerCount - 1]);

                    if (_fingerCount <= FingerPressRequired)
                    {
                        FireEvent(
                            new FingerprintReaderEventArgs(FingerprintReaderEventArgs.EventTypeValue.FPReaderFingerPress));
                    }


                    if (_createRegTemplate.TemplateStatus == Enrollment.Status.Failed)
                    {
                        _fingerCount = 0;
                        _registrationInProgress = false;

                        _createRegTemplate.Clear();
                        FireEvent(
                            new FingerprintReaderEventArgs(
                                FingerprintReaderEventArgs.EventTypeValue.FPReaderRegistrationFailure));
                    }

                    else
                    {
                        if (_createRegTemplate.TemplateStatus != Enrollment.Status.Ready)
                        {
                            return;
                        }

                        LastTemplate = _createRegTemplate.Template;
                        _registrationInProgress = false;
                        _fingerCount = 0;

                        _createRegTemplate.Clear();
                        FireEvent(
                            new FingerprintReaderEventArgs(
                                FingerprintReaderEventArgs.EventTypeValue.FPReaderRegistrationSuccess));
                    }
                }

                catch (DPFP.Error.SDKException sdke)
                {
                    if (sdke.ErrorCode == DPFP.Error.ErrorCodes.InvalidFeatureSet)
                    {
                        _registrationInProgress = false;
                        _fingerCount = 0;

                        _createRegTemplate.Clear();
                        FireEvent(
                            new FingerprintReaderEventArgs(
                                FingerprintReaderEventArgs.EventTypeValue.FPReaderRegistrationFailure));
                    }
                }
            }
        }

        public void OnFingerGone(object capture, string readerSerialNumber)
        {
            FireEvent(new FingerprintReaderEventArgs(FingerprintReaderEventArgs.EventTypeValue.FPReaderFingerGone));
        }

        public void OnFingerTouch(object capture, string readerSerialNumber)
        {
            FireEvent(new FingerprintReaderEventArgs(FingerprintReaderEventArgs.EventTypeValue.FPReaderFingerTouch));
        }

        public void OnReaderConnect(object capture, string readerSerialNumber)
        {
            FireEvent(new FingerprintReaderEventArgs(FingerprintReaderEventArgs.EventTypeValue.FPReaderConnect));
        }

        public void OnReaderDisconnect(object capture, string readerSerialNumber)
        {
            FireEvent(new FingerprintReaderEventArgs(FingerprintReaderEventArgs.EventTypeValue.FPReaderDisconnect));
        }

        public void OnSampleQuality(object capture, string readerSerialNumber, CaptureFeedback captureFeedback)
        {
            // ~ no use
        }

        private void FireEvent(FingerprintReaderEventArgs args)
        {
            var handler = FingerprintReaderEvent;

            if (handler != null)
            {
                handler(this, args);
            }
        }
    }

    public enum FingerIndex
    { 
        LeftPinky      = 0,
        LeftRing       = 1,
        LeftMiddle     = 2,
        LeftIndex      = 3,
        LeftThumb      = 4,
        RightThumb     = 5,
        RightIndex     = 6,
        RightMiddle    = 7,
        RightRing      = 8,
        RightPinky     = 9
    }
}
