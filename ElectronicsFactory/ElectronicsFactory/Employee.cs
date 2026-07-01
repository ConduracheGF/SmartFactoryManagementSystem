using System;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace ElectronicsFactory
{
    public enum DepartmentStatus_t
    {
        Management,
        Production,
        Technical,
        Sales,
        Finance
    }
    internal class EmployeeManagement
    {
        private Employee[] employees;
        private int employeesCount = 0;

        public EmployeeManagement(int maxCapacity)
        {
            employees = new Employee[maxCapacity];
            employeesCount = 0;
        }
        public bool AddEmployee(Employee employee)
        {
            if (employeesCount >= employees.Length)
            {
                Logger.Error("The factory has reached its maximum employee limit!");
                return false;
            }

            int index = Search(employee.Id);

            if (index == -1)
            {
                employees[employeesCount] = employee;
                employeesCount++;

                Logger.Info($"The employee with ID {employee.Id} has been added.");
                return true;
            } else
            {
                Logger.Warning($"The employee with the ID {employee.Id} already exists in the company!");
                return false;
            }
        }
        public void RemoveEmployee(Employee employee)
        {
            if (employeesCount == 0)
            {
                Logger.Error("The factory was left without employees!");
                return;
            }

            int index = Search(employee.Id);

            if (index != -1)
            {
                for(int i = index; i < employees.Length - 1; i++)
                {
                    employees[i] = employees[i + 1];
                }

                Logger.Info($"Employee with ID {employee.Id} has been fired.");
            }
            else
            {
                Logger.Info($"The employee with the ID {employee.Id} does not exist in the company!");
            }
        }

        public int Search(string Id_or_Name)
        {
            if (employeesCount == 0)
            {
                Logger.Info("There are no employees in the factory.");
                return -1;
            }

            for (int i = 0; i < employeesCount; i++)
            {
                if (employees[i].Id == Id_or_Name || employees[i].Name == Id_or_Name)
                {
                    employees[i].DisplayInfo();
                    return i;
                }
            }
            Logger.Warning($"Employee {Id_or_Name} was not found.");
            return -1;
        }
    }
    public abstract class Employee
    {
        public string Id { get; set; } 
        public string Name { get; set; }
        public DepartmentStatus_t Department { get; set; }
        public double Salary { get; set; }

        public Employee(string id, string name, DepartmentStatus_t department, double salary)
        {
            Id = id;
            Name = name;
            Department = department;
            Salary = salary;
        }

        public virtual void DisplayInfo()
        {
            Logger.Info($"[{Id}] Employee: {Name}, Department: {Department}, Salary: {Salary} RON");
        }
    }

    internal class Director : Employee
    {
      
        public Director(string id, string name, double salary): base(id, name, DepartmentStatus_t.Management, salary){}


        public void ReviewProductionStatistics(int totalEmployees, int totalMachines, int totalStock)
        {
            Logger.Info($"Director {Name} reviews the factory report:");
            Console.WriteLine($" -> Total Active Employees: {totalEmployees}");
            Console.WriteLine($" -> Total Machines registered: {totalMachines}");
            Console.WriteLine($" -> Total Products in stock: {totalStock}");
          
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Logger.Info($"Director reviews the factory report.");
        }
    }

    internal class ProductionManager : Employee
    {
        public ProductionManager(string id, string name, double salary)
            : base(id, name, DepartmentStatus_t.Production, salary) { }

        public int CreateProductionOrder(string productType, int quantity)
        {
            if (quantity <= 0)
            {
                Logger.Warning($"Manager {Name} refused the order: The requested quantity ({quantity}) must be greater than 0!");
                return 0;
            }
            
            Logger.Info($"Production Manager {Name} generated and approved the order for {quantity}x units of type {productType}.");
            return quantity;
        }
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Logger.Info($"Production Manager {Name} generated and approved the orders.");
        }
    }

    internal class Engineer : Employee
    {
        public Engineer(string id, string name, double salary)
            : base(id, name, DepartmentStatus_t.Technical, salary) { }

        public bool InspectMachine(Machine machine)
        {
            Logger.Info($"Engineer {Name} is inspecting the machine with serial number {machine.SerialNumber}.");
            bool needsRepair = machine.Inspect();

            if (needsRepair)
            {
                Logger.Warning($"Machine {machine.SerialNumber} has problems [Status: {machine.Condition}] and requires repair!");
            }
            else
            {
                Logger.Info($"Machine {machine.SerialNumber} is in good working order.");
            }

            return needsRepair; 
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Logger.Info($"Engineer {Name} inspects the machines.");
        }
    }

    internal class Technician : Employee
    {
        public Technician(string id, string name, double salary)
            : base(id, name, DepartmentStatus_t.Technical, salary) { }


        public void RepairMachine(Machine machine)
        {
            Logger.Info($"Technician {Name} is attempting to repair machine {machine.SerialNumber}.");

            if (machine.Status == MachineStatus_t.Running)
            {
                Logger.Error($"Technician {Name} CANNOT repair a working machine!"); return;
            }

            bool success = machine.Repair();
            if (success)
            {
                Logger.Info($"Technician {Name} successfully completed the repair of machine {machine.SerialNumber}.");
            }
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Logger.Info($"Technician {Name} is attempting to repair machines.");
        }
    }

    internal class SalesAgent : Employee
    {
        public SalesAgent(string id, string name, double salary)
            : base(id, name, DepartmentStatus_t.Sales, salary) { }


       
         public void SellElectronics(Product product, int quantityRequested, ref int productStock)
        {
            Logger.Info($"Salesperson {Name} is attempting to sell {quantityRequested} units.");
            if (productStock < quantityRequested)
            {
                Logger.Error($"Insufficient stock! Available: {productStock}, Requested: {quantityRequested}"); return;
            }

           
            productStock -= quantityRequested;
            float totalIncome = quantityRequested * product.Currency;

            Logger.Info($"{quantityRequested} pieces sold. Revenue generated: {totalIncome} lei. Remaining stock: {productStock}");
        }
        

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            
        }
    }

    internal class Accountant : Employee
    {
        public Accountant(string id, string name, double salary)
            : base(id, name, DepartmentStatus_t.Finance, salary) { }

        public void CalculateProductionValue(int productStock, double productionCost)
        {
            Logger.Info($"Accountant {Name} is calculating the value of current production...");

            double estimatedPricePerUnit = 1500.0;
            double totalStockValue = productStock * estimatedPricePerUnit;
            double financialBalance = totalStockValue - productionCost;


            Logger.Info($"[FINANCIAL REPORT] Generated by {Name}:");
            Logger.Info($"Units in stock: {productStock} pcs.");
            Logger.Info($"Total stock value: {totalStockValue} RON");
            Logger.Info($"Total production costs: {productionCost} RON");
            if (financialBalance >= 0)
            {
                Logger.Info($" The factory is recording a potential PROFIT: +{financialBalance} lei");
            }
            else
            {
                Logger.Warning($" Factory records temporary LOSS: {financialBalance} lei");
            }
       
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Logger.Info($"Accountant {Name} is calculating the value of current production.");
        }

    }
    internal class MachineOperator : Employee
    {
        public MachineOperator(string id, string name, double salary)
            : base(id, name, DepartmentStatus_t.Production, salary) { }

       
        public bool StartMachine(Machine machine)
        {
            Logger.Info($"Operator {Name} is attempting to start machine {machine.SerialNumber}.");
            return machine.Start(); 
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Logger.Info($"Operator {Name} is attempting to start machines.");
        }
    }
}