namespace SecurityModules.FingerprintReader
{
    public class FingerprintReaderEventArgs
    {
        /// <summary>
        /// Enumeration of possible notification
        /// </summary>
        public enum EventTypeValue
        {
            FPReaderFingerTouch,
            FPReaderFingerGone,
            FPReaderConnect,
            FPReaderDisconnect,
            FPReaderReadingComplete, // finished reading a fingerprint
            FPReaderFingerPress, // enrollment press
            FPReaderRegistrationSuccess, // enrollment succeeded
            FPReaderRegistrationFailure, // enrollment failed
        }

        private readonly EventTypeValue _eventType;

        /// <summary>
        /// Fingerprint reader type (master/slave) property.
        /// </summary>
        public bool IsMaster { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="isMaster"></param>
        public FingerprintReaderEventArgs(EventTypeValue eventType, bool isMaster = true)
        {
            _eventType = eventType;
            IsMaster = isMaster;
        }

        /// <summary>
        /// Event type property.
        /// </summary>
        /// <returns>A value of enum EventTypeValue</returns>
        public EventTypeValue EventType
        {
            get { return _eventType; }
        }
    }
}
