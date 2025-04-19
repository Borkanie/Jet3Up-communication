using Jet3UpHelpers;

namespace Helpers
{
    public class Job
    {
        /// <summary>
        ///  The delay of the writing command in micrometers.
        /// </summary>
        public int Delay;

        /// <summary>
        /// The desired <see cref="FontSizeEnum"/> of the writing.
        /// </summary>
        public FontSizeEnum FontSize;

        /// <summary>
        /// The machine has two flags, 180 dgereees and mirrored.
        /// This are configured trough the ehternet interface using an angle of 0 90 180 or 270. 
        /// Any other values will result in unexpected behaviour.
        /// </summary>
        public int Rotation;

        /// <summary>
        /// The resolution of th used encoder. It is necessary for the job.
        /// </summary>
        public int EncoderResolution;

        /// <summary>
        /// Number of objects to print.
        /// </summary>
        public int ExpectedQuantity;

        /// <summary>
        /// Number of already printed lablels in thi sjob
        /// </summary>
        public int AlreadyPrinted { get; } = 0;

        public Dictionary<string, string> Objects { get; } = new Dictionary<string, string>();

    }
}
