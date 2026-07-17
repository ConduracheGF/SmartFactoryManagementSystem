using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace ElectronicsFactory
{
    // A simple blueprint for managing a collection of items (like adding, removing, or finding them).
    internal interface IRepository<T> where T: class
    {
        void Add(T genericObject);
        bool Remove(T genericObject);
        void Update(T genericObject);
        IEnumerable<T> GetAll();
        IEnumerable<T?> Find(Predicate<T> predicate);
    }

    // A general list that stores items in memory and automatically saves changes to files.
    public class GenericRepository<T>: IRepository<T> where T: class
    {
        protected readonly List<T> _storage = new List<T>();
        private readonly Action _saveCallback;

        // A general list that stores items in memory and automatically runs a save function whenever things change.
        public GenericRepository(IEnumerable<T> initialData, Action saveCallback)
        {
            if (initialData != null)
            {
                _storage.AddRange(initialData);
            }
            _saveCallback = saveCallback;
        }

        public void Add(T genericObject)
        {
            _storage.Add(genericObject);
            _saveCallback?.Invoke();
        }

        public bool Remove(T genericObject)
        {
            bool flag = _storage.Remove(genericObject);
            _saveCallback?.Invoke();
            return flag;
        }

        // Finds an existing item in the list, replaces it with the updated version, and saves the changes.
        public void Update(T genericObject)
        {
            var index = _storage.IndexOf(genericObject);
            if (index != -1)
            {
                _storage[index] = genericObject;
            }
            _saveCallback.Invoke();
        }

        // Returns all the items currently stored in our list.
        public IEnumerable<T> GetAll() => _storage;

        // Searches the list and returns only the items that match a specific condition (like searching by ID or name).
        public IEnumerable<T?> Find(Predicate<T> predicate) => _storage.Where(x => predicate(x));
    }
}
