﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Minie.Carters.Data
{
    public class Order
    {
        public ObjectId Id { get; set; }
        public string UserId { get; set; }
        public string SessionId { get; set; }
        public DateTime CreatedOn { get; set; }
        public IList<OrderItem> Items { get; set; }
        public string Status { get; set; }

        public Order()
        {
            Items = new List<OrderItem>();
        }
    }

    public static class OrderExtensions
    {
        public static Order AdjustItemPrices(this Order order)
        {
            if (order != null)
            {
                foreach (var item in order.Items)
                {
                    item.Price *= 5;
                }
            }
            return order;
        }
    }
}
