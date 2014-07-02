using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minie.Carters.Data;
using Minie.Carters.Interfaces.Repositories;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Minie.Carters.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private MongoCollection<User> _collection = null;

        public UsersRepository(MongoDatabase db)
        {
            _collection = db.GetCollection<User>("users");
        }

        public User Find(string email)
        {
            return _collection.FindOne(Query<User>.EQ(u => u.Email, email));
        }

        public bool IsUniqueEmail(string email)
        {
            return !_collection.Find(Query<User>.EQ(u => u.Email, email.ToLowerInvariant())).Any();
        }

        public void Save(User user)
        {
            _collection.Save(user);
        }

        public bool ValidateUser(string email, string password)
        {
            User user = _collection.FindOne(Query<User>.EQ(u => u.Email, email));
            return user != null && PasswordHash.ValidatePassword(password, user.Password);
        }
    }
}
