using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minie.Carters.Data;

namespace Minie.Carters.Interfaces.Repositories
{
    public interface IProductsRepository
    {
        void Save(Product product);

        IEnumerable<Product> GetByCategory(string category, string[] sizes, string order, int page, out int pages);

        Product Get(string sku);
    }
}
