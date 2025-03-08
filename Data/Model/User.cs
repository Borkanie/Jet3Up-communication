// Copyrigth (c) S.C.SoftLab S.R.L.
// All Rigths reserved.

namespace Aerotec.Data.Model
{
    /// <summary>
    /// A controller maning a machine.
    /// </summary>
    public class User
    {
        public User()
        {
            Id = "";
            Name = "";
        }

        public User(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The Identifier of the controller (A 000).
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Display name of the controller.
        /// </summary>
        public string Name { get; set; }
    }
}