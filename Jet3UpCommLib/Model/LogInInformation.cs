// Copyrigth (c) S.C.SoftLab S.R.L.
// All Rigths reserved.

using System.Net;

namespace Aerotec.Data.Model
{
    /// <summary>
    /// Information used to log in to the device.
    /// It wil be used to identify current user.
    /// </summary>
    public class LogInInformation
    {
        public LogInInformation(User user, string ip)
        {
            User = user;
            Address = IPAddress.Parse(ip);
        }

        /// <summary>
        /// User logging in to the machine.
        /// </summary>
        public User User { get; }

        /// <summary>
        /// IP adress used to communicate to the device.
        /// </summary>
        public IPAddress Address { get; }
    }
}
