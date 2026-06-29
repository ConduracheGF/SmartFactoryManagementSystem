using System;

namespace ElectronicsFactory
{
    public abstract class Employee
    {
        public string Id { get; set; } 
        public string Name { get; set; }
        public string Department { get; set; }
        public double Salary { get; set; }

        public Employee(string id, string name, string department, double salary)
        {
            Id = id;
            Name = name;
            Department = department;
            Salary = salary;
        }

        public virtual void DisplayInfo()
        {
            Console.WriteLine($"[{Id}] Angajat: {Name} | Departament: {Department} | Salariu: {Salary} RON");
        }
    }

    internal class Director : Employee
    {
      
        public Director(string id, string name, double salary)
             : base(id, name, "Management", salary)
        {
           
        }

        
        public void ReviewProductionStatistics()
        {
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
        
        }
    }

    internal class ProductionManager : Employee
    {
       
      
        public ProductionManager(string id, string name, double salary)
            : base(id, name, "Production", salary) { }

      
        public void CreateProductionOrder(string productType, int quantity)
        {

        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
        }
    }

    internal class Engineer : Employee
    {
        public Engineer(string id, string name, double salary)
            : base(id, name, "Engineering", salary) { }

        
        public bool InspectMachine(string machineName, string condition)
        {

            return false;
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
           
        }
    }

    internal class Technician : Employee
    {
        public Technician(string id, string name, double salary)
            : base(id, name, "Maintenance", salary) { }

        
        public void RepairMachine(string machineName, ref string machineCondition, string machineStatus)
        {

        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
           
        }
    }


    internal class MachineOperator : Employee
    {
        public MachineOperator(string id, string name, double salary)
            : base(id, name, "Production", salary) { }

        
        public void SimulateMachineProduction(string machineName, ref string machineStatus, ref int productStock, int quantityToProduce)
        {
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
        }
    }

    internal class SalesAgent : Employee
    {
        public SalesAgent(string id, string name, double salary)
            : base(id, name, "Sales", salary) { }


        public void SellElectronics(string productType, int quantityRequested, ref int productStock)
        {
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
           
        }
    }

    internal class Accountant : Employee
    {
        public Accountant(string id, string name, double salary)
            : base(id, name, "Finance", salary) { }

        public void CalculateProductionValue(int productStock, double productionCost)
        {

        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
          
        }
    }
}