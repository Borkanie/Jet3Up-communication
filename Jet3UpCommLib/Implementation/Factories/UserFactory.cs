// Copyrigth (c) S.C.SoftLab S.R.L.
// All Rigths reserved.

using Aerotec.Data.Model;
using Jet3UpCommLib.Interfaces.Factories;
using Newtonsoft.Json;

namespace Jet3UpCommLib.Implementation.Factories
{
    /// <inheritdoc/>
    public class UserFactory : IUserFactory
    {
        private List<User> users = new();

        public UserFactory()
        {
            RevertChanges();
        }

        /// <inheritdoc/>
        public User Create()
        {
            var user = new User();
            users.Add(user);
            return user;
        }

        /// <inheritdoc/>
        public User Create(string name)
        {
            var user = new User(name);
            users.Add(user);
            return user;
        }

        /// <inheritdoc/>
        public void Destroy(User user)
        {
            try
            {
                users.Remove(user);
                string jsonFilePath = getUserDataFilePath();
                string json = File.ReadAllText(jsonFilePath);
                var currentUserBase = JsonConvert.DeserializeObject<List<User>>(json);

                if (currentUserBase != null && currentUserBase.Contains(user))
                {
                    currentUserBase.Remove(user);
                    string serializedJson = JsonConvert.SerializeObject(currentUserBase, Formatting.Indented);
                    File.WriteAllText(jsonFilePath, serializedJson);
                    users.Remove(user);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <inheritdoc/>
        public List<string> GetUserNames()
        {
            var names = new List<string>();
            foreach (var user in users)
            {
                names.Add(user.Name);
            }
            return names;
        }

        /// <inheritdoc/>
        public List<User> GetUsers()
        {
            return users;
        }

        /// <inheritdoc/>
        public void RevertChanges()
        {
            try
            {
                string jsonFilePath = getUserDataFilePath();
                string json = File.ReadAllText(jsonFilePath);

                var defaultUsers = JsonConvert.DeserializeObject<List<User>>(json);
                if (defaultUsers == null)
                {
                    return;
                }
                users.Clear();
                foreach (var user in defaultUsers)
                {
                    var dummy = new User()
                    {
                        Name = user.Name,
                        Id = user.Id,
                    };
                    users.Add(dummy);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <inheritdoc/>
        public void SaveChanges()
        {
            try
            {
                string jsonFilePath = getUserDataFilePath();
                string serializedJson = JsonConvert.SerializeObject(users, Formatting.Indented);
                File.WriteAllText(jsonFilePath, serializedJson);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static string getUserDataFilePath()
        {
            string jsonFilePath = AppContext.BaseDirectory + "Resources\\Controllers.json";

            if (!File.Exists(jsonFilePath))
            {
                File.Create(jsonFilePath);
            }

            return jsonFilePath;
        }
    }
}
