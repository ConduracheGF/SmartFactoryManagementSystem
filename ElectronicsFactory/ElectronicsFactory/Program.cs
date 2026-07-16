using System;

namespace ElectronicsFactory
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Smart Factory Management System";

            
            const string EmployeesFile = "employees.txt";
            const string MachinesFile = "machines.txt";
            const string ProductsFile = "products.txt";
            const string OrdersFile = "orders.txt";
            const string OperationsFile = "operations.txt";

            FileStorageService storageService = new FileStorageService();
            DataPersistenceService persistenceService = new DataPersistenceService(storageService);

           
            Factory electronicsFactory = new Factory(50000f);

            persistenceService.LoadProducts(ProductsFile, electronicsFactory.ProductManager);
            persistenceService.LoadEmployees(EmployeesFile, electronicsFactory.EmployeeManager);
            persistenceService.LoadMachines(MachinesFile, electronicsFactory.MachineManager);
            persistenceService.LoadOrders(OrdersFile, electronicsFactory.OrderManager, electronicsFactory.ProductManager);

            
            AuthService authService = new AuthService(electronicsFactory.EmployeeManager, storageService, OperationsFile);

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
                    Console.WriteLine("\n[Error] Wrong information. Try again");
                    Console.ResetColor();
                    Console.WriteLine("Press enter to reload.");
                    Console.ReadLine();
                }
            }

            MenuManagement menu = new MenuManagement(electronicsFactory, authService);
            menu.DisplayMenu();
        }
    }
}