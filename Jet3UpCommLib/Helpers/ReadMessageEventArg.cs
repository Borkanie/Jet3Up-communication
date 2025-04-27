// Copyrigth (c) S.C.SoftLab S.R.L.
// All Rigths reserved.

namespace Jet3UpCommLib.Helpers
{
    /// <summary>
    /// A message has been read from the fileinterface.Used in mockup.
    /// </summary>
    public class ReadMessageEventArg : EventArgs
    {
        public string Text { get; }
        public ReadMessageEventArg(string text)
        {
            Text = text;
        }
    }
}