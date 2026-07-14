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

            const string OperationsFile = "operations.txt";


        FileStorageService storageService = new FileStorageService();
            DataPersistenceService persistenceService = new DataPersistenceService(storageService);

            // Create the factory with capacities for employees, machines, products, and a starting budget
            Factory electronicsFactory = new Factory(20, 10, 100, 50000f);

            persistenceService.LoadEmployees("employees.txt", electronicsFactory.EmployeeManager);
            persistenceService.LoadMachines("machines.txt", electronicsFactory.MachineManager);
            persistenceService.LoadProducts("products.txt", electronicsFactory.ProductManager);

            AuthService authService = new AuthService(electronicsFactory.EmployeeManager, storageService);

            Logger.Info("The factory was initialised and data has been loaded from files.");
            Console.WriteLine("Press any key to proceed to the login screen...");
            Console.ReadKey();

    
            bool isAuthenticated = false;
            while (!isAuthenticated)
            {
                Console.Clear();
                Logger.Info("SMART FACTORY SYSTEM - LOGIN");

                Console.Write("Username: ");
                string username = Console.ReadLine() ?? "";

                Console.Write("Password: ");
                string password = Console.ReadLine() ?? "";

                
                isAuthenticated = authService.Login(username, password);

                if (!isAuthenticated)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n[Eroare] Informatii incorecte! Incearca din nou.");
                    Console.ResetColor();
                    Console.WriteLine("Apasa Enter pentru a reincerca...");
                    Console.ReadLine();
                }
            }

   
            MenuManagement menu = new MenuManagement(electronicsFactory, authService);
            menu.DisplayMenu();

        }
    }
}
   