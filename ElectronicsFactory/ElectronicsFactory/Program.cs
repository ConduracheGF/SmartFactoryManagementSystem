using System;

namespace ElectronicsFactory
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Smart Factory Management System";

            
            Factory electronicsFactory = new Factory(20, 10, 100, 50000f);

       
            electronicsFactory.EmployeeManager.HiredEmployee(new ProductionManager("1", "Andrei Vasilescu", 6500));
            electronicsFactory.EmployeeManager.HiredEmployee(new MachineOperator("2", "Mihai Ion", 3500));
            electronicsFactory.EmployeeManager.HiredEmployee(new Engineer("3", "Elena Popa", 5500));
            electronicsFactory.EmployeeManager.HiredEmployee(new Technician("4", "Dorel Stefan", 4500));
            electronicsFactory.EmployeeManager.HiredEmployee(new SalesAgent("5", "Andreea Marin", 4000));
            electronicsFactory.EmployeeManager.HiredEmployee(new Accountant("6", "Radu Georgescu", 5000));

            
            electronicsFactory.MachineManager.AddMachine(new TestingMachine("X100", 2));

            Logger.Info("The fabric was initialised.");
            Console.WriteLine("Press any key to show the menu");
            Console.ReadKey();

  
            MenuManagement menu = new MenuManagement(electronicsFactory);
            menu.DisplayMenu();
        }
    }
}