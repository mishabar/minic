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
    public class OrdersRepository : IOrdersRepository
    {
        private MongoCollection<Order> _collection = null;

        public OrdersRepository(MongoDatabase db)
        {
            _collection = db.GetCollection<Order>("orders");
        }

        public bool AddItem(string sessionId, string userId, OrderItem item)
        {
            try
            {
                Order order = GetCurrentCart(sessionId, userId);
                if (order == null)
                {
                    order = new Order { SessionId = sessionId, UserId = userId, Status = "Open", CreatedOn = DateTime.UtcNow };
                }

                OrderItem exitsingItem = order.Items.FirstOrDefault(i => i.SKU == item.SKU && i.Size == item.Size);
                if (exitsingItem != null)
                {
                    exitsingItem.Quantity++;
                }
                else
                {
                    order.Items.Add(item);
                }
                _collection.Save(order);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Order GetCurrentCart(string sessionId, string userId)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                return _collection.FindOne(Query.And(Query<Order>.EQ(o => o.UserId, userId), Query<Order>.EQ(o => o.Status, "Open")));
            }

            return _collection.FindOne(Query.And(Query<Order>.EQ(o => o.SessionId, sessionId), Query<Order>.EQ(o => o.Status, "Open")));
        }

        public void SetItemQuantity(string sessionId, string userId, string sku, string size, int quantity)
        {
            Order order = GetCurrentCart(sessionId, userId);
            OrderItem item = order.Items.FirstOrDefault(i => i.SKU == sku && i.Size == size);
            if (item != null)
            {
                item.Quantity = quantity;
                _collection.Save(order);
            }
        }

        public void RemoveItem(string sessionId, string userId, string sku, string size)
        {
            try
            {
                Order order = GetCurrentCart(sessionId, userId);
                if (order != null)
                {
                    OrderItem item = order.Items.FirstOrDefault(i => i.SKU == sku && i.Size == size);
                    if (item != null)
                    {
                        order.Items.Remove(item);
                        _collection.Save(order);
                    }
                }
            }
            catch
            {
            }
        }
    }
}
