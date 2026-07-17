using System;
using System.Diagnostics;
using System.Globalization;
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

   
    // Job role/title of an employee
    public enum JobStatus_t
    {
        Director,
        ProductionManager,
        Engineer,
        Technician,
        MachineOperator,
        SalesAgent,
        Accountant
    }

    
    // Abstract base class representing any employee working in the factory.
    // Provides shared identity (auto-generated ID) and common employment attributes.
    public abstract class Employee
    {
        // Static counter shared across all Employee instances, used to auto-generate unique IDs
        private static int nextId = 1;

        public static void SetNextId(int id)
        {
            nextId = id;
        }
        
        public string Id { get; private set; }

        
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public DepartmentStatus_t Department { get; set; }

        public JobStatus_t JobStatus { get; set; }

        public double Salary { get; set; }

        // Initializes a new employee with an auto-generated unique ID
        public Employee(string name, DepartmentStatus_t department, JobStatus_t job, double salary, string username, string password)
        {
            Id = (nextId++).ToString();
            Name = name;
            Department = department;
            JobStatus = job;
            Salary = salary;

            Username = username;
            Password = password;
        }

        
        public virtual void DisplayInfo()
        {
            Logger.Info($"[{Id}] Employee: {Name}, Department: {Department}, Job: {JobStatus}, Salary: {Salary} RON");
        }

        // Converts the employee's data into a standardized CSV string for file persistence.
        public virtual string ToFileRow()
        {
            return $"{GetType().Name};{Id};{Name};{Username};{Password};{Salary};{Department};{JobStatus}";
        }
    }

    
    // Represents the factory Director, responsible for reviewing factory-wide statistics but not for operating machines directly.
    internal class Director : Employee
    {
        
        public Director(string name, double salary, string username, string password): base(name, DepartmentStatus_t.Management, JobStatus_t.Director, salary, username, password) { }

        public Director(string name, double salary)
    : base(name, DepartmentStatus_t.Management, JobStatus_t.Director, salary,
           name.ToLower().Replace(" ", "") + ".director", "parola123")
        { }

        
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

    
    // Represents a Production Manager, the only role authorized to create production orders
    internal class ProductionManager : Employee
    {
       
        public ProductionManager(string name, double salary, string username, string password)
            : base(name, DepartmentStatus_t.Production, JobStatus_t.ProductionManager, salary, username, password) { }

        public ProductionManager(string name, double salary)
    : base(name, DepartmentStatus_t.Production, JobStatus_t.ProductionManager, salary,
           name.ToLower().Replace(" ", "") + ".manager", "parola123")
        { }

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

    
    // Represents an Engineer, responsible for diagnosing machine condition before maintenance can proceed
    internal class Engineer : Employee
    {
        
        public Engineer(string name, double salary, string username, string password)
            : base(name, DepartmentStatus_t.Technical, JobStatus_t.Engineer, salary, username, password) { }

        public Engineer(string name, double salary)
        : base(name, DepartmentStatus_t.Technical, JobStatus_t.Engineer, salary,
               name.ToLower().Replace(" ", "") + ".inginer", "parola123")
        { }

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

    
    // Represents a Technician, the only role authorized to repair machines and only when they are not currently running
    internal class Technician : Employee
    {
       
        public Technician(string name, double salary, string username, string password)
            : base(name, DepartmentStatus_t.Technical, JobStatus_t.Technician, salary, username, password) { }

        public Technician(string name, double salary)
    : base(name, DepartmentStatus_t.Technical, JobStatus_t.Technician, salary,
           name.ToLower().Replace(" ", "") + ".tehnic", "parola123")
        { }

        // Attempts to repair the given machine. Refuses if the machine is currently running; otherwise puts it into Maintenance and repairs it
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
        
        public SalesAgent(string name, double salary, string username, string password)
            : base(name, DepartmentStatus_t.Sales, JobStatus_t.SalesAgent, salary, username, password) { }

        public SalesAgent(string name, double salary)
    : base(name, DepartmentStatus_t.Sales, JobStatus_t.SalesAgent, salary,
           name.ToLower().Replace(" ", "") + ".vanzari", "parola123")
        { }

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

 
    // Represents an Accountant, responsible for generating the factory's financial report (income, profit, costs, and overall balance)
    internal class Accountant : Employee
    {
        
        public Accountant(string name, double salary, string username, string password)
            : base(name, DepartmentStatus_t.Finance, JobStatus_t.Accountant, salary, username, password) { }

        public Accountant(string name, double salary)
    : base(name, DepartmentStatus_t.Finance, JobStatus_t.Accountant, salary,
           name.ToLower().Replace(" ", "") + ".contabil", "parola123")
        { }


        // Calculates and prints a full financial report: total income, profit from stocked products, and costs of installed machine components
        public void CalculateProductionValue(ProductManagement productManagement, MachineManagement machineManagement, float income)
        {
            float costs = 0;
            float profit = 0;

            Logger.Info($"Accountant {Name} is calculating the value of current production...");

           
            for (int i = 0; i < productManagement.ProductCount; i++)
            {
                Product product = productManagement.Storage[i];
                income = product.SellProduct(income);
                profit += product.Price;
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

    
    // Represents a Machine Operator, the only role authorized to start machines for production
    internal class MachineOperator : Employee
    {
       
        public MachineOperator(string name, double salary, string username, string password)
            : base(name, DepartmentStatus_t.Production, JobStatus_t.MachineOperator, salary, username, password) { }

        public MachineOperator(string name, double salary)
    : base(name, DepartmentStatus_t.Production, JobStatus_t.MachineOperator, salary,
           name.ToLower().Replace(" ", "") + ".operator", "parola123")
        { }

        
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