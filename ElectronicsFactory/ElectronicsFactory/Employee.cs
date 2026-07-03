using System;
using System.Diagnostics;
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

    public enum JobStatus_t
    {
        Director,
        ProductionManager,
        Engineer,
        Technician,
        MachineOperator,
        SalesAgent,
        Acountant
    }

    internal class EmployeeManagement
    {
        private Employee?[] employees;
        private int employeesCount = 0;

        public Employee?[] Employees { get { return employees; } set { employees = value; } }
        public int EmployeesCount { get { return employeesCount; } set { employeesCount = value; } }

        public EmployeeManagement(int maxCapacity)
        {
            employees = new Employee?[maxCapacity];
            employeesCount = 0;
        }

        public bool HiredEmployee(Employee employee)
        {
            if (employeesCount >= employees.Length)
            {
                Logger.Error("The factory has reached its maximum employee limit!");
                return false;
            }

            int index = SearchEmployee(employee.Id);

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

        public void FiredEmployee(string id)
        {
            if (employeesCount == 0)
            {
                Logger.Error("The factory was left without employees!");
                return;
            }

            int index = SearchEmployee(id);

            if (index != -1)
            {
                for(int i = index; i < employeesCount - 1; i++)
                {
                    employees[i] = employees[i + 1];
                }
                employees[employeesCount - 1] = null;
                employeesCount--;

                Logger.Info($"Employee with ID {id} has been fired.");
            }
            else
            {
                Logger.Info($"The employee with the ID {id} does not exist in the company!");
            }
        }

        public int SearchEmployee(string Id_or_Name)
        {
            if (employeesCount == 0)
            {
                Logger.Info("There are no employees in the factory.");
                return -1;
            }

            for (int i = 0; i < employeesCount; i++)
            {
                if (employees[i]!.Id == Id_or_Name || employees[i]!.Name == Id_or_Name)
                {
                    employees[i]!.DisplayInfo();
                    return i;
                }
            }
            Logger.Warning($"Employee {Id_or_Name} was not found.");
            return -1;
        }
    }
    public abstract class Employee
    {
        private static int nextId = 1;

        public string Id { get; private set; } 
        public string Name { get; set; }
        public DepartmentStatus_t Department { get; set; }
        public JobStatus_t JobStatus { get; set; }
        public double Salary { get; set; }

        public Employee(string name, DepartmentStatus_t department, JobStatus_t job, double salary)
        {
            Id = (nextId++).ToString();
            Name = name;
            Department = department;
            JobStatus = job;
            Salary = salary;
        }

        public virtual void DisplayInfo()
        {
            Logger.Info($"[{Id}] Employee: {Name}, Department: {Department}, Job: {JobStatus}, Salary: {Salary} RON");
        }
    }

    internal class Director : Employee
    {
        public Director(string name, double salary): base(name, DepartmentStatus_t.Management, JobStatus_t.Director, salary){}

        public void ReviewProductionStatistics(int totalEmployees, int totalMachines, int totalStock, float income)
        {
            Logger.Info($"Director {Name} reviews the factory report:");
            Logger.Info($" -> Total Active Employees: {totalEmployees}");
            Logger.Info($" -> Total Machines registered: {totalMachines}");
            Logger.Info($" -> Total Products in stock: {totalStock}");
            Logger.Info($" -> Total Income in finance: {income}");
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Logger.Info($"Director reviews the factory report.");
        }
    }

    internal class ProductionManager : Employee
    {
        public ProductionManager(string name, double salary)
            : base(name, DepartmentStatus_t.Production, JobStatus_t.ProductionManager, salary) { }

        public bool CreateProductionOrder(ref ProductManagement productManagement, Product product, int quantity)
        {
            int count = 0;
            if (quantity <= 0)
            {
                Logger.Warning($"Manager {Name} refused the order: The requested quantity ({quantity}) must be greater than 0!");
                return false;
            }

            for (int i = 0; i < quantity; i++)
            {
                if (productManagement.AddProduct(product))
                {
                    count++;
                }
            }

            if (count == quantity)
            {
                Logger.Info($"Production Manager {Name} generated and approved the order for {quantity} x units of type {product.ProductType}.");
                return true;
            }

            return false;
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Logger.Info($"Production Manager {Name} generated and approved the orders.");
        }
    }

    internal class Engineer : Employee
    {
        public Engineer(string name, double salary)
            : base(name, DepartmentStatus_t.Technical, JobStatus_t.Engineer, salary) { }

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
        public Technician(string name, double salary)
            : base(name, DepartmentStatus_t.Technical, JobStatus_t.Technician, salary) { }


        public void RepairMachine(Machine machine, ref float income)
        {
            Logger.Info($"Technician {Name} is attempting to repair machine {machine.SerialNumber}.");

            if (machine.Status == MachineStatus_t.Running)
            {
                Logger.Error($"Technician {Name} CANNOT repair a working machine!");
                return;
            }
            machine.Status = MachineStatus_t.Maintenance;
            bool success = machine.Repair(ref income);
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
        public SalesAgent(string name, double salary)
            : base(name, DepartmentStatus_t.Sales, JobStatus_t.SalesAgent, salary) { }
       
        public float SellElectronics(ref ProductManagement productManagement, Product product, float income)
        {
            Logger.Info($"The product has sold! The income is increasing!");
            income = productManagement.SoldProduct(product, income);
            return income;
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
        }
    }

    internal class Accountant : Employee
    {
        public Accountant(string name, double salary)
            : base(name, DepartmentStatus_t.Finance, JobStatus_t.Acountant, salary) { }

        public void CalculateProductionValue(ProductManagement productManagement, MachineManagement machineManagement, float income)
        {
            float costs = 0;
            float profit = 0;

            Logger.Info($"Accountant {Name} is calculating the value of current production...");

            for (int i = 0; i < productManagement.ProductsCount; i++)
            {
                Product product = productManagement.Storage[i];
                income = product.SellProduct(income);
                profit += product.Currency;
            }

            for (int i = 0; i < machineManagement.MachineCount; i++)
            {
                Machine machine = machineManagement.Machines[i];
                
                foreach (MachineParts part in machine.Components)
                {
                    if (part != null)
                    {
                        costs += part.Currency;
                    }
                } 
            }

            float financialBalance = income - costs;
            Logger.Info($"[FINANCIAL REPORT] Generated by {Name}:");
            Logger.Info($"Total income: {income} pcs.");
            Logger.Info($"Total production profit: {profit} RON");
            Logger.Info($"Total production costs: {costs} RON");
            if (financialBalance >= 0)
            {
                Logger.Info($"The factory is recording a potential PROFIT: +{financialBalance} RON");
            }
            else
            {
                Logger.Warning($"Factory records temporary LOSS: {financialBalance} RON");
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
        public MachineOperator(string name, double salary)
            : base(name, DepartmentStatus_t.Production, JobStatus_t.MachineOperator, salary) { }

       
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