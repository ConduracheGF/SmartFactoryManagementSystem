using System;
using System.Linq;

namespace ElectronicsFactory
{
    internal class AuthService
    {
        private readonly EmployeeManagement _employeeManager;
        private readonly FileStorageService _fileStorage;
        private const string OperationsFile = "operations.txt";

        public Employee? CurrentUser { get; private set; }

        public AuthService(EmployeeManagement employeeManager, FileStorageService fileStorage)
        {
            _employeeManager = employeeManager;
            _fileStorage = fileStorage;
            CurrentUser = null;
        }

        public bool Login(string username, string password)
        {
            var user = _employeeManager.Employees.FirstOrDefault(e =>
                e.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && e.Password == password);

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (user != null)
            {
                CurrentUser = user;
                _fileStorage.Append(OperationsFile, $"{timestamp} | {username} | Successful login");
                Logger.Info($"Autentificare reusita! Bun venit, {user.Name} ({user.JobStatus}).");
                return true;
            }

            _fileStorage.Append(OperationsFile, $"{timestamp} | {username} | Failed login attempt");
            Logger.Error("Credentiale invalide! Incercare esuata de logare.");
            return false;
        }

        public void Logout()
        {
            if (CurrentUser != null)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                _fileStorage.Append(OperationsFile, $"{timestamp} | {CurrentUser.Username} | Logged out");
                Logger.Info($"La revedere, {CurrentUser.Name}!");
                CurrentUser = null;
            }
        }

        public bool IsDirector => CurrentUser is Director;
        public bool IsTechnician => CurrentUser is Technician;
        public bool IsEngineer => CurrentUser is Engineer;
        public bool IsSalesAgent => CurrentUser is SalesAgent;
        public bool IsAccountant => CurrentUser is Accountant;
        public bool IsProductionManager => CurrentUser is ProductionManager;
    }
}