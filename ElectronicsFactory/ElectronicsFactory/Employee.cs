using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace ElectronicsFactory
{
    /// <summary>
    /// Organizational department an employee belongs to
    /// </summary>
    public enum DepartmentStatus_t
    {
        Management,
        Production,
        Technical,
        Sales,
        Finance
    }

    /// <summary>
    /// Job role/title of an employee
    /// </summary>
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

    // Manages the factory's employee roster: hiring, firing, and lookup, enforcing the unique-ID business rule
    internal class EmployeeManagement
    {
        private Employee?[] employees;
        private int employeesCount = 0;

        // Underlying fixed-size storage array for employees (may contain unused trailing slots)
        public Employee?[] Employees { get { return employees; } set { employees = value; } }

        // Number of employees currently on the roster
        public int EmployeesCount { get { return employeesCount; } }

        // Initializes employee storage with a fixed maximum capacity
        public EmployeeManagement(int maxCapacity)
        {
            employees = new Employee?[maxCapacity];
            employeesCount = 0;
        }

        // Hires a new employee, rejecting it if capacity is full or if an employee with the same ID already exists
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

        // Fires the employee with the given ID, shifting remaining elements left to keep storage compact
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
                // Shift all subsequent elements one position left, then shrink the logical count
                for (int i = index; i < employeesCount - 1; i++)
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

        // Searches the roster for an employee by ID or by name. As a side effect, displays the employee's info when found
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

    /// <summary>
    /// Abstract base class representing any employee working in the factory.
    /// Provides shared identity (auto-generated ID) and common employment attributes.
    /// </summary>
    public abstract class Employee
    {
        // Static counter shared across all Employee instances, used to auto-generate unique IDs
        private static int nextId = 1;

        // Unique, auto-generated identifier assigned at construction time
        public string Id { get; private set; }

        // Full name of the employee
        public string Name { get; set; }

        // Organizational department this employee belongs to
        public DepartmentStatus_t Department { get; set; }

        // Job role of this employee
        public JobStatus_t JobStatus { get; set; }

        // Monthly salary, in RON
        public double Salary { get; set; }

        // Initializes a new employee with an auto-generated unique ID
        public Employee(string name, DepartmentStatus_t department, JobStatus_t job, double salary)
        {
            Id = (nextId++).ToString();
            Name = name;
            Department = department;
            JobStatus = job;
            Salary = salary;
        }

        // Displays this employee's basic information 
        // Overridden by each derived class to add role-specific details
        public virtual void DisplayInfo()
        {
            Logger.Info($"[{Id}] Employee: {Name}, Department: {Department}, Job: {JobStatus}, Salary: {Salary} RON");
        }
    }

    /// <summary>
    /// Represents the factory Director, responsible for reviewing factory-wide statistics but not for operating machines directly.
    /// </summary>
    internal class Director : Employee
    {
        // Initializes a new Director
        public Director(string name, double salary): base(name, DepartmentStatus_t.Management, JobStatus_t.Director, salary){}

        // Prints a factory-wide overview report: employee count, machine count, stock levels, and current income
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

    /// <summary>
    /// Represents a Production Manager, the only role authorized to create production orders
    /// </summary>
    internal class ProductionManager : Employee
    {
        // Initializes a new ProductionManager
        public ProductionManager(string name, double salary)
            : base(name, DepartmentStatus_t.Production, JobStatus_t.ProductionManager, salary) { }

        // Creates and approves a production order for a given quantity of a product
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

    /// <summary>
    /// Represents an Engineer, responsible for diagnosing machine condition before maintenance can proceed
    /// </summary>
    internal class Engineer : Employee
    {
        // Initializes a new Engineer
        public Engineer(string name, double salary)
            : base(name, DepartmentStatus_t.Technical, JobStatus_t.Engineer, salary) { }

        // Inspects a machine and reports whether it requires repair
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

    /// <summary>
    /// Represents a Technician, the only role authorized to repair machines and only when they are not currently running
    /// </summary>
    internal class Technician : Employee
    {
        // Initializes a new Technician
        public Technician(string name, double salary)
            : base(name, DepartmentStatus_t.Technical, JobStatus_t.Technician, salary) { }

        // Attempts to repair the given machine. Refuses if the machine is currently running; otherwise puts it into Maintenance and repairs it
        public void RepairMachine(Machine machine, ref float income)
        {
            Logger.Info($"Technician {Name} is attempting to repair machine {machine.SerialNumber}.");

            // A technician cannot repair a running machine.
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

    /// <summary>
    /// Represents a Sales Agent, responsible for selling products from inventory
    /// </summary>
    internal class SalesAgent : Employee
    {
        // Initializes a new SalesAgent
        public SalesAgent(string name, double salary)
            : base(name, DepartmentStatus_t.Sales, JobStatus_t.SalesAgent, salary) { }

        // Sells a product from inventory and updates factory income
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

    /// <summary>
    /// Represents an Accountant, responsible for generating the factory's financial report (income, profit, costs, and overall balance)
    /// </summary>
    internal class Accountant : Employee
    {
        // Initializes a new Accountant
        public Accountant(string name, double salary)
            : base(name, DepartmentStatus_t.Finance, JobStatus_t.Acountant, salary) { }


        // Calculates and prints a full financial report: total income, profit from stocked products, and costs of installed machine components
        public void CalculateProductionValue(ProductManagement productManagement, MachineManagement machineManagement, float income)
        {
            float costs = 0;
            float profit = 0;

            Logger.Info($"Accountant {Name} is calculating the value of current production...");

            // Only iterate the populated portion of storage (avoids null-reference on unused slots)
            for (int i = 0; i < productManagement.ProductsCount; i++)
            {
                Product product = productManagement.Storage[i];
                income = product.SellProduct(income);
                profit += product.Currency;
            }

            // Only iterate the populated portion of storage (avoids null-reference on unused slots)
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

    /// <summary>
    /// Represents a Machine Operator, the only role authorized to start machines for production
    /// </summary>
    internal class MachineOperator : Employee
    {
        // Initializes a new MachineOperator
        public MachineOperator(string name, double salary)
            : base(name, DepartmentStatus_t.Production, JobStatus_t.MachineOperator, salary) { }

        // Attempts to start the given machine
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