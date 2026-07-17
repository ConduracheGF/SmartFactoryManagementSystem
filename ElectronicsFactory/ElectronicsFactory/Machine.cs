using ElectronicsFactory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml.Linq;

namespace ElectronicsFactory
{

    // Represents what a machine is currently doing
    public enum MachineStatus_t
    {
        Offline,
        Running,
        Maintenance,
        Broken
    }
    
    // Physical condition of a machine, degrades over production cycle
    public enum ConditionStatus_t
    {
        Good,
        Bad,
        Critical
    }

    // Abstract base class representing any production machine in the factory.
    public abstract class Machine
    {
        
        private static int _nextId = 1;

        
        public int Id { get; private set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public MachineStatus_t Status { get; set; }

        public ConditionStatus_t Condition { get; set; }

        public float WearLevel { get; set; }
        public int TotalHoursOperated { get; set; }
        public int SuccessfulCycles { get; set; }
        public int TotalCyclesAttempted { get; set; }

        public List<MachineParts> Components { get; set; } = new List<MachineParts>();

        public Machine(string name, string serialNumber, MachineStatus_t status, ConditionStatus_t condition)
        {
            Id = _nextId++;
            Name = name;
            SerialNumber = serialNumber;
            Status = MachineStatus_t.Offline;
            Condition = ConditionStatus_t.Good;

            WearLevel = condition switch
            {
                ConditionStatus_t.Critical => 85.0f,
                ConditionStatus_t.Bad => 50.0f,
                _ => 0.0f
            };

            TotalHoursOperated = 0;
            SuccessfulCycles = 0;
            TotalCyclesAttempted = 0;

            Components = new List<MachineParts>
            {
                new Motor(currency: 500, brand: "Bosch", energyClass: 2, component: ComponentsType_t.Motor, powerEnergy: 1500.0f, horsePower: 3),
                new Senzor(currency: 150, brand: "Sick", energyClass: 1, component: ComponentsType_t.Senzor, percentAccuracy: 0.98f, frequency: 100),
                new Controler(currency: 800, brand: "Siemens", energyClass: 1, component: ComponentsType_t.Controler, frequency: 400),
                new Display(currency: 300, brand: "Nextion", energyClass: 3, component: ComponentsType_t.Display, rezolution: 1080.0f),
                new CoolingFan(currency: 75, brand: "Noctua", energyClass: 1, component: ComponentsType_t.CoolingFan, speed: 2000)
            };
        }

        // Stops the machine, unless it is currently under maintenance
        public bool Stop()
        {
            if (Status == MachineStatus_t.Maintenance)
            {
                Logger.Warning($"Machine {SerialNumber} is undergoing maintenance and cannot be stopped!"); 
                return false;
            }



            Status = MachineStatus_t.Offline;

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            new FileStorageService().Append("operations.txt", $"{timestamp} | System | Machine {SerialNumber} ({Name}) stopped");

            Logger.Info($"The machine with serial number: {SerialNumber} has been stopped.");
            return true;
        }

        // Diagnoses the machine's current condition 
        // Overridden by derived classes to provide machine-type-specific diagnostic output
        public virtual bool Inspect()
        {
            return (Condition == ConditionStatus_t.Bad || Condition == ConditionStatus_t.Critical);
        }

        // Starts the machine, unless its condition is Bad or Critical, in which case it becomes Broken
        public virtual bool Start()
        {
            if (Condition == ConditionStatus_t.Critical || Condition == ConditionStatus_t.Bad)
            {
                Logger.Warning($"The machine with serial number: {SerialNumber} is defective and cannot start!"); 
                Status = MachineStatus_t.Broken;
                return false;
            }

         

            Logger.Info($"The machine with serial number {SerialNumber} started successfully!");
            Status = MachineStatus_t.Running;

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            new FileStorageService().Append("operations.txt", $"{timestamp} | System | Machine {SerialNumber} ({Name}) started");

            return true;
        }

        // Repairs the machine by replacing one randomly-selected component
        // Restoring its condition to Good and its status to Stopped
        public virtual bool Repair(ref float income)
        {
            if (Status == MachineStatus_t.Running)
            {
                throw new InvalidMachineStateException($"Cannot perform repair on machine {SerialNumber} while it is RUNNING! Stop it first.");
            }

            if (Components == null || Components.Count == 0)
            {
                throw new InvalidOperationException($"Machine {SerialNumber} does not have any components configured to be repaired!");
            }

            if (Status == MachineStatus_t.Running)
            {
                Logger.Warning($"Cannot perform repair on machine {SerialNumber} while it is running");
                return false;
            }
            Logger.Info("Tehnician has found a piece of machine which it has a bad functionality!");

            
            if (Components == null || Components.Count == 0)
            {
                Logger.Error("This machine does not have any components configured to be repaired!");
                return false;
            }


            int indexRandom = Random.Shared.Next(Components.Count);
     
            income = Components[indexRandom].Replacement(income);

            ComponentsType_t swapComponents = (ComponentsType_t)indexRandom;
            Logger.Warning($"The component with bad behavior is {swapComponents} and it will be replaced!");

            Condition = ConditionStatus_t.Good;
            Status = MachineStatus_t.Offline;
            WearLevel = 0.0f;
            return true;
        }

        // Processes a single unit of the given product
        // Base implementation handles status validation and condition degradation
        public virtual bool Process(Product product)
        {
            TotalCyclesAttempted++;
            TotalHoursOperated++;

            switch(Status)
            {
                case MachineStatus_t.Offline:
                    Logger.Warning($"The machine with serial number {SerialNumber} is stopped and cannot process products!"); 
                    return false;
                case MachineStatus_t.Running:
                    Logger.Info("The machine is processing products");
                    DegradeCondition();

                    if (Status != MachineStatus_t.Broken)
                    {
                        SuccessfulCycles++;
                    }
                    return true;
                case MachineStatus_t.Maintenance:
                    Logger.Warning($"The machine with serial number {SerialNumber} is under maintenance and cannot process products");
                    return false;
                case MachineStatus_t.Broken:
                    Logger.Error($"The machine is broken and cannot process products!");
                    return false;
                default: return false;
            }
        }

        // Randomly degrades the machine's condition after processing a unit
        // 20% chance per unit: escalates Good → Bad → Critical
        private void DegradeCondition()
        {
            if (Random.Shared.NextDouble() < 0.20)
            {
                Condition = (Condition == ConditionStatus_t.Good ? ConditionStatus_t.Bad : ConditionStatus_t.Critical);
                Logger.Warning($"Machine {SerialNumber} condition degraded to {Condition} after processing!");

                if (Condition == ConditionStatus_t.Bad) WearLevel = 55.0f;
                if (Condition == ConditionStatus_t.Critical)
                {
                    WearLevel = 100.0f;
                    Status = MachineStatus_t.Broken;
                }
            }
            else
            {
                WearLevel += 2.5f;
            }
        }
        public string ToFileRow()
        {
            return $"{GetType().Name};{Id};{Name};{SerialNumber};{Status};{Condition};{WearLevel};{TotalHoursOperated};{SuccessfulCycles};{TotalCyclesAttempted}";
        }
    }

   
    // Machine specialized in packaging finished products into boxes
    internal class PackagingMachine : Machine
    {
      
        public PackagingMachine(string name, string serialNumber, MachineStatus_t status, ConditionStatus_t condition)
                    : base(name, serialNumber, status, condition) { }
        public override bool Start()
        {
            if (base.Start())
            {
                Logger.Info("The packaging machine is packaging the product boxes.");
                return true;
            }
            return false;
        }


        // Packages a product unit and re-evaluates its quality
        // Aborts mid-cycle if the machine breaks down due to condition degradation
        public override bool Process(Product product)
        {
            if (Status == MachineStatus_t.Offline)
            {
                base.Process(product);
                Logger.Warning($"PackagingMachine nu poate impacheta produsul!");
                return false;
            }

            bool baseResult = base.Process(product);

            if (Status == MachineStatus_t.Broken)
            {
                Logger.Error($"The packaging machine {SerialNumber} broke down during processing! Product not completed.");
                return false;
            }

            if (baseResult)
            {
                product.TestProduct();
                Logger.Info("The packaging machine is in the process of packaging the product!");
                Logger.Info($"Its quality is: {product.Quality}");
                return true;
            }
            return false;
        }
    }

    // Machine specialized in soldering/fabricating PCBs for products
    internal class PcbFabricationMachine : Machine
    {
       
        public PcbFabricationMachine(string name, string serialNumber, MachineStatus_t status, ConditionStatus_t condition)
                    : base(name, serialNumber, status, condition) { }
        public override bool Start()
        {
            if (base.Start())
            {
                Logger.Info("PCB for soldering circuits is starting...");
                return true;
            }
            return false;
        }

        // Solders a product's PCB and re-evaluates its quality
        public override bool Process(Product product)
        {
            if (Status == MachineStatus_t.Offline)
            {
                base.Process(product);
                Logger.Warning($"PCB Fabrication Machine cannot solder PCBs for our product!");
                return false;
            }

            bool baseResult = base.Process(product);

            if (Status == MachineStatus_t.Broken)
            {
                Logger.Error($"The PCB machine {SerialNumber} broke down during processing! Product not completed.");
                return false;
            }

            if (baseResult)
            {
                product.TestProduct();
                Logger.Info("The PCB manufacturing machine is in the soldering process for our product!");
                Logger.Info($"Its quality is: {product.Quality}");
                return true;
            }
            return false;
        }
    }

  
    // Machine specialized in assembling product components together
    internal class AssemblyMachine : Machine
    {
        
        public AssemblyMachine(string name, string serialNumber, MachineStatus_t status, ConditionStatus_t condition)
                    : base(name, serialNumber, status, condition) { }
        public override bool Start()
        {
            if (base.Start())
            {
                Logger.Info("Assembling the machine components");
                return true;
            }
            return false;
        }

        // Assembles a product and re-evaluates its quality
        // Aborts mid-cycle if the machine breaks down due to condition degradation
        public override bool Process(Product product)
        {
            if (Status == MachineStatus_t.Offline)
            {
                base.Process(product);
                Logger.Warning($"Assembly machine cannot assemble the product!");
                return false;
            }

            bool baseResult = base.Process(product);

            if (Status == MachineStatus_t.Broken)
            {
                Logger.Error($"The assembly machine {SerialNumber} broke down during processing! Product not completed.");
                return false;
            }

            if (baseResult)
            {
                product.TestProduct();
                Logger.Info("The machine is in assembly processing!");
                Logger.Info($"Its quality is: {product.Quality}");
                return true;
            }
            return false;
        }
    }

 
    // Machine specialized in quality-testing finished products before storage
    internal class TestingMachine : Machine
    {
        // Initializes a new TestingMachine
        public TestingMachine(string name, string serialNumber, MachineStatus_t status, ConditionStatus_t condition)
                    : base(name, serialNumber, status, condition) { }
        public override bool Start()
        {
            if (base.Start())
            {
                Logger.Info("Testing boards.");
                return true;
            }
            return false;
        }

        // Provides testing-machine-specific diagnostic output (voltage/calibration checks)
        public override bool Inspect()
        {
            if (Condition == ConditionStatus_t.Good)
            {
                Logger.Info("All digital systems, optical sensors and calibration modules are working at 100%.");
            }
            else
            {
                Logger.Warning($"Voltage fluctuations detected. The reported condition is {Condition}.");
            }
            return base.Inspect();
        }

        // Tests a product and re-evaluates its quality
        public override bool Process(Product product)
        {
            if (Status == MachineStatus_t.Offline)
            {
                base.Process(product);
                Logger.Warning($"Testing Machine cannot test the product!");
                return false;
            }

            bool baseResult = base.Process(product);

            if (Status == MachineStatus_t.Broken)
            {
                Logger.Error($"The testing machine {SerialNumber} broke down during processing! Product not completed.");
                return false;
            }

            if (baseResult)
            {
                product.TestProduct();
                Logger.Info("The machine is undergoing product testing!");
                Logger.Info($"Its quality is: {product.Quality}");
                return true;
            }
            return false;
        }
    }
}
