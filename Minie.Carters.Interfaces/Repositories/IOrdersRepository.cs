using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minie.Carters.Data;

namespace Minie.Carters.Interfaces.Repositories
{
    public interface IOrdersRepository
    {
        bool AddItem(string sessionId, string userId, OrderItem item);
        Order GetCurrentCart(string sessionId, string userId);
        void SetItemQuantity(string sessionId, string userId, string sku, string size, int quantity);
        void RemoveItem(string sessionId, string userId, string sku, string size);
        void Save(Order order);
        Order GetByMPRefID(string mpRefID);
    }
}
