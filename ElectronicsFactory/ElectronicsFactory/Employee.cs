using System;
using System.Reflection.Metadata.Ecma335;

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
                Logger.Error("Fabrica a atins limita maxim de angajati!");
                return false;
            }

            int index = Search(employee.Id);

            if (index == -1)
            {
                employees[employeesCount] = employee;
                employeesCount++;

                Logger.Info($"Angajatul cu ID-ul {employee.Id} a fost adăugat.");
                return true;
            } else
            {
                Logger.Warning($"Angajatul cu ID-ul {employee.Id} exista deja in firma!");
                return false;
            }
        }
        public void RemoveEmployee(Employee employee)
        {
            if (employeesCount == 0)
            {
                Console.WriteLine("Fabrica a ramas fara angajati!");
                return;
            }

            int index = Search(employee.Id);

            if (index == -1)
            {
                employeesCount--;

                Console.WriteLine($"Angajat cu ID-ul {employee.Id} a fost concediat.");
            }
            else
            {
                Console.WriteLine($"Angajatul cu ID-ul {employee.Id} nu exista in firma!");
            }
        }

        public int Search(string Id_or_Name)
        {
            if (employeesCount == 0)
            {
                Logger.Info("Nu exista angajati in fabrica.");
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
            Logger.Warning($"Angajatul {Id_or_Name} nu a fost găsit.");
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
            Logger.Info($"[{Id}] Angajat: {Name}, Departament: {Department}, Salariu: {Salary} lei");
        }
    }

    internal class Director : Employee
    {
      
        public Director(string id, string name, double salary)
             : base(id, name, DepartmentStatus_t.Management, salary)
        {
           
        }

        
        public void ReviewProductionStatistics()
        {
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo(); 
            Logger.Info("Directorul verifica statisticile productiei.");
        
        }
    }

    internal class ProductionManager : Employee
    {
        public ProductionManager(string id, string name, double salary)
            : base(id, name, DepartmentStatus_t.Production, salary) { }

      
        public void CreateProductionOrder(string productType, int quantity)
        {


        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Logger.Info("Managerul de productie creeaza o comanda de productie.");
        }
    }

    internal class Engineer : Employee
    {
        public Engineer(string id, string name, double salary)
            : base(id, name, DepartmentStatus_t.Technical, salary) { }

        
        public bool InspectMachine(string machineName, string condition)
        {
            return false;
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Logger.Info("Inginerul inspecteaza masina selectata.");
        }
    }

    internal class Technician : Employee
    {
        public Technician(string id, string name, double salary)
            : base(id, name, DepartmentStatus_t.Technical, salary) { }

        
        public void RepairMachine(string machineName, ref string machineCondition, string machineStatus)
        {

        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Logger.Info("Tehnicianul repara masina daca este necesar");
        }
    }

    internal class SalesAgent : Employee
    {
        public SalesAgent(string id, string name, double salary)
            : base(id, name, DepartmentStatus_t.Sales, salary) { }


        public void SellElectronics(string productType, int quantityRequested, ref int productStock)
        {
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Logger.Info("Agentul de vanzari vinde produse");
        }
    }

    internal class Accountant : Employee
    {
        public Accountant(string id, string name, double salary)
            : base(id, name, DepartmentStatus_t.Finance, salary) { }

        public void CalculateProductionValue(int productStock, double productionCost)
        {

        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Logger.Info("Contabilul calculeaza productia");
        }
    }
}