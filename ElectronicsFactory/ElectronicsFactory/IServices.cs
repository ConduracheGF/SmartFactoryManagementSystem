using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsFactory
{
    internal interface IAuthService
    {
        Employee? CurrentUser { get; }
        bool Login(string username, string password);
        void Logout();
    }

    internal interface IMachineHealthService
    {
        void DisplayEfficencyDashboard();
        void RunPredictiveMaintenanceReport();
        void CheckMachineAlerts();
    }

    internal interface IInventoryDashboardService
    {
        void VerifyStockThresholds(int minimumAllowedUnits);
        void DisplayAdvancedReports();
    }

    internal interface IUndoService
    {
        void RegisterAction(IReversibleAction action);
        void UndoLastOperation();
        string GetLastActionDescription();
        int GetHistoryCount();
    }
}
