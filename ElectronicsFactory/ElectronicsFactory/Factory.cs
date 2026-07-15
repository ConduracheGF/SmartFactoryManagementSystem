using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsFactory
{
    /// <summary>
    /// Central orchestrator of the Smart Factory Management System
    /// Owns the employee, machine, and product managers, and coordinates all cross-cutting
    /// business workflows (production, inspection, repair, sales, reporting).
    /// </summary>
    internal class Factory
    {
        // Manages the factory's employee roster
        public EmployeeManagement EmployeeManager { get; private set; }

        // Manages the factory's registered machines
        public MachineManagement MachineManager { get; private set; }

        // Manages the factory's product inventory
        public ProductManagement ProductManager { get; private set; }

        public OrderPriorityService OrderManager { get; private set; } = new OrderPriorityService();

        public UndoService UndoManager { get; private set; } = new UndoService();

        // Current factory income/budget, in RON
        public float Income { get; set; }

        // Initializes a new Factory with the given capacities and starting income
        public Factory(int maxEmployees, int maxMachines, int maxProducts, float initialIncome)
        {
            EmployeeManager = new EmployeeManagement();
            MachineManager = new MachineManagement();
            ProductManager = new ProductManagement();
            Income = initialIncome;
            UndoManager = new UndoService();
        }

        /// Validates the manager/operator/machine
        /// Starts the machine if needed, then manufactures the requested quantity
        /// of a product one unit at a time (stopping early if the machine breaks down)
        public void RunProductionCycle(string managerId, string operatorId, string machineSerial, Product targetProduct, int quantity)
        {
            // Product quantity can never become negative
            if (quantity <= 0)
            {
                Logger.Error("Quantity must be greater than 0!");
                return;
            }

            // Only a Production Manager can create production orders
            int managerIdx = EmployeeManager.SearchEmployee(managerId);
            if (managerIdx == -1 || !(EmployeeManager.Employees[managerIdx] is ProductionManager))
            {
                Logger.Error("Order creation failed: Valid Production Manager ID required!");
                return;
            }

            // Only a Machine Operator can start production.
            int operatorIdx = EmployeeManager.SearchEmployee(operatorId);
            if (operatorIdx == -1 || !(EmployeeManager.Employees[operatorIdx] is MachineOperator operatorObj))
            {
                Logger.Error("Production failed: Valid Machine Operator ID required!");
                return;
            }

            
            Machine? machine = MachineManager.FindMachine(machineSerial);
            if (machine == null) return;

            // A machine cannot produce products while stopped.
            if (machine.Status != MachineStatus_t.Running)
            {
                Logger.Warning($"Machine {machineSerial} is not Running. Current status: {machine.Status}. Operator is attempting to start it.");

         
                bool started = operatorObj.StartMachine(machine);
                if (!started)
                {
                    Logger.Error("Production aborted: Machine could not be started.");
                    return;
                }
            }

            // A machine cannot produce if required machine parts are missing.
            if (machine.Condition == ConditionStatus_t.Critical)
            {
                Logger.Error("Production aborted: Machine condition is CRITICAL. Requires technician repair!");
                return;
            }

            Logger.Info($"Starting production of {quantity} x {targetProduct.GetType().Name}...");

            int producedCount = 0;

            for (int i = 0; i < quantity; i++)
            {
                // A machine condition decreases after each production cycle
                bool processSuccess = machine.Process(targetProduct);

                if (processSuccess)
                {
                    Product newProduct = targetProduct.Clone();
                    ProductManager.AddProduct(newProduct);
                    producedCount++;
                }
                else
                {
                    Logger.Error($"Production stopped unexpectedly at item {i + 1} due to machine failure.");
                    break;
                }
            }

            if (producedCount == quantity)
            {
                Logger.Info($"Production cycle finished successfully. {producedCount}/{quantity} units produced.");
            }
            else
            {
                Logger.Warning($"Production cycle finished incomplete. Only {producedCount}/{quantity} units were produced before machine failure.");
            }

            // La finalul logicii de producție din RunProductionCycle, după ce s-au adăugat produsele:
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            new FileStorageService().Append("operations.txt", $"{timestamp} | System | Produced {quantity} (Type: {targetProduct})");
        }

        //  An Engineer inspects the selected machine.
        public void InspectMachineWithEngineer(string engineerId, string machineSerial)
        {
            int engIdx = EmployeeManager.SearchEmployee(engineerId);
            if (engIdx == -1 || !(EmployeeManager.Employees[engIdx] is Engineer engineer))
            {
                Logger.Error("Only an Engineer can perform inspections!");
                return;
            }

            Machine? machine = MachineManager.FindMachine(machineSerial);
            if (machine == null) return;

            engineer.InspectMachine(machine);
        }

        // A Technician repairs the machine if necessary
        public void RepairMachineWithTechnician(string technicianId, string machineSerial)
        {
            int techIdx = EmployeeManager.SearchEmployee(technicianId);
            if (techIdx == -1 || !(EmployeeManager.Employees[techIdx] is Technician technician))
            {
                Logger.Error("Only a Technician can repair machines!");
                return;
            }

            Machine? machine = MachineManager.FindMachine(machineSerial);
            if (machine == null) return;

            //  A technician cannot repair a running machine.
            if (machine.Status == MachineStatus_t.Running)
            {
                Logger.Error($"Cannot repair machine {machineSerial} while it is RUNNING! Stop it first.");
                return;
            }

            float currentIncome = Income;
            technician.RepairMachine(machine, ref currentIncome);
            Income = currentIncome;

            
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            new FileStorageService().Append("operations.txt", $"{timestamp} | {techIdx} | Machine {machineSerial} successfully repaired");
        }

        // A Sales Agent sells products.
        public void SellProductWithAgent(string agentId, int productId)
        {
            int agentIdx = EmployeeManager.SearchEmployee(agentId);
            if (agentIdx == -1 || !(EmployeeManager.Employees[agentIdx] is SalesAgent))
            {
                Logger.Error("Only a Sales Agent can sell products!");
                return;
            }

            int prodIdx = ProductManager.Search(productId);
            // A Sales Agent cannot sell products that are not available in inventory.
            if (prodIdx == -1)
            {
                Logger.Error($"Product with ID {productId} is not available in inventory!");
                return;
            }

            Product p = ProductManager.Storage[prodIdx];

            // The inventory quantity must decrease after a successful sale.
            float currentIncome = Income;
            Income = ProductManager.SoldProduct(p, currentIncome); 

            Logger.Info($"Product {productId} successfully sold by Agent. New factory income: {Income} RON");
        }

        // Generates a factory-wide overview report through the given Director
        public void GenerateDirectorReport(string directorId)
        {
            int index = EmployeeManager.SearchEmployee(directorId);

            if((index == -1) || !(EmployeeManager.Employees[index] is Director director))
            {
                Logger.Error("Only a director can review factory-wide statics!");
                return;
            }

            director.ReviewProductionStatistics(EmployeeManager.EmployeeCount, MachineManager.MachineCount, ProductManager.ProductCount, Income);
        }
    }
}