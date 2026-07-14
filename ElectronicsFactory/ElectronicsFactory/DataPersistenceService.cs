using System;
using System.Collections.Generic;
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

            return (product != null) ? product : null;
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
                    product.GetType().GetField("_id", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(product, product.Id);
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

            return (employee != null) ? employee : null;
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
                    var idProperty = employee.GetType().GetProperty("Id");
                    if (idProperty != null)
                    {
                        idProperty.DeclaringType?
                                  .GetProperty("Id")?
                                  .GetSetMethod(nonPublic: true)?
                                  .Invoke(employee, new object[] { employee.Id });
                    }

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
                    machineManager.AddMachine(machine);
            }
        }
    }
}
