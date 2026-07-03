using System;

namespace ElectronicsFactory
{
    /// <summary>
    /// Application entry point. Bootstraps the factory with an initial set of employees and machines, then hands control over to the console menu
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Smart Factory Management System";

            // Create the factory with capacities for employees, machines, products, and a starting budget
            Factory electronicsFactory = new Factory(20, 10, 100, 50000f);

            // Seed the factory with an initial staff
            electronicsFactory.EmployeeManager.HiredEmployee(new Director("Ion Creanga", 9000));
            electronicsFactory.EmployeeManager.HiredEmployee(new ProductionManager("Andrei Vasilescu", 6500));
            electronicsFactory.EmployeeManager.HiredEmployee(new MachineOperator("Mihai Ion", 3500));
            electronicsFactory.EmployeeManager.HiredEmployee(new Engineer("Elena Popa", 5500));
            electronicsFactory.EmployeeManager.HiredEmployee(new Technician("Dorel Stefan", 4500));
            electronicsFactory.EmployeeManager.HiredEmployee(new SalesAgent("Andreea Marin", 4000));
            electronicsFactory.EmployeeManager.HiredEmployee(new Accountant("Radu Georgescu", 5000));

            // Seed the factory with an initial set of machines across all machine types
            electronicsFactory.MachineManager.AddMachine(new TestingMachine("X100", 2));
            electronicsFactory.MachineManager.AddMachine(new PackagingMachine("Z90", 3));
            electronicsFactory.MachineManager.AddMachine(new PcbFabricationMachine("UY78", 1));
            electronicsFactory.MachineManager.AddMachine(new AssemblyMachine("ER243", 3));
            electronicsFactory.MachineManager.AddMachine(new PackagingMachine("PCKM21", 6));

            Logger.Info("The fabric was initialised.");
            Console.WriteLine("Press any key to show the menu");
            Console.ReadKey();
  
            MenuManagement menu = new MenuManagement(electronicsFactory);
            menu.DisplayMenu();
        }
    }
}