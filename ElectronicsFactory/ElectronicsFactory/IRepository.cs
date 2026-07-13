using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsFactory
{
    internal interface IRepository<T> where T: class
    {
        void Add(T genericObject);
        bool Remove(T genericObject);
        IEnumerable<T> GetAll();
        T? Find(Predicate<T> predicate);
    }

    public class GenericRepository<T>: IRepository<T> where T: class
    {
        protected readonly List<T> _storage = new List<T>();

        public void Add(T genericObject)
        {
            _storage.Add(genericObject);
        }

        public bool Remove(T genericObject)
        {
            return _storage.Remove(genericObject);
        }

        public IEnumerable<T> GetAll()
        {
            return _storage;
        }

        public T? Find(Predicate<T> predicate)
        {
            return _storage.Find(predicate);
        }
    }
}
