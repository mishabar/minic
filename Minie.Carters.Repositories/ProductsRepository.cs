using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minie.Carters.Data;
using Minie.Carters.Interfaces.Repositories;
using Minie.Carters.Repositories;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Minie.Carters.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private MongoCollection<Product> _collection = null;

        public ProductsRepository(MongoDatabase db)
        {
            _collection = db.GetCollection<Product>("products");
        }

        public void Save(Product product)
        {
            _collection.Save(product);
        }

        public IEnumerable<Product> GetByCategory(string category, string[] sizes, string order, int page, out int pages)
        {
            IMongoQuery query = Query<Product>.EQ(p => p.Category, category);
            if (sizes.Length > 0)
            {
                query = Query.And(query, Query<Product>.In(p => p.Sizes, sizes));
            }
            else
            {
                query = Query.And(query, Query<Product>.Where(p => p.Sizes.Count > 0));
            }
            
            long count = _collection.Count(query);
            pages = 0;
            if (count > 0)
            {
                pages = (int)(count / 24) + 1;
            }

            switch (order)
            {
                case "phl":
                    return _collection.Find(query).OrderByDescending(p => p.Price).Skip(24 * (page - 1)).Take(24);
                    
                case "plh":
                default:
                    return _collection.Find(query).OrderBy(p => p.Price).Skip(24 * (page - 1)).Take(24);
            }
        }

        public Product Get(string sku)
        {
            return _collection.FindOne(Query<Product>.EQ(p => p.SKU, sku));
        }

        public void DeleteOutdated(long timestamp)
        {
            IMongoQuery query = Query<Product>.LT(p => p.Timestamp, timestamp);
            _collection.Remove(query, RemoveFlags.None);
        }
    }
}
