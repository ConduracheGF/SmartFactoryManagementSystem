using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ElectronicsFactory
{
    // Defines how urgent a production order is (from Low to Critical)
    internal enum PriorityLevel_t
    {
        Low,
        Medium,
        High,
        Critical
    }

    internal class ProductionOrder
    {
        private static int _id = 1;

        public int Id { get; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public PriorityLevel_t Priority { get; set; }
        public string Requester { get; set; }
        public DateTime Created { get; private set; }

        public ProductionOrder(Product product, int quantity, PriorityLevel_t priority, string requester)
        {
            Id = _id++;
            Product = product;
            Quantity = quantity;
            Priority = priority;
            Requester = requester;
            Created = DateTime.Now;
        }
    }

    // Manages the queue of active orders, making sure higher priority and older orders are handled first.
    internal class OrderPriorityService
    {
        private readonly Queue<ProductionOrder> _orderQueue = new Queue<ProductionOrder>();
        private readonly HashSet<int> _processedOrdersIds = new HashSet<int>();
        private readonly List<ProductionOrder> _orders = new List<ProductionOrder>();

        public ProductionOrder AddOrder(Product product, int quantity, PriorityLevel_t priority, string requester)
        {
            var newOrder = new ProductionOrder(product, quantity, priority, requester);
            _orders.Add(newOrder);
            RebuildQueue();
            return newOrder;
        }

        // Returns all orders sorted from the most urgent to the least urgent.
        public IEnumerable<ProductionOrder> GetAllOrders()
        {
            return _orders.OrderByDescending(o => o.Priority).ThenBy(o => o.Created);
        }

        
        public void ClearAllOrders()
        {
            _orders.Clear();
            _orderQueue.Clear();
            _processedOrdersIds.Clear();
        }

        // Deletes a specific order from the list and updates the waiting line.
        public void RemoveOrder(ProductionOrder order)
        {
            _orders.RemoveAll(o => o.Id == order.Id);
            RebuildQueue();
        }

        // Retrieves and removes the next most urgent order from the queue to process it.
        public ProductionOrder? GetNextOrder()
        {
            if (_orderQueue.Count == 0)
            {
                return null;
            }

            var next = _orderQueue.Dequeue();
            _orders.Remove(next);
            _processedOrdersIds.Add(next.Id);

            return next;
        }

        // Re-sorts all remaining orders so the queue always processes the highest priority items first.
        public void RebuildQueue()
        {
            _orderQueue.Clear();

            var sortedList = _orders.OrderByDescending(o => o.Priority).ThenBy(o => o.Created);

            foreach (var order in sortedList)
            {
                if (!_processedOrdersIds.Contains(order.Id))
                {
                    _orderQueue.Enqueue(order);
                }
            }
        }
    }
}