namespace Jet3UpCommLib.Helpers
{
    public interface Job
    {
        /// <summary>
        ///  The delay of the writing command in micrometers.
        /// </summary>
        public int Delay { get; set; }

        /// <summary>
        /// The desired <see cref="FontSizeEnum"/> of the writing.
        /// </summary>
        public FontSizeEnum FontSize { get; set; }

        /// <summary>
        /// The machine has two flags, 180 dgereees and mirrored.
        /// This are configured trough the ehternet interface using an angle of 0 90 180 or 270. 
        /// Any other values will result in unexpected behaviour.
        /// </summary>
        public int Rotation { get; set; }

        /// <summary>
        /// The resolution of th used encoder. It is necessary for the job.
        /// </summary>
        public int EncoderResolution { get; set; }

        /// <summary>
        /// Number of objects to print.
        /// </summary>
        public int ExpectedQuantity { get; set; }

        /// <summary>
        /// Number of already printed lablels in thi sjob
        /// </summary>
        public int CurrentlyPrinted { get; set; }

        /// <summary>
        /// The list of messages the job has. It can use keys to map them.
        /// </summary>
        public Dictionary<string, string> Objects { get; }

        /// <summary>
        /// Get's the job's inititalizaiton  message.
        /// </summary>
        /// <returns></returns>
        public string getJobConfigurationMessage();
    }
}
