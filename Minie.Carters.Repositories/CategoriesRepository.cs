using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minie.Carters.Data;
using Minie.Carters.Interfaces.Repositories;
using Minie.Carters.Repositories;
using MongoDB.Driver;

namespace Minie.Carters.Repositories
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private MongoCollection<Category> _collection = null;

        public CategoriesRepository(MongoDatabase db)
        {
            _collection = db.GetCollection<Category>("categories");
        }

        public void Save(Category category)
        {
            _collection.Save(category);
        }

        public IEnumerable<Category> FindAll()
        {
            return _collection.FindAll().OrderBy(c => c.Order).AsEnumerable();
        }
    }
}
