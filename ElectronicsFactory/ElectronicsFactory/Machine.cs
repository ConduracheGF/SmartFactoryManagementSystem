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

        public MachineManagement(int maxCapacity) 
        {
            machines = new Machine[maxCapacity];
            machineCount = 0;
        }
        public void AddMachine(Machine machine)
        {
            if (machineCount >= machines.Length)
            {
                Console.WriteLine("Fabrica a atins limita maximă de mașini!");
                return;
            }

            for (int i = 0; i < machineCount; i++)
            {
                if (machines[i].SerialNumber == machine.SerialNumber)
                {
                    Console.WriteLine($"O mașină cu seria {machine.SerialNumber} există deja!");
                    return;
                }
            }

            machines[machineCount] = machine;
            machineCount++;

            Console.WriteLine($"Mașina cu seria {machine.SerialNumber} a fost adăugată.");
        }

        public void DisplayMachinesStatus( string serialNumber )
        {
            if (machineCount == 0)
            {
                Console.WriteLine("Nu există mașini în fabrică.");
                return;
            }

            for (int i = 0; i < machineCount; i++)
            {
                if (machines[i].SerialNumber == serialNumber)
                {
                    machines[i].Start();
                    return;
                }
            }
            Console.WriteLine($"Mașina cu seria {serialNumber} nu a fost găsită.");
        }
    }
    abstract public class Machine
    {
        private MachineParts[] components;
        private int nrOfComponents = 0;
        public string SerialNumber { get; set; }
        public MachineStatus_t Status { get; set; }
        public ConditionStatus_t Condition { get; set; }

        public Machine(string serialNumber, int maxCapacity)
        {
            SerialNumber = serialNumber;
            Status = MachineStatus_t.Stopped;
            Condition = ConditionStatus_t.Good;
            components = new MachineParts[maxCapacity];
            nrOfComponents = 0;
        }

        public void Stop()
        {
            Status = MachineStatus_t.Stopped;
            Console.WriteLine($"Masina cu serialNumber {SerialNumber} a fost oprita.");
        }

        public virtual bool Inspect()
        {
            return (Condition == ConditionStatus_t.Bad || Condition == ConditionStatus_t.Critical);
        }

        public virtual void Start()
        {
            if (Condition == ConditionStatus_t.Critical || Condition == ConditionStatus_t.Bad)
            {
                Status = MachineStatus_t.Broken;
                return;
            }

            Status = MachineStatus_t.Running;
        }

        public virtual void Repair()
        {
            if (Status == MachineStatus_t.Running)
            {
                Console.WriteLine($"Nu se poate efectua reparatia masinii {SerialNumber} in timp ce este in functiune");
                return;
            }

            Status = MachineStatus_t.Maintenance;
           
            Condition = ConditionStatus_t.Good; 
            Status = MachineStatus_t.Stopped;
        }
    }

    internal class PackagingMachine : Machine
    {
        public PackagingMachine(string serialNumber, int maxCapacity) : base(serialNumber, maxCapacity) { }

        public override void Start()
        {
            base.Start();
            if (Status == MachineStatus_t.Running)
            {
                Console.WriteLine("Masina de ambalare impacheteaza cutiile cu produse.");
            }
        }
    }

    internal class PcbFabricationMachine : Machine
    {
        public PcbFabricationMachine(string serialNumber, int maxCapacity) : base(serialNumber, maxCapacity) { }

        public override void Start()
        {
            base.Start();
            if (Status == MachineStatus_t.Running)
            {
                Console.WriteLine("PCB pt lipirea circuitelor");
            }
        }
    }

    internal class AssemblyMachine : Machine
    {
        public AssemblyMachine(string serialNumber, int maxCapacity) : base(serialNumber, maxCapacity) { }

        public override void Start()
        {
            base.Start();
            if (Status == MachineStatus_t.Running)
            {
                Console.WriteLine("Se monteaza componentele masinii");
            }
        }
    }

    internal class TestingMachine : Machine
    {
        public TestingMachine(string serialNumber, int maxCapacity) : base(serialNumber, maxCapacity) { }

        public override void Start()
        {
            base.Start();
            if (Status == MachineStatus_t.Running)
            {
                Console.WriteLine("Testare placi.");
            }
        }


        public override bool Inspect()
        {
            if (Condition == ConditionStatus_t.Good)
            {
                Console.WriteLine("Toate sistemele digitale, senzorii optici și modulele de calibrare funcționează la 100%.");
            }
            else
            {
                Console.WriteLine($"S-au detectat fluctuații de tensiune. Condiția raportată este {Condition}.");
            }
            return (Condition == ConditionStatus_t.Bad || Condition == ConditionStatus_t.Critical);
        }
    }
}
