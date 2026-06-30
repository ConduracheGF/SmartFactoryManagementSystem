using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ElectronicsFactory
{
    public enum MachineStatus
    {
        Stopped,
        Running,
        Maintenance,
        Broken
    }

    internal class MachineManagement
    {
        private Machine[] machines;
        private int machineCount = 0;

        public MachineManagement(int maxCapacity) 
        {
            this.machines = new Machine[maxCapacity];
            this.machineCount = 0;
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
        public string SerialNumber { get; set; }
        public MachineStatus Status { get; set; }
        public string Condition { get; set; } 

        public Machine(string serialNumber)
        {
            SerialNumber = serialNumber;
            Status = MachineStatus.Stopped;
            Condition = "Buna";
        }

        public void Stop()
        {
            Status = MachineStatus.Stopped;
            Console.WriteLine($"Masina cu serialNumber {SerialNumber} a fost oprita.");
        }

        public virtual bool Inspect()
        {
            return (Condition == "Rea" || Condition == "Critica");
        }

        public virtual void Start()
        {
            if (Condition == "Critica" || Condition == "Rea")
            {
                Status = MachineStatus.Broken;
                return;
            }

            Status = MachineStatus.Running;
        }

        public virtual void Repair()
        {
            if (Status == MachineStatus.Running)
            {
                Console.WriteLine($"Nu se poate efectua reparatia masinii {SerialNumber} in timp ce este in functiune");
                return;
            }

            Status = MachineStatus.Maintenance;
           
            Condition = "Buna"; 
            Status = MachineStatus.Stopped;
        }
    }

    internal class PackagingMachine : Machine
    {
        public PackagingMachine(string serialNumber) : base(serialNumber) { }

        public override void Start()
        {
            base.Start();
            if (Status == MachineStatus.Running)
            {
                Console.WriteLine("Masina de ambalare impacheteaza cutiile cu produse.");
            }
        }
    }

    internal class PcbFabricationMachine : Machine
    {
        public PcbFabricationMachine(string serialNumber) : base(serialNumber) { }

        public override void Start()
        {
            base.Start();
            if (Status == MachineStatus.Running)
            {
                Console.WriteLine("PCB pt lipirea circuitelor");
            }
        }
    }

    internal class AssemblyMachine : Machine
    {
        public AssemblyMachine(string serialNumber) : base(serialNumber) { }

        public override void Start()
        {
            base.Start();
            if (Status == MachineStatus.Running)
            {
                Console.WriteLine("Se monteaza componentele masinii");
            }
        }
    }

    internal class TestingMachine : Machine
    {
        public TestingMachine(string serialNumber) : base(serialNumber) { }

        public override void Start()
        {
            base.Start();
            if (Status == MachineStatus.Running)
            {
                Console.WriteLine("Testare placi.");
            }
        }


        public override bool Inspect()
        {
            if (Condition == "Buna")
            {
                Console.WriteLine("Toate sistemele digitale, senzorii optici și modulele de calibrare funcționează la 100%.");
            }
            else
            {
                Console.WriteLine($"S-au detectat fluctuații de tensiune. Condiția raportată este {Condition}.");
            }
            return (Condition == "Rea" || Condition == "Critica");
        }
    }
}
