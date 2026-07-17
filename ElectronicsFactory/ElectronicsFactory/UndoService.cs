using System;
using System.Collections.Generic;

namespace ElectronicsFactory
{
    // A simple blueprint for any action in the system that can be undone (reversed).
    internal interface IReversibleAction
    {
        string Description { get; }
        void Undo();
    }

    // Handles reversing the process of hiring a new employee by removing them from the system.
    internal class HireEmployeeAction : IReversibleAction
    {
        private readonly EmployeeManagement _employeeManager;
        private readonly Employee _employee;

        public string Description => $"Hired employee {_employee.Name} (ID: {_employee.Id})";

        public HireEmployeeAction(EmployeeManagement employeeManager, Employee employee)
        {
            _employeeManager = employeeManager;
            _employee = employee;
        }

        public void Undo()
        {
            _employeeManager.Employees.Remove(_employee);
            Logger.Info($"[UNDO] Employee {_employee.Name} has been removed from the system.");
        }
    }

    // Handles reversing a newly added production order by removing it from the waiting queue.
    internal class AddOrderAction : IReversibleAction
    {
        private readonly OrderPriorityService _orderManager;
        private readonly ProductionOrder _order;

        public string Description => $"Added production order for {_order.Quantity}x {_order.Product?.Name}";

        public AddOrderAction(OrderPriorityService orderManager, ProductionOrder order)
        {
            _orderManager = orderManager;
            _order = order;
        }

        public void Undo()
        {
            _orderManager.RemoveOrder(_order);
        }
    }

    // Handles reversing the firing of an employee by bringing them back (reinstating them) into the system.
    internal class FireEmployeeAction : IReversibleAction
    {
        private readonly EmployeeManagement _employeeManager;
        private readonly Employee _employee;

        public string Description => $"Fired employee {_employee.Name} (ID: {_employee.Id})";

        public FireEmployeeAction(EmployeeManagement employeeManager, Employee employee)
        {
            _employeeManager = employeeManager;
            _employee = employee;
        }

        public void Undo()
        {
            
            _employeeManager.Add(_employee);
            Logger.Info($"[UNDO] Employee {_employee.Name} has been reinstated.");
        }
    }

    // Handles cancelling a product sale by returning the product to stock and subtracting the money from the budget.
    internal class SellProductAction : IReversibleAction
    {
        private readonly ProductManagement _productManager;
        private readonly Product _product;
        private readonly Factory _factory; 

        public string Description => $"Sold product {_product.Name} (ID: {_product.Id}) for {_product.Price} RON";

        public SellProductAction(ProductManagement productManager, Product product, Factory factory)
        {
            _productManager = productManager;
            _product = product;
            _factory = factory;
        }

        public void Undo()
        {
            
            _productManager.AddProduct(_product);
           
            _factory.Income -= _product.Price;
            Logger.Info($"[UNDO] Sale cancelled. Product {_product.Id} returned to stock. Budget adjusted.");
        }
    }

    // Keeps track of a history of actions so we can reverse (undo) them one by one, starting with the most recent.
    internal class UndoService: IUndoService
    {
        private readonly Stack<IReversibleAction> _actionHistory = new Stack<IReversibleAction>();

        // Saves a completed action to the history list so it can be undone later if needed.
        public void RegisterAction(IReversibleAction action)
        {
            _actionHistory.Push(action);
        }

        public void UndoLastOperation() 
        {
            if (_actionHistory.Count == 0)
            {
                Logger.Warning("No operations to undo!");
                return;
            }

            IReversibleAction lastAction = _actionHistory.Pop(); 
            Logger.Info($"Reversing operation: {lastAction.Description}...");
            lastAction.Undo();
        }

        // Peeks at the history and returns a description of the last action made, without removing it.
        public string GetLastActionDescription()
        {
            if (_actionHistory.Count == 0) return "None";
            return _actionHistory.Peek().Description;
        }

        public int GetHistoryCount() => _actionHistory.Count;
    }
}