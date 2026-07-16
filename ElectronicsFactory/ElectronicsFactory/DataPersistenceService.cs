using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace ElectronicsFactory
{
    internal class DataPersistenceService
    {
        private readonly FileStorageService _storageService;

        public DataPersistenceService(FileStorageService storageService)
        {
            _storageService = storageService;
        }

        // PRODUCTS PERSISTENCE
        public void SaveProducts(string filename, ProductManagement productManager)
        {
            var rows = productManager.Storage.Select(p => p.ToFileRow());
            _storageService.Write(filename, rows);
        }

        public Product? LoadProduct(string row)
        {
            if (string.IsNullOrWhiteSpace(row)) return null;

            List<string> tokens = row.Split(';').ToList();
            if (tokens.Count < 6) return null;

            int id = int.Parse(tokens[0]);
            string name = tokens[1];
            float price = float.Parse(tokens[2]);
            float consumption = float.Parse(tokens[3]);
            string quality = tokens[4];
            ProductType_t type = Enum.Parse<ProductType_t>(tokens[5]);

            Product? product = type switch
            {
                ProductType_t.Phones when tokens.Count >= 8 => new Phones(name, price, consumption, quality, type, int.Parse(tokens[6]), tokens[7]),
                ProductType_t.Tablets when tokens.Count >= 8 => new Tablets(name, price, consumption, quality, type, int.Parse(tokens[6]), tokens[7]),
                ProductType_t.Computers when tokens.Count >= 8 => new Computers(name, price, consumption, quality, type, tokens[7], int.Parse(tokens[6])),
                ProductType_t.Headphones when tokens.Count >= 8 => new Headphones(name, price, consumption, quality, type, int.Parse(tokens[6]), int.Parse(tokens[7])),
                _ => null
            };

            if (product != null)
            {
                var idField = typeof(Product).GetField("_id", BindingFlags.NonPublic | BindingFlags.Instance);
                idField?.SetValue(product, id);
            }

            return product;
        }
        public void LoadProducts(string filename, ProductManagement productManager)
        {
            IEnumerable<string> rows = _storageService.Read(filename);
            productManager.Storage.Clear();

            foreach (var row in rows)
            {
                Product? product = LoadProduct(row);

                if (product != null)
                {
                    productManager.AddProduct(product);
                }
            }
        }

        // EMPLOYEES PERSISTENCE
        public void SaveEmployees(string filename, EmployeeManagement employeeManager)
        {
            var rows = employeeManager.Employees.Select(e => e.ToFileRow());
            _storageService.Write(filename, rows);
        }

        public Employee? LoadEmployee(string row)
        {
            if (string.IsNullOrWhiteSpace(row)) return null;

            List<string> tokens = row.Split(';').ToList();
            if (tokens.Count < 8) return null;

            string jobName = tokens[0];
            string id = tokens[1];
            string name = tokens[2];

            // Potrivirea corectă a indecșilor conform ToFileRow():
            string username = tokens[3];
            string password = tokens[4];
            double salary = double.Parse(tokens[5]);
            // note: tokens[6] este Department, iar tokens[7] este JobStatus (sunt gestionate deja de constructorul fiecărei clase)

            Employee? employee = jobName switch
            {
                "Director" => new Director(name, salary, username, password),
                "ProductionManager" => new ProductionManager(name, salary, username, password),
                "MachineOperator" => new MachineOperator(name, salary, username, password),
                "Engineer" => new Engineer(name, salary, username, password),
                "Technician" => new Technician(name, salary, username, password),
                "SalesAgent" => new SalesAgent(name, salary, username, password),
                "Accountant" => new Accountant(name, salary, username, password),
                _ => null
            };

            if (employee != null)
            {
                var idProp = typeof(Employee).GetProperty("Id", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (idProp != null && idProp.CanWrite)
                {
                    idProp.SetValue(employee, id);
                }
                else
                {
                    var backingField = typeof(Employee).GetField($"<{idProp?.Name}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);
                    backingField!.SetValue(employee, id);
                }
            }

            return employee;
        }

        public void LoadEmployees(string filename, EmployeeManagement employeeManager)
        {
            var rows = _storageService.Read(filename);
            employeeManager.Employees.Clear();

            if (!rows.Any())
            {
                var defaultAdmin = new Director("ADMIN", 9000, "admin", "admin");
                employeeManager.HiredEmployee(defaultAdmin);
                SaveEmployees(filename, employeeManager);
                return;
            }

            foreach (var row in rows)
            {
                Employee? employee = LoadEmployee(row);

                if (employee != null)
                {
                    employeeManager.HiredEmployee(employee);
                }
            }
        }

        // MACHINES PERSISTENCE
        public void SaveMachines(string filename, MachineManagement machineManager)
        {
            var rows = machineManager.Machines.Select(m => m.ToFileRow());
            _storageService.Write(filename, rows);
        }

        public Machine? LoadMachine(string row)
        {
            if (string.IsNullOrWhiteSpace(row)) return null;

            List<string> tokens = row.Split(';').ToList();
            if (tokens.Count < 10)
            {
                return null ;
            }

            string machineType = tokens[0];
            int id = int.Parse(tokens[1]);
            string name = tokens[2];
            string serial = tokens[3];
            MachineStatus_t status = Enum.Parse<MachineStatus_t>(tokens[4]);
            ConditionStatus_t condition = Enum.Parse<ConditionStatus_t>(tokens[5]);
            float wear = float.Parse(tokens[6]);
            int hours = int.Parse(tokens[7]);
            int success = int.Parse(tokens[8]);
            int total = int.Parse(tokens[9]);

            Machine? machine = machineType switch
            {
                "PackagingMachine" => new PackagingMachine(name, serial, status, condition),
                "PcbFabricationMachine" => new PcbFabricationMachine(name, serial, status, condition),
                "AssemblyMachine" => new AssemblyMachine(name, serial, status, condition),
                "TestingMachine" => new TestingMachine(name, serial, status, condition),
                _ => null
            };

            if (machine != null)
            {
                var idField = typeof(Machine).GetField("_id", BindingFlags.NonPublic | BindingFlags.Instance);

                if (idField != null)
                {
                    idField.SetValue(machine, id);
                }
                else
                {
                    var idProp = typeof(Machine).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
                    idProp?.GetSetMethod(nonPublic: true)?.Invoke(machine, new object[] { id });
                }

                machine.WearLevel = wear;
                machine.TotalHoursOperated = hours;
                machine.SuccessfulCycles = success;
                machine.TotalCyclesAttempted = total;
                machine.Status = status;
            }

            return machine;
        }

        public void LoadMachines(string filename, MachineManagement machineManager)
        {
            var rows = _storageService.Read(filename);
            machineManager.Machines.Clear();

            foreach (var row in rows)
            {
                Machine? machine = LoadMachine(row);

                if (machine != null)
                {
                    machineManager.AddMachine(machine);
                }
            }
        }

        // ORDERS PERSISTENCE
        public void SaveOrders(string filename, OrderPriorityService orderManager)
        {
            var rows = new List<string>();

            foreach(var order in orderManager.GetAllOrders())
            {
                string row = $"{order.Product.Id};{order.Quantity};{order.Priority};{order.Requester}";
                rows.Add(row);
            }

            _storageService.Write(filename, rows);
            Logger.Info("Production orders successfully saved.");
        }

        public void LoadOrders(string filename, OrderPriorityService orderManager, ProductManagement productManager)
        {
            var rows = _storageService.Read(filename);
            orderManager.ClearAllOrders(); 

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row)) continue;

                string[] tokens = row.Split(';');
                if (tokens.Length < 4) continue;

                int productId = int.Parse(tokens[0]);
                int quantity = int.Parse(tokens[1]);
                PriorityLevel_t priority = Enum.Parse<PriorityLevel_t>(tokens[2]);
                string requester = tokens[3];

                Product? product = productManager.Storage.FirstOrDefault(p => p.Id == productId);

                if (product != null)
                {
                    orderManager.AddOrder(product, quantity, priority, requester);
                }
                else
                {
                    Logger.Warning($"Could not load order for Product ID {productId} because the product does not exist in the database.");
                }
            }
            orderManager.RebuildQueue();
            Logger.Info("Production orders successfully loaded.");
        }
    }
}
