using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Minie.Carters.Data
{
    public class Category
    {
        [BsonId]
        public string CategoryID { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string OriginUrl { get; set; }
        public int Order { get; set; }
        [BsonIgnoreIfNull]
        public List<Category> SubCategories { get; set; }
        public string[] Sizes { get; set; }
    }
}
