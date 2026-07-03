namespace ElectronicsFactory
{
    internal class MenuManagement
    {
        private Factory _factory;

        public MenuManagement(Factory factory)
        {
            _factory = factory;
        }

        public void DisplayMenu()
        {
            bool running = true;
            while (running)
            {
                Logger.Clear();
                Logger.Info("SMART FACTORY MANAGEMENT SYSTEM");
                Logger.Info($"Current Factory Budget: {_factory.Income} RON");
                Logger.Info("1. Employee Management");
                Logger.Info("2. Machine Management");
                Logger.Info("3. Product Management");
                Logger.Info("4. Production");
                Logger.Info("5. Sales & Reports");
                Logger.Info("6. Factory Overview");
                Logger.Info("0. Exit");
                Logger.Info("\nSelect an option: ");

                string? choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": SubMenuEmployees(); break;
                    case "2": SubMenuMachines(); break;
                    case "3": SubMenuProducts(); break;
                    case "4": SubMenuProductionWorkflow(); break;
                    case "5": SubMenuReports(); break;
                    case "6": SubMenuFactoryOverview(); break;
                    case "0": running = false; break;
                    default:
                        Logger.Info("Invalid choice! Press Enter to retry...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void WaitForEnter()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Logger.Info("\n[Press Enter to return to the menu...]");
            Logger.ResetColor();
            Console.ReadKey();
        }

        private void SubMenuEmployees()
        {
            Console.Clear();
            Logger.Info("Employee Management");
            Logger.Info("1. Hire Employee");
            Logger.Info("2. Fire Employee");
            Logger.Info("3. Display All Employees");
            Logger.Info("Choice: ");
            string? sub = Console.ReadLine()!;

            if (sub == "1")
            {
                Logger.Info("Name: "); string? name = Console.ReadLine()!;
                Logger.Info("Salary: "); double salary = double.Parse(Console.ReadLine()!);
                if ( salary <= 0)
                {
                    Logger.Info("Invalid salary! Please enter a positive value.");
                    WaitForEnter();
                    return;
                }
                Logger.Info("Type: 1. Production Manager, 2. Machine Operator, 3. Engineer, 4. Technician, 5. Sales Agent, 6. Accountant");
                string? type = Console.ReadLine()!;

                Employee? newEmp = null;
               
                if (type == "1") newEmp = new ProductionManager(name, salary);
                if (type == "2") newEmp = new MachineOperator(name, salary);
                if (type == "3") newEmp = new Engineer(name, salary); 
                if (type == "4") newEmp = new Technician(name, salary);
                if (type == "5") newEmp = new SalesAgent(name, salary); 
                if (type == "6") newEmp = new Accountant(name, salary);
                if (type == "7") newEmp = new Director(name, salary);

                if (newEmp != null) _factory.EmployeeManager.HiredEmployee(newEmp);
            }
            else if (sub == "2")
            {
                Logger.Info("Enter Unique ID: "); string? id = Console.ReadLine()!;

                if (id != null) _factory.EmployeeManager.FiredEmployee(id);
            }
            else if (sub == "3")
            {
                // Each derived employee and machine class should override at least one inherited method
                foreach (var emp in _factory.EmployeeManager.Employees)
                {
                    if (emp != null)
                    emp.DisplayInfo();
                }
            }
            WaitForEnter();
        }

        private void SubMenuProductionWorkflow()
        {
            Console.Clear();
            Logger.Info("Production Simulation");
            Logger.Info("Enter Production Manager ID: "); string? managerId = Console.ReadLine()!;
            Logger.Info("Enter Machine Operator ID: "); string? operatorId = Console.ReadLine()!;
            Logger.Info("Enter Machine Serial Number: "); string? serial = Console.ReadLine()!;

            Logger.Info("Select Product to create: 1. Phone, 2. Headphones, 3. Computers, 4. Tablets");
            string? prodType = Console.ReadLine();
            Logger.Info("Quantity: "); int.TryParse(Console.ReadLine()!, out int qty);
            if (qty <= 0)
            {
                Logger.Info("Invalid quantity! Please enter a positive value.");
                WaitForEnter();
                return;
            }

            Product? target = null;
            // Products cannot have a negative production cost or selling price
            if (prodType == "1") target = new Phones(currency: 1200, consumption: 2.5f, quality: "A", ProductType_t.Phones, yearOfProduction: 2026, processor: "M3");
            if (prodType == "2") target = new Headphones(currency: 300, consumption: 1.2f, quality: "B", ProductType_t.Headphones, yearOfProduction: 2024, power: 50);
            if (prodType == "3") target = new Computers(currency: 500, consumption: 3.0f, quality: "C", ProductType_t.Computers, processor: "i5", weight: 10);
            if (prodType == "4") target = new Tablets(currency: 300, consumption: 1.7f, quality: "B", ProductType_t.Tablets, yearOfProduction: 2023, processor: "snapdragon");

            if (target != null)
            {
                _factory.RunProductionCycle(managerId, operatorId, serial, target, qty);
            }
            WaitForEnter();
        }

        private void SubMenuMachines()
        {
            Console.Clear();
            Logger.Info("Machine Management");
            Logger.Info("1. Inspect Machine");
            Logger.Info("2. Repair Machine");
            Logger.Info("3. View Machine Components");
            Logger.Info("Choice: ");
            string? sub = Console.ReadLine();

            Logger.Info("Enter Machine Serial: "); string? serial = Console.ReadLine()!;

            if (sub == "1")
            {
                Logger.Info("Enter Engineer ID: "); string? engId = Console.ReadLine()!;
                _factory.InspectMachineWithEngineer(engId, serial);
            }
            else if (sub == "2")
            {
                Logger.Info("Enter Technician ID: "); string? techId = Console.ReadLine()!;
                _factory.RepairMachineWithTechnician(techId, serial);
            }
            else if (sub == "3")
            {
                Machine? machine = _factory.MachineManager.FindMachine(serial);
                if (machine != null && machine.Components != null)
                {
                    Logger.Info($"Components installed on machine {machine.SerialNumber}:");
                    foreach (var part in machine.Components)
                    {
                        if (part != null)
                        {
                            Logger.Info($" -> {part.Component} | Brand: {part.Brand} | Energy Class: {part.EnergyClass} | Cost: {part.Currency} RON");
                        }
                    }
                }
            }
            WaitForEnter();
        }

        private void SubMenuProducts()
        {
            Console.Clear();
            Logger.Info("Product Management & Inventory");

            bool empty = true;
            foreach (var prod in _factory.ProductManager.Storage)
            {
                if (prod != null)
                {
                    empty = false;
                    Logger.Info($"Product ID: {prod.Id} | Type: {prod.ProductType} | Cost: {prod.Currency} RON | Quality: {prod.Quality}");

                    if (prod is Phones phone) phone.DisplayFunctionality();
                    else if (prod is Tablets tablet) tablet.DisplayFunctionality();
                    else if (prod is Computers computer) computer.WifiConectionDescription();
                    else if (prod is Headphones headphones) headphones.TestProduct();
                }
            }

            if (empty)
            {
                Logger.Warning("Inventory is currently empty. No products in storage.");
            }

            WaitForEnter();
        }
        private void SubMenuReports()
        {
            Console.Clear();
            Logger.Info("Sales & Reports Management");
            Logger.Info("1. Sell a Product");
            Logger.Info("2. Generate Factory Financial Report");
            Logger.Info("Choice: ");
            string? sub = Console.ReadLine();

            if (sub == "1")
            {
                Logger.Info("Enter Sales Agent ID: "); string agentId = Console.ReadLine() ?? "";
                Logger.Info("Enter Product ID to sell: "); int prodId = int.Parse(Console.ReadLine() ?? "0");

                if (prodId <= 0)
                {
                    Logger.Info("Invalid product ID! Please enter a positive value.");
                    WaitForEnter();
                    return;
                }

                _factory.SellProductWithAgent(agentId, prodId);
            }
            else if (sub == "2")
            {
                Logger.Info("Enter Accountant ID: "); string accId = Console.ReadLine() ?? "";

                int index = _factory.EmployeeManager.SearchEmployee(accId);
                if (index != -1 && _factory.EmployeeManager.Employees[index] is Accountant accountant)
                {
                    accountant.CalculateProductionValue(_factory.ProductManager, _factory.MachineManager, _factory.Income);
                }
                else
                {
                    Logger.Error("Invalid Accountant ID!");
                }
            }
            WaitForEnter();
        }

        private void SubMenuFactoryOverview()
        {
            Logger.Clear();
            Logger.Info("Factory Overview");
            Logger.Info("Enter Director ID: ");
            string? directorId = Console.ReadLine()!;

            _factory.GenerateDirectorReport(directorId);

            WaitForEnter();
        }
    }
}