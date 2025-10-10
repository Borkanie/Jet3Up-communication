// Copyrigth (c) S.C.SoftLab S.R.L.
// All Rigths reserved.

using Aerotec.Data.Model;
using Jet3UpInterfaces.Factories;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace Implementation.Factories
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

        public static List<User> GetDefaultUsers()
        {
            return new List<User>
        {
            new User { Id = "BR 115", Name = "ALECU" },
            new User { Id = "BR 093", Name = "DOHOTARU" },
            new User { Id = "BR 041", Name = "BACIU" },
            new User { Id = "BR 110", Name = "NEGOESCU" },
            new User { Id = "BR 105", Name = "POENARIU" },
            new User { Id = "BR 066", Name = "CIREASA" },
            new User { Id = "BR 134", Name = "LAZAR" },
            new User { Id = "BR 137", Name = "ZAHARIA" },
            new User { Id = "BR 123", Name = "ADAM" }
        };
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
            var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"Aerotec");
            if(!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string jsonFilePath = Path.Combine(folderPath, "Controllers.json");
            if (!File.Exists(jsonFilePath))
            {
                var stream = File.Create(jsonFilePath);
                string json = System.Text.Json.JsonSerializer.Serialize(GetDefaultUsers(), new JsonSerializerOptions { WriteIndented = true });
                // Convert string to bytes
                byte[] bytes = Encoding.UTF8.GetBytes(json);

                // Write bytes to file
                stream.Write(bytes, 0, bytes.Length);

                stream.Close();
            }

            return jsonFilePath;
        }
    }
}
