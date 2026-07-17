using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsFactory
{
    public class FactoryException: Exception
    {
        public FactoryException(string message): base(message) { }
        public FactoryException(string message, Exception innerException): base(message, innerException) { }
    }

    public class DataPersistenceException: FactoryException
    {
        public DataPersistenceException(string message): base(message) { }
        public DataPersistenceException(string message, Exception innerException): base(message, innerException) { }
    }

    public class AuthenticationException: FactoryException
    {
        public AuthenticationException(string message): base(message) { }
    }

    public class InsufficientFundException: FactoryException
    {
        public InsufficientFundException(string  message): base(message) { }
    }

    public class InvalidMachineStateException: FactoryException
    {
        public InvalidMachineStateException(string message): base(message) { }
    }

    public class ProductNotFoundException: FactoryException
    {
        public ProductNotFoundException(string message): base(message) { }
    }
}
