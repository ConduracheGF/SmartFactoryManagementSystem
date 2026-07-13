using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsFactory
{
    // Manages the factory's employee roster: hiring, firing, and lookup, enforcing the unique-ID business rule
    internal class EmployeeManagement: GenericRepository<Employee>
    {
        // Underlying fixed-size storage array for employees (may contain unused trailing slots)
        public int EmployeeCount => _storage.Count;
        public List<Employee> Employees => _storage;

        // Hires a new employee, rejecting it if capacity is full or if an employee with the same ID already exists
        public bool HiredEmployee(Employee employee)
        {
            if (employee == null) return false;

            if (_storage.Any(e => e.Id == employee.Id))
            {
                Logger.Warning($"The employee with the ID {employee.Id} already exists in the company!");
                return false;
            }

            Add(employee);
            Logger.Info($"The employee with ID {employee.Id} ({employee.Name}) has been added.");
            return true;
        }

        // Fires the employee with the given ID, shifting remaining elements left to keep storage compact
        public void FiredEmployee(string id)
        {
            if (_storage.Count == 0)
            {
                Logger.Error("The factory was left without employees!");
                return;
            }

            var employee = _storage.Find(e => e.Id == id);

            if (employee != null)
            {
                Remove(employee);
                Logger.Info($"Employee with ID {id} has been fired.");
            }
            else
            {
                Logger.Info($"The employee with the ID {id} does not exist in the company!");
            }
        }

        // Searches the roster for an employee by ID or by name. As a side effect, displays the employee's info when found
        public int SearchEmployee(string Id_or_Name)
        {
            if (_storage.Count == 0)
            {
                Logger.Info("There are no employees in the factory.");
                return -1;
            }

            var employee = _storage.Find(e => e.Id == Id_or_Name || e.Name.Equals(Id_or_Name));

            if (employee != null)
            {
                employee.DisplayInfo();
                return _storage.IndexOf(employee);
            }

            Logger.Warning($"Employee {Id_or_Name} was not found.");
            return -1;
        }
    }
}
