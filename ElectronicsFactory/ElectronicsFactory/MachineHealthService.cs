using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsFactory
{

    // Monitors and reports the health, performance, and maintenance needs of all machines in the factory.
    internal class MachineHealthService: IMachineHealthService
    {
        private readonly MachineManagement _machineManager;

        public MachineHealthService(MachineManagement machineManager)
        {
            _machineManager = machineManager;
        }

        public void DisplayEfficencyDashboard()
        {
            Logger.Info("PRODUCTION EFFICIENCY DASHBOARD");
            foreach (var machine in _machineManager.Machines)
            {
                float efficiency = (machine.TotalCyclesAttempted == 0) ? 100f : ((float)machine.SuccessfulCycles / machine.TotalCyclesAttempted) * 100;

                Logger.Info($"Machine [{machine.SerialNumber}] {machine.Name} -> Efficiency: {efficiency:F2}%");
                Logger.Info($"Cycles: {machine.SuccessfulCycles}/{machine.TotalCyclesAttempted}");
            }
        }

        // Estimates how many runs each machine has left before it breaks down and warns us if we need to plan repairs.
        public void RunPredictiveMaintenanceReport()
        {
            Logger.Info("PREDICTIVE MAINTENANCE REPORT");
            foreach (var machine in _machineManager.Machines)
            {
                if (machine.Status == MachineStatus_t.Broken)
                {
                    Logger.Info($"Machine {machine.SerialNumber} requires urgent intervation: Status is BROKEN!");
                    continue;
                }

                float remainingWear = 100f - machine.WearLevel;
                int estimatedCyclesLeft = (int)(remainingWear / 2.5f);

                if (estimatedCyclesLeft <= 3)
                {
                    Logger.Error($"Machine {machine.SerialNumber} will fail in approximately {estimatedCyclesLeft} cycles! Plan immediate maintenance.");
                }
                else if (estimatedCyclesLeft <= 8)
                {
                    Logger.Warning($"Machine {machine.SerialNumber} is degrading rapidly. Est. cycles left: {estimatedCyclesLeft}.");
                }
                else
                {
                    Logger.Info($"Machine {machine.SerialNumber}: Healthy state. Est. cycles left: {estimatedCyclesLeft}.");
                }
            }
        }

        // Scans all machines and triggers alerts if any machine is broken, in critical condition, or wearing out too fast.
        public void CheckMachineAlerts()
        {
            Logger.Info("MONITORING SYSTEM: MACHINE ALERTS");
            int alertCount = 0;

            foreach (var machine in _machineManager.Machines)
            {
                if (machine.Condition == ConditionStatus_t.Critical || machine.Status == MachineStatus_t.Broken)
                {
                    Logger.Error($"ALERT: Machine {machine.Name} - {machine.SerialNumber} is in CRITICAL/BROKEN status!");
                    alertCount++;
                }
                else if (machine.Condition == ConditionStatus_t.Bad || machine.WearLevel > 60f)
                {
                    Logger.Warning($"ALERT: Machine {machine.Name} - {machine.SerialNumber} reports high wear profile - {machine.WearLevel}%");
                    alertCount++;
                }
            }

            if (alertCount == 0)
            {
                Logger.Info("All systems normal. No machine health alerts triggered.");
            }    
        }
    }
}
