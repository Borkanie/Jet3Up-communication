// Copyrigth (c) S.C.SoftLab S.R.L.
// All Rigths reserved.

using Aerotec.Data.Model;

namespace Jet3UpCommLib.Interfaces.Factories
{
    /// <summary>
    /// A factory allowing the user interaction with <see cref="IUserContainer"/>
    /// </summary>
    public interface IUserFactory
    {
        /// <summary>
        /// Get's a list of all the <see cref="User"/> from the json file.
        /// </summary>
        /// <returns></returns>
        void RevertChanges();

        /// <summary>
        /// Save the changes done to the <see cref="User"/> to the database.
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Returns the names of all the <see cref="User"/> from the system.
        /// </summary>
        /// <returns></returns>
        List<string> GetUserNames();

        /// <summary>
        /// Returns all the <see cref="User"/> from the system.
        /// </summary>
        /// <returns></returns>
        List<User> GetUsers();

        /// <summary>
        /// Creates a new empty <see cref="User"/>.
        /// </summary>
        /// <returns></returns>
        User Create();

        /// <summary>
        /// Creates a new <see cref="User"/>.
        /// </summary>
        /// <param name="name">The name that will be set to it.</param>
        /// <returns></returns>
        User Create(string name);

        /// <summary>
        /// Removes a user from the database and from the cache.
        /// </summary>
        /// <param name="user"></param>
        void Destroy(User user);
    }
}
