// Copyrigth (c) S.C.SoftLab S.R.L.
// All Rigths reserved.

namespace Jet3UpHelpers
{
    /// <summary>
    /// Communication interrupted event.
    /// </summary>
    public class Jet3UpCommunicationInterruptedErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Error causing the interruption.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// The error appeared while reading the current counter.
        /// That will happen on a different thread and this flag will be used to kill it.
        /// </summary>
        public bool ErrorWhenReading { get; }

        public Jet3UpCommunicationInterruptedErrorEventArgs(Exception error, bool errorWhenReading)
        {
            Exception = error;
            ErrorWhenReading = errorWhenReading;
        }
    }
}
