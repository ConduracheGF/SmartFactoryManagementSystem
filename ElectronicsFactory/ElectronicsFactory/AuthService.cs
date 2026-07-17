using System;
using System.Linq;

namespace ElectronicsFactory
{
    // Manages user authentication (login, logout) and tracks who is currently active in the system.
    internal class AuthService: IAuthService
    {
        private readonly EmployeeManagement _employeeManager;
        private readonly FileStorageService _fileStorage;
        private readonly string _operationsFilename; 

        public Employee? CurrentUser { get; private set; }

       
        public AuthService(EmployeeManagement employeeManager, FileStorageService fileStorage, string operationsFilename)
        {
            _employeeManager = employeeManager;
            _fileStorage = fileStorage;
            _operationsFilename = operationsFilename; 
            CurrentUser = null;
        }

        public bool Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new AuthenticationException("Username and password fields cannot be empty.");
            }

            var user = _employeeManager.Employees.FirstOrDefault(e =>
                e.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && e.Password == password);

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (user != null)
            {
                CurrentUser = user;
                _fileStorage.Append(_operationsFilename, $"{timestamp} | {username} | Successful login"); 
                Logger.Info($"Successful login, welcome {user.Name} ({user.JobStatus}).");
                return true;
            }

            _fileStorage.Append(_operationsFilename, $"{timestamp} | {username} | Failed login attempt"); 
            Logger.Error("Failed login attempt. Wrong credentials");
            return false;
        }

        public void Logout()
        {
            if (CurrentUser != null)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                _fileStorage.Append(_operationsFilename, $"{timestamp} | {CurrentUser.Username} | Logged out"); 
                Logger.Info($"Goodbye, {CurrentUser.Name}!");
                CurrentUser = null;
            }
        }

        // Quick helper checks to easily verify the active user's role across the app
        public bool IsDirector => CurrentUser is Director;
        public bool IsTechnician => CurrentUser is Technician;
        public bool IsEngineer => CurrentUser is Engineer;
        public bool IsSalesAgent => CurrentUser is SalesAgent;
        public bool IsAccountant => CurrentUser is Accountant;
        public bool IsProductionManager => CurrentUser is ProductionManager;
    }
}