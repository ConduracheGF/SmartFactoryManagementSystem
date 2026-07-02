using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ElectronicsFactory
{
    public enum MachineStatus_t
    {
        Stopped,
        Running,
        Maintenance,
        Broken
    }

    public enum ConditionStatus_t
    {
        Good,
        Bad,
        Critical
    }

    internal class MachineManagement
    {
        private Machine[] machines;
        private int machineCount = 0;

        public Machine[] Machines { get { return machines; } set { machines = value; } }

        public MachineManagement(int maxCapacity) 
        {
            machines = new Machine[maxCapacity];
            machineCount = 0;
        }
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

        public void DisplayMachinesStatus( string serialNumber )
        {
            if (machineCount == 0)
            {
                Logger.Warning("There are no machines in the factory.");
                return;
            }

            for (int i = 0; i < machineCount; i++)
            {
                if (machines[i].SerialNumber == serialNumber)
                {
                    machines[i].Inspect();
                    return;
                }
            }
            Logger.Info($"Machine with serial number {serialNumber} not found.");
        }
    }
    abstract public class Machine
    {
        private MachineParts[] components;
        private int nrOfComponents = 0;
        public string SerialNumber { get; set; }
        public MachineStatus_t Status { get; set; }
        public ConditionStatus_t Condition { get; set; }
        public MachineParts[] Components { get { return components; } set { components = value; } }
        public Machine(string serialNumber, int maxCapacity)
        {
            SerialNumber = serialNumber;
            Status = MachineStatus_t.Stopped;
            Condition = ConditionStatus_t.Good;
            components = new MachineParts[maxCapacity];
            nrOfComponents = 0;
        }

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

        public virtual bool Inspect()
        {
            return (Condition == ConditionStatus_t.Bad || Condition == ConditionStatus_t.Critical);
        }

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

        public virtual bool Repair(ref float income)
        {
            if (Status == MachineStatus_t.Running)
            {
                Logger.Warning($"Cannot perform repair on machine {SerialNumber} while it is running");
                return false;
            }

            Logger.Info("Tehnician has found a piece of machine which it has a bad functionality!");
            int indexRandom = Random.Shared.Next(components.Length);
            income = components[indexRandom].Replacement(income);
            ComponentsType_t swapComponents = (ComponentsType_t)indexRandom;
            Logger.Warning($"The component with bad behavior is {swapComponents} and it will be replaced!");
            Condition = ConditionStatus_t.Good;
            Status = MachineStatus_t.Stopped;
            return true;
        }

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
    }

    internal class PackagingMachine : Machine
    {
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

    internal class PcbFabricationMachine : Machine
    {
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

    internal class AssemblyMachine : Machine
    {
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
