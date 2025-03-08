// Copyrigth (c) S.C.SoftLab S.R.L.
// All Rigths reserved.

using Aerotec.Data.Model;
using Jet3UpInterfaces.Factories;

namespace Jet3Up.Services.Mockup
{
    /// <inheritdoc/>
    public class UserFactoryMockup : IUserFactory
    {
        private List<User> DefaultUsers = new List<User>(){
        new User(){ Name = "1" , Id = "1"},
        new User(){ Name = "2", Id = "2" },
        new User(){ Name = "3", Id = "3" },
        new User(){ Name = "4", Id = "4" }
        };
        private List<User> users = new List<User>();

        public UserFactoryMockup()
        {
            users.AddRange(DefaultUsers);
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
            users.Remove(user);
            if (DefaultUsers.Contains(user))
            {
                DefaultUsers.Remove(user);
            }          
        }

        /// <inheritdoc/>
        public List<string> GetUserNames()
        {
            var names = new List<string>();
            foreach( var user in users)
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
            users.Clear();
            foreach (var user in DefaultUsers)
            {
                var dummy = new User()
                {
                    Name = user.Name,
                    Id = user.Id,
                };
                users.Add(dummy);
            }
        }

        /// <inheritdoc/>
        public void SaveChanges()
        {
            DefaultUsers.Clear();
            foreach(var user in users)
            {
                var dummy = new User()
                {
                    Name = user.Name,
                    Id = user.Id,
                };
                DefaultUsers.Add(dummy);
            }
        }

    }
}
