using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minie.Carters.Data;

namespace Minie.Carters.Interfaces.Repositories
{
    public interface ICategoriesRepository
    {
        void Save(Category category);
        IEnumerable<Category> FindAll();
    }
}
