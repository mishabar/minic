using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minie.Carters.Data;

namespace Minie.Carters.Interfaces.Repositories
{
    public interface IUsersRepository
    {
        User Find(string email);
        bool IsUniqueEmail(string email);
        void Save(User user);

        bool ValidateUser(string p1, string p2);
    }
}
