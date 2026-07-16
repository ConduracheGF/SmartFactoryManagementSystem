namespace ElectronicsFactory
{
    internal enum MenuExitReason
    {
        Logout,
        CloseApplication
    }

    /// <summary>
    /// Console-based user interface layer for the Smart Factory Management System
    /// Renders menus, reads user input, and delegates all business logic to the underlying instance
    /// </summary>
    internal class MenuManagement
    {
        private Factory _factory;
        private AuthService _authService; 
    
        public MenuManagement(Factory factory, AuthService authService)
        {
            _factory = factory;
            _authService = authService;
        }

        // Displays the main menu loop, routing user choices to the appropriate submenu until the user chooses to exit
        public MenuExitReason DisplayMenu()
        {
            bool running = true;
            MenuExitReason exitReason = MenuExitReason.CloseApplication;

            while (running)
            {
                Logger.Clear();
                Logger.Info("SMART FACTORY MANAGEMENT SYSTEM");

                if (_authService.CurrentUser != null)
                {
                    Logger.Info($"Logged in as: {_authService.CurrentUser.Name} | Role: {_authService.CurrentUser.GetType().Name}");
                }

                Logger.Info($"Current Factory Budget: {_factory.Income} RON");
                Logger.Info("1. Employee Management");
                Logger.Info("2. Machine Management");
                Logger.Info("3. Product Management");
                Logger.Info("4. Production");
                Logger.Info("5. Sales & Reports");

                if(_authService.IsDirector)
                {
                    Logger.Info("6. Factory Overview");
                }

                Logger.Info("7. Manage Production Orders");

                string lastActionDesc = _factory.UndoManager.GetLastActionDescription();
                Logger.Info($"8. Undo Last Action: {lastActionDesc}");
                Logger.Info($"9. LogOut");
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
                    case "6": 
                        if (_authService.IsDirector)
                        {
                            SubMenuFactoryOverview();
                        }
                        else
                        {
                            Logger.Error("Access Denied! Only the Director can view the factory overview.");
                            Console.ReadKey();
                        }
                        break;

                    case "7": 
                        SubMenuOrders();
                        break;

                    case "8":
                        _factory.UndoManager.UndoLastOperation();

                        var persistence = new DataPersistenceService(new FileStorageService());
                        persistence.SaveEmployees("employees.txt", _factory.EmployeeManager);
                        persistence.SaveOrders("orders.txt", _factory.OrderManager);
                        persistence.SaveProducts("products.txt", _factory.ProductManager);

                        WaitForEnter();
                        break;
                    case "9":
                        _authService.Logout();
                        exitReason = MenuExitReason.Logout;
                        running = false;
                        break;
                    case "0":
                        exitReason = MenuExitReason.CloseApplication;
                        running = false;
                        break;
                    default:
                        Logger.Info("Invalid choice! Press Enter to retry...");
                        Console.ReadKey();
                        break;
                }
            }
            return exitReason;
        }

        // Pauses execution until the user presses a key, used to let the user read output before returning to the previous menu
        private void WaitForEnter()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Logger.Info("\n[Press Enter to return to the menu...]");
            Logger.ResetColor();
            Console.ReadKey();
        }

        // Handles employee management: hiring, firing, and listing all employees
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

                if (!_authService.IsDirector)
                {
                    Logger.Error("Access Denied! Only Director can hire staff.");
                    WaitForEnter();
                    return;
                }
                // Hire a new employee of the chosen type. ID is auto-generated internally
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
               

                if (newEmp != null)
                {
                    
                    _factory.EmployeeManager.HiredEmployee(newEmp);

                   
                    _factory.UndoManager.RegisterAction(new HireEmployeeAction(_factory.EmployeeManager, newEmp));

                   
                    var persistence = new DataPersistenceService(new FileStorageService());
                    persistence.SaveEmployees("employees.txt", _factory.EmployeeManager);

                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    string operatorName = _authService.CurrentUser?.Username ?? "System";
                    new FileStorageService().Append("operations.txt", $"{timestamp} | {operatorName} | Employee added: {newEmp.Name} (ID: {newEmp.Id})"); 

                    Logger.Info($"Employee {newEmp.Name} hired successfully and saved to file.");
                    WaitForEnter();
                }
            }
            else if (sub == "2")
            {

                if (!_authService.IsDirector)
                {
                    Logger.Error("Access Denied! Only Director can fire staff.");
                    WaitForEnter();
                    return;
                }
                // Fire an employee by ID
                Logger.Info("Enter Unique ID: ");
                string? id = Console.ReadLine()!;

                if (!string.IsNullOrEmpty(id))
                {

                    int index = _factory.EmployeeManager.SearchEmployee(id);

                    if (index != -1)
                    {
                        Employee empToFire = _factory.EmployeeManager.Employees[index];


                        _factory.EmployeeManager.FiredEmployee(id);


                        _factory.UndoManager.RegisterAction(new FireEmployeeAction(_factory.EmployeeManager, empToFire));


                        var persistence = new DataPersistenceService(new FileStorageService());
                        persistence.SaveEmployees("employees.txt", _factory.EmployeeManager);


                        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        string operatorName = _authService.CurrentUser?.Username ?? "System";
                        new FileStorageService().Append("operations.txt", $"{timestamp} | {operatorName} | Employee removed with ID: {id}");

                        Logger.Info($"Employee {empToFire.Name} was fired. You can now undo this action.");
                    }
                    else
                    {
                        Logger.Error("Employee not found!");
                    }
                }

                
            }
            else if (sub == "3")
            {
                // Each derived employee and machine class should override at least one inherited method
                foreach (var emp in _factory.EmployeeManager.Employees)
                {
                    if (emp != null)
                    {
                        emp.DisplayInfo();
                    }
                }
            }
            WaitForEnter();
        }

        // Handle Production Workflow:
        // Collecting the manager/operator/machine and the product type/quantity, then delegating to the factory to manufacture it
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
            if (prodType == "1")
                target = new Phones("PhoneTemplate", 1200f, 2.5f, "A", ProductType_t.Phones, 2026, "M3");

            if (prodType == "2")
                target = new Headphones("HeadphonesTemplate", 300f, 1.2f, "B", ProductType_t.Headphones, 2024, 50);

            if (prodType == "3")
                target = new Computers("ComputerTemplate", 500f, 3.0f, "C", ProductType_t.Computers, "i5", 10);

            if (prodType == "4")
                target = new Tablets("TabletTemplate", 300f, 1.7f, "B", ProductType_t.Tablets, 2023, "snapdragon");

            if (target != null)
            {
                _factory.RunProductionCycle(managerId, operatorId, serial, target, qty);
            }
            WaitForEnter();
        }

        // Handles machine management:
        // Inspection, repair, and viewing installed components
        private void SubMenuMachines()
        {
            Console.Clear();
            Logger.Info("Machine Management");
            Logger.Info("1. Inspect Machine");
            Logger.Info("2. Repair Machine");
            Logger.Info("3. View Machine Components");
            Logger.Info("4. Production Efficiency Dashboard");
            Logger.Info("5. Predictive Maintenance Report");
            Logger.Info("6. Check Machine Alerts");
            Logger.Info("Choice: ");
            string? sub = Console.ReadLine();

            if (sub == "4")
            {
                _factory.HealthService.DisplayEfficencyDashboard();
                WaitForEnter();
                return;
            }
            else if (sub == "5")
            {
                _factory.HealthService.RunPredictiveMaintenanceReport();
                WaitForEnter();
                return;
            }
            else if (sub == "6")
            {
                _factory.HealthService.CheckMachineAlerts();
                WaitForEnter();
                return;
            }

            Logger.Info("Enter Machine Serial: "); string? serial = Console.ReadLine()!;

            if (sub == "1")
            {
                if (!_authService.IsEngineer)
                {
                    Logger.Error("Access Denied! Only Engineers can inspect machines.");
                    WaitForEnter();
                    return;
                }
                Logger.Info("Enter Engineer ID: "); string? engId = Console.ReadLine()!;
                _factory.InspectMachineWithEngineer(engId, serial);
            }
            else if (sub == "2")
            {

                if (!_authService.IsTechnician)
                {
                    Logger.Error("Access Denied! Only Technicians can repair machines.");
                    WaitForEnter();
                    return;
                }
                Logger.Info("Enter Technician ID: "); string? techId = Console.ReadLine()!;
                _factory.RepairMachineWithTechnician(techId, serial);
            }
            else if (sub == "3")
            {
                // Display all installed components for the requested machine
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

        // Displays the current product inventory, including type-specific functionality descriptions via polymorphic dispatch
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
                    Logger.Info($"Product ID: {prod.Id} | Type: {prod.ProductType} | Cost: {prod.Price}| Quality: {prod.Quality}");

                    // Invoke the type-specific description for each concrete product subtype
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

        // Handles sales and reporting: selling a product, or generating the accountant's financial report
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

                if (!_authService.IsSalesAgent)
                {
                    Logger.Error("Access Denied! Only Sales Agents can sell products.");
                    WaitForEnter();
                    return;
                }
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
                if (!_authService.IsAccountant)
                {
                    Logger.Error("Access Denied! Only Accountants can generate reports.");
                    WaitForEnter();
                    return;
                }
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



        // Displays the factory-wide overview report, generated by a Director
        private void SubMenuFactoryOverview()
        {
            Logger.Clear();
            Logger.Info("Factory Overview");
            Logger.Info("Enter Director ID: ");
            string? directorId = Console.ReadLine()!;

            _factory.GenerateDirectorReport(directorId);

            WaitForEnter();
        }

        private void SubMenuOrders()
        {
            Console.Clear();
            Logger.Info("PRODUCTION ORDERS MANAGEMENT");
            Logger.Info("1. Add New Production Order");
            Logger.Info("2. Display All Orders (Sorted by Priority)");
            Logger.Info("3. Process Next Order (Highest Priority)");
            Logger.Info("4. Back to Main Menu");
            Console.Write("Choice: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddNewOrder();
                    break;
                case "2":
                    DisplayOrders();
                    break;
                case "3":
                    ProcessNextOrder();
                    break;
                case "4":
                    return;
                default:
                    Logger.Error("Invalid option!");
                    WaitForEnter();
                    break;
            }
        }

        private void AddNewOrder()
        {
            Console.Clear();
            Logger.Info("ADD NEW PRODUCTION ORDER");

            if (!_authService.IsProductionManager)
            {
                Logger.Error("Access Denied! Only Production Manager can create production orders!");
                WaitForEnter();
                return;
            }

            
            if (!_factory.ProductManager.Storage.Any())
            {
                Logger.Warning("No products available in the database! Add products first.");
                WaitForEnter();
                return;
            }

            Logger.Info("Available Products:");
            foreach (var prod in _factory.ProductManager.Storage)
            {
                Console.WriteLine($"ID: {prod.Id} | Name: {prod.Name} | Price: {prod.Price}");
            }

            Console.Write("\nEnter Product ID: ");
            if (!int.TryParse(Console.ReadLine(), out int prodId)) return;

            var product = _factory.ProductManager.Storage.FirstOrDefault(p => p.Id == prodId);
            if (product == null)
            {
                Logger.Error("Product not found!");
                WaitForEnter();
                return;
            }

            
            Console.Write("Enter Quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0)
            {
                Logger.Error("Invalid quantity!");
                WaitForEnter();
                return;
            }

            
            Logger.Info("Select Priority: 1. Low, 2. Medium, 3. High, 4. Critical");
            string? pChoice = Console.ReadLine();
            PriorityLevel_t priority = pChoice switch
            {
                "1" => PriorityLevel_t.Low,
                "2" => PriorityLevel_t.Medium,
                "3" => PriorityLevel_t.High,
                "4" => PriorityLevel_t.Critical,
                _ => PriorityLevel_t.Low
            };

            
            string requester = _authService.CurrentUser?.Name ?? "System";

            
            _factory.OrderManager.AddOrder(product, qty, priority, requester);

            
            var persistence = new DataPersistenceService(new FileStorageService());
            persistence.SaveOrders("orders.txt", _factory.OrderManager);

            Logger.Info("Order added successfully and saved to orders.txt!");
            WaitForEnter();
        }

        private void DisplayOrders()
        {
            Console.Clear();
            Logger.Info("PRODUCTION ORDERS QUEUE (Sorted by Priority)");

            var orders = _factory.OrderManager.GetAllOrders();

            if (!orders.Any())
            {
                Logger.Warning("No pending production orders.");
                WaitForEnter();
                return;
            }

            foreach (var order in orders)
            {
                Console.WriteLine($"[Priority: {order.Priority}] Product: {order.Product.Name} (ID: {order.Product.Id}) | Qty: {order.Quantity} | Requested by: {order.Requester}");
            }

            WaitForEnter();
        }

        private void ProcessNextOrder()
        {
            Console.Clear();
            Logger.Info("PROCESSING HIGHEST PRIORITY ORDER");

            
            var nextOrder = _factory.OrderManager.GetNextOrder();

            if (nextOrder == null)
            {
                Logger.Warning("No orders left to process!");
                WaitForEnter();
                return;
            }

            Logger.Info($"Processing order for: {nextOrder.Quantity}x {nextOrder.Product.Name}...");
            Logger.Info($"[SUCCESS] Order requested by {nextOrder.Requester} has been completed!");

            
            var persistence = new DataPersistenceService(new FileStorageService());
            persistence.SaveOrders("orders.txt", _factory.OrderManager);

            
            var storage = new FileStorageService();
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            storage.Append("operations.txt", $"{timestamp} | {_authService.CurrentUser?.Username} | Processed order for {nextOrder.Quantity}x {nextOrder.Product.Name}");

            WaitForEnter();
        }

        
       
    }
}