using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Minie.Carters.Data
{
    public class User
    {
        [BsonId]
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
