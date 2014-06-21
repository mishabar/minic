using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Minie.Carters.Data
{
    public class Product
    {
        [BsonId]
        public string SKU { get; set; }
        public string Name { get; set; }
        public float? MSRP { get; set; }
        public float Price { get; set; }
        public string Image { get; set; }
        public bool Active { get; set; }
        public string Category { get; set; }
        public List<string> Sizes { get; set; }
        public bool Invalid { get; set; }
    }

    public static class ProductExtensions
    {
        public static OrderItem ToOrderItem(this Product product, string size)
        {
            return new OrderItem { SKU = product.SKU, Name = product.Name, Price = product.Price, ImageUrl = product.Image, Quantity = 1, Size = size };
        }
    }
}
