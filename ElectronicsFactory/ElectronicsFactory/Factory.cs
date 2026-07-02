using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsFactory
{
    internal class Factory
    {
        public EmployeeManagement EmployeeManager { get; private set; }
        public MachineManagement MachineManager { get; private set; }
        public ProductManagement ProductManager { get; private set; }
        public float Income { get; set; }

        public Factory(int maxEmployees, int maxMachines, int maxProducts, float initialIncome)
        {
            EmployeeManager = new EmployeeManagement(maxEmployees);
            MachineManager = new MachineManagement(maxMachines);
            ProductManager = new ProductManagement(maxProducts);
            Income = initialIncome;
        }

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

           
            for (int i = 0; i < quantity; i++)
            {
                // A machine condition decreases after each production cycle
                bool processSuccess = machine.Process(targetProduct);

                if (processSuccess)
                {
                    
                    ProductManager.AddProduct(targetProduct);
                }
                else
                {
                    Logger.Error($"Production stopped unexpectedly at item {i + 1} due to machine failure.");
                    break;
                }
            }

            Logger.Info("Production cycle finished.");
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
            if (machine != null)
            {
                // Diagnostics should produce different results depending on the machine type
                machine.Inspect();
                Logger.Info($"Engineer {engineer.Name} completed inspection on {machineSerial}.");
            }
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
    }
}
    

