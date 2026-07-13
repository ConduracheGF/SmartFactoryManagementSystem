using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsFactory
{
    /// <summary>
    /// Manages the collection of machines owned by the factory: registration, lookup, and capacity/duplicate validation
    /// </summary>
    internal class MachineManagement : GenericRepository<Machine>
    {
        public int MachineCount => _storage.Count;
        public List<Machine> Storage => _storage;
        public List<Machine> Machines => _storage;

        // Registers a new machine in the factory, rejecting it if capacity is full
        // Or if a machine with the same serial number already exists.
        public bool AddMachine(Machine machine)
        {
            if (machine == null) return false;

            if (_storage.Any(m => m.SerialNumber == machine.SerialNumber))
            {
                Logger.Warning($"A machine with serial number {machine.SerialNumber} already exists!");
                return false;
            }

            Add(machine);
            Logger.Info($"Machine with serial number {machine.SerialNumber} has been added.");
            return true;
        }

        // Looks up a machine by its serial number
        public Machine? FindMachine(string serialNumber)
        {
            var machine = _storage.Find(m => m.SerialNumber == serialNumber);

            if (machine == null)
            {
                Logger.Warning($"Machine with serial {serialNumber} not found.");
            }
            return machine;
        }

        // Searches for a machine by ID and triggers an active inspection as a side effect.
        public int Search(int id)
        {
            var machine = _storage.Find(m => m.Id == id);

            if (machine != null)
            {
                machine.Inspect();
                Logger.Info($"The machine {id} ({machine.Name}) was found!");
                return _storage.IndexOf(machine);
            }

            Logger.Warning($"Machine {id} was not found.");
            return -1;
        }
    }
}
