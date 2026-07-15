using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsFactory
{
    internal enum PriorityLevel_t
    {
        Low,
        Medium,
        High,
        Critical
    };

    internal class ProductionOrder
    {
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public PriorityLevel_t Priority { get; set; }
        public string? Requester { get; set; }
        public DateTime? Created { get; private set; }
    }

    internal class OrderPriorityService
    {
        public void AddOrder()
        {

        }

        public IEnumerable<ProductionOrder> GetAllOrders()
        {
            return null;
        }

        public void ClearAllOrders()
        {

        }
    }
}
