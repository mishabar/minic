﻿using System;
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
        private MongoCollection<Product> _productCollection = null;

        public OrdersRepository(MongoDatabase db)
        {
            _collection = db.GetCollection<Order>("orders");
            _productCollection = db.GetCollection<Product>("products");
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
            return GetCurrentCart(sessionId, userId, false);
        }

        public Order GetCurrentCart(string sessionId, string userId, bool verifyAvailability)
        {
            Order order = null;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                order = _collection.FindOne(Query.And(Query<Order>.EQ(o => o.UserId, userId), Query<Order>.EQ(o => o.Status, "Open")));
                return verifyAvailability ? VerifyAvailability(order) : order;
            }

            order = _collection.FindOne(Query.And(Query<Order>.EQ(o => o.SessionId, sessionId), Query<Order>.EQ(o => o.Status, "Open")));
            return verifyAvailability ? VerifyAvailability(order) : order;
        }

        private Order VerifyAvailability(Order order)
        {
            foreach (var item in order.Items)
            {
                item.IsValid = (_productCollection.FindOneById(item.SKU) != null);
            }
            _collection.Save(order);
            return order;
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

        public void Save(Order order)
        {
            _collection.Save(order);
        }

        public Order GetByMPRefID(string mpRefID)
        {
            return _collection.FindOne(Query<Order>.EQ(o => o.MPRefID, mpRefID));
        }

        public Order GetByMPCollectionID(string id)
        {
            return _collection.FindOne(Query<Order>.EQ(o => o.MPCollectionID, id));
        }
    }
}
