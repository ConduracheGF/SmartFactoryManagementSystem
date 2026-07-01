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
        //de facut pe mai tarziu o metoda de vandut Masinaria in caz de nu ne mai trebuie sau e inutila sau e defecta de tot (suplimentar)
        //de facut o metoda de cautat masina in registru fabricii (suplimentar)
        private Machine[] machines;
        private int machineCount = 0;

        public MachineManagement(int maxCapacity) 
        {
            machines = new Machine[maxCapacity];
            machineCount = 0;
        }
        public bool AddMachine(Machine machine)
        {
            if (machineCount >= machines.Length)
            {
                Logger.Error("Fabrica a atins limita maximă de mașini!");
                return false;
            }

            for (int i = 0; i < machineCount; i++)
            {
                if (machines[i].SerialNumber == machine.SerialNumber)
                {
                    Logger.Warning($"O mașină cu seria {machine.SerialNumber} există deja!");
                    return false;
                }
            }

            machines[machineCount] = machine;
            machineCount++;

            Logger.Info($"Mașina cu seria {machine.SerialNumber} a fost adăugată.");
            return true;
        }

        public void DisplayMachinesStatus( string serialNumber )
        {
            if (machineCount == 0)
            {
                Logger.Warning("Nu există mașini în fabrică.");
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
            Logger.Info($"Mașina cu seria {serialNumber} nu a fost găsită.");
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

        public bool Stop()
        {
            if (Status == MachineStatus_t.Maintenance)
            {
                Logger.Warning($"Masina {SerialNumber} este in mentenanta si nu poate fi oprita!");
                return false;
            }

            Status = MachineStatus_t.Stopped;
            Logger.Info($"Masina cu serial number: {SerialNumber} a fost oprita.");
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
                Logger.Warning($"Masina cu serial number: {SerialNumber} este defecta si nu poate porni!");
                Status = MachineStatus_t.Broken;
                return false;
            }

            Logger.Info($"Masina cu serial number {SerialNumber} a pornit cu succes!");
            Status = MachineStatus_t.Running;
            return true;
        }

        public virtual bool Repair()
        {
            if (Status == MachineStatus_t.Running)
            {
                Logger.Warning($"Nu se poate efectua reparatia masinii {SerialNumber} in timp ce este in functiune");
                return false;
            }

            Status = MachineStatus_t.Maintenance;
            //trebuie facut ceva care sa lege cu mentenanta si tehnicianul
            Condition = ConditionStatus_t.Good; 
            Status = MachineStatus_t.Stopped;
            return true;
        }

        public virtual bool Process(Product product)
        {
            switch(Status)
            {
                case MachineStatus_t.Stopped:
                    Logger.Warning($"Masina cu serial number {SerialNumber} este oprita si nu poate procesa produse!");
                    return false;
                case MachineStatus_t.Running:
                    Logger.Info("Masina este in procesarea produselor");
                    return true;
                case MachineStatus_t.Maintenance:
                    Logger.Warning($"Masina cu serial number {SerialNumber} este in mentenanta si nu poate procesa produse");
                    return false;
                case MachineStatus_t.Broken:
                    Logger.Error($"Masina este stricata si nu poate procesa produse!");
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
                Logger.Info("Masina de ambalare impacheteaza cutiile cu produse.");
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
                    Logger.Info("Masina este in procesarea produsului!");
                    Logger.Info($"Calitatea sa este: {product.Quality}");
                    return true;
                case MachineStatus_t.Maintenance:
                    base.Process(product);
                    Logger.Warning($"Masina cu serial number {SerialNumber} este in mentenanta si nu poate procesa produse");
                    return false;
                case MachineStatus_t.Broken:
                    base.Process(product);
                    Logger.Error($"Masina este stricata si nu poate procesa produse!");
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
                Logger.Info("PCB pt lipirea circuitelor");
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
                    Logger.Info("Masina este in procesarea produsului!");
                    Logger.Info($"Calitatea sa este: {product.Quality}");
                    return true;
                case MachineStatus_t.Maintenance:
                    base.Process(product);
                    Logger.Warning($"Masina cu serial number {SerialNumber} este in mentenanta si nu poate procesa produse");
                    return false;
                case MachineStatus_t.Broken:
                    base.Process(product);
                    Logger.Error($"Masina este stricata si nu poate procesa produse!");
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
                Logger.Info("Se monteaza componentele masinii");
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
                    Logger.Info("Masina este in procesarea produsului!");
                    Logger.Info($"Calitatea sa este: {product.Quality}");
                    return true;
                case MachineStatus_t.Maintenance:
                    base.Process(product);
                    Logger.Warning($"Masina cu serial number {SerialNumber} este in mentenanta si nu poate procesa produse");
                    return false;
                case MachineStatus_t.Broken:
                    base.Process(product);
                    Logger.Error($"Masina este stricata si nu poate procesa produse!");
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
                Logger.Info("Testare placi.");
                return true;
            }
            return false;
        }


        public override bool Inspect()
        {
            if (Condition == ConditionStatus_t.Good)
            {
                Logger.Info("Toate sistemele digitale, senzorii optici și modulele de calibrare funcționează la 100%.");
            }
            else
            {
                Logger.Warning($"S-au detectat fluctuații de tensiune. Condiția raportată este {Condition}.");
            }
            return (Condition == ConditionStatus_t.Bad || Condition == ConditionStatus_t.Critical);
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
                    Logger.Info("Masina este in procesarea produsului!");
                    Logger.Info($"Calitatea sa este: {product.Quality}");
                    return true;
                case MachineStatus_t.Maintenance:
                    base.Process(product);
                    Logger.Warning($"Masina cu serial number {SerialNumber} este in mentenanta si nu poate procesa produse");
                    return false;
                case MachineStatus_t.Broken:
                    base.Process(product);
                    Logger.Error($"Masina este stricata si nu poate procesa produse!");
                    return false;
                default: return false;
            }
        }
    }
}
