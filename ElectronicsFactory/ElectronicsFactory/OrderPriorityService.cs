using System;
using System.Collections.Generic;
using System.Linq;

namespace ElectronicsFactory
{
    internal enum PriorityLevel_t
    {
        Low,
        Medium,
        High,
        Critical
    }

    internal class ProductionOrder
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public PriorityLevel_t Priority { get; set; }
        public string Requester { get; set; }
        public DateTime Created { get; private set; }

        public ProductionOrder(Product product, int quantity, PriorityLevel_t priority, string requester)
        {
            Product = product;
            Quantity = quantity;
            Priority = priority;
            Requester = requester;
            Created = DateTime.Now;
        }
    }

    internal class OrderPriorityService
    {
        
        private readonly List<ProductionOrder> _orders = new List<ProductionOrder>();

        public void AddOrder(Product product, int quantity, PriorityLevel_t priority, string requester)
        {
            var newOrder = new ProductionOrder(product, quantity, priority, requester);
            _orders.Add(newOrder);
        }

        
        public IEnumerable<ProductionOrder> GetAllOrders()
        {
           
            return _orders.OrderByDescending(o => o.Priority);
        }

        
        public void ClearAllOrders()
        {
            _orders.Clear();
        }

        public void RemoveOrder(ProductionOrder order)
        {
            _orders.Remove(order);
        }

        public ProductionOrder? GetNextOrder()
        {
            var next = GetAllOrders().FirstOrDefault();
            if (next != null)
            {
                _orders.Remove(next);
            }
            return next;
        }
    }
}