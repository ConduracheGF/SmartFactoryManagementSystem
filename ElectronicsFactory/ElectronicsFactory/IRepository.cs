using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace ElectronicsFactory
{
    internal interface IRepository<T> where T: class
    {
        void Add(T genericObject);
        bool Remove(T genericObject);
        void Update(T genericObject);
        IEnumerable<T> GetAll();
        IEnumerable<T?> Find(Predicate<T> predicate);
    }

    public class GenericRepository<T>: IRepository<T> where T: class
    {
        protected readonly List<T> _storage = new List<T>();
        private readonly Action _saveCallback;

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

        public void Update(T genericObject)
        {
            var index = _storage.IndexOf(genericObject);
            if (index != -1)
            {
                _storage[index] = genericObject;
            }
            _saveCallback.Invoke();
        }

        public IEnumerable<T> GetAll() => _storage;

        public IEnumerable<T?> Find(Predicate<T> predicate) => _storage.Where(x => predicate(x));
    }
}
