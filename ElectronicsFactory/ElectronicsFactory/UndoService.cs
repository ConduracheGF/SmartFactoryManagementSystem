using System;
using System.Collections.Generic;

namespace ElectronicsFactory
{
    internal interface IReversibleAction
    {
        string Description { get; }
        void Undo();
    }

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

    internal class UndoService
    {
        private readonly Stack<IReversibleAction> _actionHistory = new Stack<IReversibleAction>(); 

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

        public string GetLastActionDescription()
        {
            if (_actionHistory.Count == 0) return "None";
            return _actionHistory.Peek().Description;
        }

        public int GetHistoryCount() => _actionHistory.Count;
    }
}