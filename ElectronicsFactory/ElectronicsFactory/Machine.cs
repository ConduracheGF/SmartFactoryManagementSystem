using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ElectronicsFactory
{
    /// <summary>
    /// Operational status of a machine
    /// </summary>
    public enum MachineStatus_t
    {
        Stopped,
        Running,
        Maintenance,
        Broken
    }

    /// <summary>
    /// Physical condition of a machine, degrades over production cycles
    /// </summary>
    public enum ConditionStatus_t
    {
        Good,
        Bad,
        Critical
    }

    /// <summary>
    /// Manages the collection of machines owned by the factory: registration, lookup, and capacity/duplicate validation
    /// </summary>
    internal class MachineManagement
    {
        private Machine[] machines;
        private int machineCount = 0;

        // Underlying fixed-size storage array for machines (may contain unused trailing slots)
        public Machine[] Machines { get { return machines; } }
        public int MachineCount { get { return machineCount; } }

        // Initializes machine storage with a fixed maximum capacity.
        public MachineManagement(int maxCapacity) 
        {
            machines = new Machine[maxCapacity];
            machineCount = 0;
        }

        // Registers a new machine in the factory, rejecting it if capacity is full
        // Or if a machine with the same serial number already exists.
        public bool AddMachine(Machine machine)
        {
            if (machineCount >= machines.Length)
            {
                Logger.Error("The factory has reached the maximum limit of machines!");
                return false;
            }

            for (int i = 0; i < machineCount; i++)
            {
                if (machines[i].SerialNumber == machine.SerialNumber)
                {
                    Logger.Warning($"A machine with serial number {machine.SerialNumber} already exists!");
                    return false;
                }
            }

            machines[machineCount] = machine;
            machineCount++;

            Logger.Info($"Machine with serial number {machine.SerialNumber} has been added.");
            return true;
        }

        // Looks up a machine by its serial number
        public Machine? FindMachine(string serialNumber)
        {
            for (int i = 0; i < machineCount; i++)
            {
                if (machines[i] != null && machines[i].SerialNumber == serialNumber)
                    return machines[i];
            }
            Logger.Warning($"Machine with serial {serialNumber} not found.");
            return null;
        }
    }

    /// <summary>
    /// Abstract base class representing any production machine in the factory.
    /// Encapsulates shared behavior (start/stop/repair/inspect/process) while leaving type-specific production logic to be overridden by derived classes.
    /// </summary>
    public abstract class Machine
    {
        // Static counter shared across all Machine instances, used to auto-generate unique IDs.
        private static int nextId = 1;

        private MachineParts[] components;
        private int nrOfComponents = 0;

        // Unique, auto-generated numeric identifier assigned at construction time
        public int Id { get; private set; }

        // Business-facing unique identifier (assigned manually), used for lookups across the app
        public string SerialNumber { get; set; }

        // Current operational status of the machine (Stopped, Running, Maintenance, Broken)
        public MachineStatus_t Status { get; set; }

        // Current physical condition of the machine; degrades with use and resets on repair
        public ConditionStatus_t Condition { get; set; }

        // The set of replaceable parts currently installed in this machine
        public MachineParts[] Components { get { return components; } set { components = value; } }

        // Initializes a new machine with a default set of 5 standard components
        public Machine(string serialNumber, int maxCapacity)
        {
            Id = nextId++;
            SerialNumber = serialNumber;
            Status = MachineStatus_t.Stopped;
            Condition = ConditionStatus_t.Good;

            // Ensure the array can always hold the 5 default components, regardless of requested capacity
            int size = Math.Max(maxCapacity, 5);
            components = new MachineParts[size];
            
            components[0] = new Motor(500, "Bosch", 1, ComponentsType_t.Motor, 200, 5);
            components[1] = new Senzor(120, "Samsung", 1, ComponentsType_t.Senzor, 0.5f, 500);
            components[2] = new Controler(350, "Intel", 1, ComponentsType_t.Controler, 3200);
            components[3] = new Display(600, "LG", 1, ComponentsType_t.Display, 4000f);
            components[4] = new CoolingFan(80, "Noctua", 1, ComponentsType_t.CoolingFan, 2500);
        
            nrOfComponents = 5;
        }

        // Stops the machine, unless it is currently under maintenance
        public bool Stop()
        {
            if (Status == MachineStatus_t.Maintenance)
            {
                Logger.Warning($"Machine {SerialNumber} is undergoing maintenance and cannot be stopped!"); 
                return false;
            }

            Status = MachineStatus_t.Stopped;
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
            return true;
        }

        // Repairs the machine by replacing one randomly-selected component
        // Restoring its condition to Good and its status to Stopped
        // Cannot be performed while running
        public virtual bool Repair(ref float income)
        {
            if (Status == MachineStatus_t.Running)
            {
                Logger.Warning($"Cannot perform repair on machine {SerialNumber} while it is running");
                return false;
            }
            Logger.Info("Tehnician has found a piece of machine which it has a bad functionality!");

            
            if (components == null || nrOfComponents == 0)
            {
                Logger.Error("This machine does not have any components configured to be repaired!");
                return false;
            }


            int indexRandom = Random.Shared.Next(nrOfComponents);
     
            income = components[indexRandom].Replacement(income);

            ComponentsType_t swapComponents = (ComponentsType_t)indexRandom;
            Logger.Warning($"The component with bad behavior is {swapComponents} and it will be replaced!");

            Condition = ConditionStatus_t.Good;
            Status = MachineStatus_t.Stopped;
            return true;
        }

        // Processes a single unit of the given product
        // Base implementation handles status validation and condition degradation
        // Derived classes extend this with machine-type-specific production behavior
        public virtual bool Process(Product product)
        {
            switch(Status)
            {
                case MachineStatus_t.Stopped:
                    Logger.Warning($"The machine with serial number {SerialNumber} is stopped and cannot process products!"); 
                    return false;
                case MachineStatus_t.Running:
                    if (nrOfComponents == 0)
                        Logger.Info("The machine is processing products");
                    DegradeCondition();
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
        // The machine as Broken once it reaches Critical.
        private void DegradeCondition()
        {
            if (Random.Shared.NextDouble() < 0.20)
            {
                var previous = Condition;
                Condition = (Condition == ConditionStatus_t.Good ? ConditionStatus_t.Bad : ConditionStatus_t.Critical);
                Logger.Warning($"Machine {SerialNumber} condition degraded to {Condition} after processing!");

                if (Condition == ConditionStatus_t.Critical)
                    Status = MachineStatus_t.Broken;
            }
        }
    }

    /// <summary>
    /// Machine specialized in packaging finished products into boxes
    /// </summary>
    internal class PackagingMachine : Machine
    {
        // Initializes a new PackagingMachine
        public PackagingMachine(string serialNumber, int maxCapacity) : base(serialNumber, maxCapacity) { }

        public override bool Start()
        {
            if (base.Start())
            {
                Logger.Info("The packaging machine is packaging the product boxes.");
                return true;
            }
            return false;
        }

        /// Packages a product unit and re-evaluates its quality
        /// Aborts mid-cycle if the machine breaks down due to condition degradation
        public override bool Process(Product product)
        {
            switch (Status)
            {
                case MachineStatus_t.Stopped:
                    base.Process(product);
                    Logger.Warning($"PackagingMachine nu poate impacheta produsul!");
                    return false;
                case MachineStatus_t.Running:
                    base.Process(product);

                    if (Status == MachineStatus_t.Broken)
                    {
                        Logger.Error($"The packaging machine {SerialNumber} broke down during processing! Product not completed.");
                        return false;
                    }

                    product.TestProduct(); 
                    Logger.Info("The packaging machine is in the process of packaging the product!");
                    Logger.Info($"Its quality is: {product.Quality}");
                    return true;
                case MachineStatus_t.Maintenance:
                    base.Process(product);
                    Logger.Warning($"The packaging machine is under maintenance and cannot process products"); 
                    return false;
                case MachineStatus_t.Broken:
                    base.Process(product);
                    Logger.Error($"The packaging machine is broken and cannot process products!");
                    return false;
                default: return false;
            }
        }
    }

    /// <summary>
    /// Machine specialized in soldering/fabricating PCBs for products
    /// </summary>
    internal class PcbFabricationMachine : Machine
    {
        // Initializes a new PcbFabricationMachine
        public PcbFabricationMachine(string serialNumber, int maxCapacity) : base(serialNumber, maxCapacity) { }

        public override bool Start()
        {
            if (base.Start())
            {
                Logger.Info("PCB for soldering circuits is starting...");
                return true;
            }
            return false;
        }

        /// Solders a product's PCB and re-evaluates its quality
        /// Aborts mid-cycle if the machine breaks down due to condition degradation
        public override bool Process(Product product)
        {
            switch (Status)
            {
                case MachineStatus_t.Stopped:
                    base.Process(product);
                    Logger.Warning($"PCB Fabrication Machine cannot solder PCBs for our product!");
                    return false;
                case MachineStatus_t.Running:
                    base.Process(product);

                    if (Status == MachineStatus_t.Broken)
                    {
                        Logger.Error($"The PCB machine {SerialNumber} broke down during processing! Product not completed.");
                        return false;
                    }

                    product.TestProduct(); 
                    Logger.Info("The PCB manufacturing machine is in the soldering process for our product!");
                    Logger.Info($"Its quality is: {product.Quality}");
                    return true;
                case MachineStatus_t.Maintenance:
                    base.Process(product);
                    Logger.Warning($"The PCB machine with serial number {SerialNumber} is under maintenance and cannot process tasks!"); return false;
                case MachineStatus_t.Broken:
                    base.Process(product);
                    Logger.Error($"The machine is broken and cannot process commands!");
                    return false;
                default: return false;
            }
        }
    }

    /// <summary>
    /// Machine specialized in assembling product components together
    /// </summary>
    internal class AssemblyMachine : Machine
    {
        // Initializes a new AssemblyMachine
        public AssemblyMachine(string serialNumber, int maxCapacity) : base(serialNumber, maxCapacity) { }

        public override bool Start()
        {
            if (base.Start())
            {
                Logger.Info("Assembling the machine components"); 
                return true;
            }
            return false;
        }

        /// Assembles a product and re-evaluates its quality
        /// Aborts mid-cycle if the machine breaks down due to condition degradation
        public override bool Process(Product product)
        {
            switch (Status)
            {
                case MachineStatus_t.Stopped:
                    base.Process(product);
                    Logger.Warning($"Assembly machine cannot assemble the product!");
                    return false;
                case MachineStatus_t.Running:
                    base.Process(product);

                    if (Status == MachineStatus_t.Broken)
                    {
                        Logger.Error($"The assembly machine {SerialNumber} broke down during processing! Product not completed.");
                        return false;
                    }

                    product.TestProduct(); 
                    Logger.Info("The machine is in assembly processing!");
                    Logger.Info($"Its quality is: {product.Quality}");
                    return true;
                case MachineStatus_t.Maintenance:
                    base.Process(product);
                    Logger.Warning($"Assembly machine {SerialNumber} is under maintenance and cannot process products");
                    return false;
                case MachineStatus_t.Broken:
                    base.Process(product);
                    Logger.Error($"The machine is broken and cannot assemble products!");
                    return false;
                default: return false;
            }
        }
    }

    internal class TestingMachine : Machine
    {
        public TestingMachine(string serialNumber, int maxCapacity) : base(serialNumber, maxCapacity) { }

        public override bool Start()
        {
            if (base.Start())
            {
                Logger.Info("Testing boards.");
                return true;
            }
            return false;
        }


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
            return (Condition == ConditionStatus_t.Bad || Condition == ConditionStatus_t.Critical);
        }

        public override bool Process(Product product)
        {
            switch (Status)
            {
                case MachineStatus_t.Stopped:
                    base.Process(product);
                    Logger.Warning($"Testing Machine cannot test the product!");
                    return false;
                case MachineStatus_t.Running:
                    base.Process(product);

                    if (Status == MachineStatus_t.Broken)
                    {
                        Logger.Error($"The testing machine {SerialNumber} broke down during processing! Product not completed.");
                        return false;
                    }

                    product.TestProduct(); 
                    Logger.Info("The machine is undergoing product testing!");
                    Logger.Info($"Its quality is: {product.Quality}");
                    return true;
                case MachineStatus_t.Maintenance:
                    base.Process(product);
                    Logger.Warning($"Test machine {SerialNumber} is under maintenance and cannot process products"); 
                    return false;
                case MachineStatus_t.Broken:
                    base.Process(product);
                    Logger.Error($"The machine is broken and cannot test the product!");
                    return false;
                default: return false;
            }
        }
    }
}
