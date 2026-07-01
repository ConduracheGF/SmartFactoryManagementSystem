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

            if (index != -1)
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


        public void ReviewProductionStatistics(int totalEmployees, int totalMachines, int totalStock)
        {
            Logger.Info($"Directorul {Name} revizuiește raportul fabricii:");
            Console.WriteLine($" -> Total Angajați activi: {totalEmployees}");
            Console.WriteLine($" -> Total Utilaje înregistrate: {totalMachines}");
            Console.WriteLine($" -> Total Produse în depozit: {totalStock}");
          
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo(); 
     
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
                Logger.Warning($"Managerul {Name} a refuzat comanda: Cantitatea solicitată ({quantity}) trebuie să fie mai mare decât 0!");
                return 0;
            }

            
            Logger.Info($"Managerul de producție {Name} a generat și aprobat comanda pentru {quantity}x unități de tip {productType}.");

            return quantity;
        }
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            
        }
    }

    internal class Engineer : Employee
    {
        public Engineer(string id, string name, double salary)
            : base(id, name, DepartmentStatus_t.Technical, salary) { }


        public bool InspectMachine(Machine machine)
        {
            Logger.Info($"Inginerul {Name} inspectează mașina cu seria {machine.SerialNumber}.");

            bool needsRepair = machine.Inspect();

            if (needsRepair)
            {
                Logger.Warning($"Mașina {machine.SerialNumber} are probleme (Stare: {machine.Condition}) și necesită reparații!");
            }
            else
            {
                Logger.Info($"Mașina {machine.SerialNumber} este în stare bună de funcționare.");
            }

            return needsRepair; 
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            
        }
    }

    internal class Technician : Employee
    {
        public Technician(string id, string name, double salary)
            : base(id, name, DepartmentStatus_t.Technical, salary) { }


        public void RepairMachine(Machine machine)
        {
            Logger.Info($"Tehnicianul {Name} încearcă să repare mașina {machine.SerialNumber}.");

          
            if (machine.Status == MachineStatus_t.Running)
            {
                Logger.Error($"Tehnicianul {Name} NU poate repara o mașină care funcționează!");
                return;
            }

            bool success = machine.Repair();
            if (success)
            {
                Logger.Info($"Tehnicianul {Name} a finalizat cu succes reparația mașinii {machine.SerialNumber}.");
            }
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
           
        }
    }

    internal class SalesAgent : Employee
    {
        public SalesAgent(string id, string name, double salary)
            : base(id, name, DepartmentStatus_t.Sales, salary) { }


       
         public void SellElectronics(Product product, int quantityRequested, ref int productStock)
        {
            Logger.Info($"Agentul de vânzări {Name} încearcă să vândă {quantityRequested} bucăți din produsul de brand {product.Brand}.");

            if (productStock < quantityRequested)
            {
                Logger.Error($"Stoc insuficient! Disponibil: {productStock}, Cerut: {quantityRequested}");
                return;
            }

           
            productStock -= quantityRequested;
            float totalIncome = quantityRequested * product.Currency; 

            Logger.Info($"S-au vândut {quantityRequested} bucăți. Venit generat: {totalIncome} lei. Stoc rămas: {productStock}");
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
            Logger.Info($"Contabilul {Name} calculează valoarea producției actuale...");
       
            double estimatedPricePerUnit = 1500.0;
            double totalStockValue = productStock * estimatedPricePerUnit;
            double financialBalance = totalStockValue - productionCost;

  
            Console.WriteLine($"[RAPORT FINANCIAR] Generat de {Name}:");
            Console.WriteLine($"Unități în stoc: {productStock} buc.");
            Console.WriteLine($"Valoarea totală a stocului: {totalStockValue} lei");
            Console.WriteLine($"Costuri totale de producție: {productionCost} lei");

            if (financialBalance >= 0)
            {
                Logger.Info($" Fabrica înregistrează PROFIT potențial: +{financialBalance} lei");
            }
            else
            {
                Logger.Warning($" Fabrica înregistrează PIERDERE temporară: {financialBalance} lei");
            }
       
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            
        }

    }
    internal class MachineOperator : Employee
    {
        public MachineOperator(string id, string name, double salary)
            : base(id, name, DepartmentStatus_t.Production, salary) { }

       
        public bool StartMachine(Machine machine)
        {
            Logger.Info($"Operatorul {Name} încearcă să pornească mașina {machine.SerialNumber}.");
            return machine.Start(); 
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            
        }
    }
}