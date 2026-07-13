using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsFactory
{
    /// <summary>
    /// Manages the factory's product inventory using a dynamic generic repository pattern.
    /// Provides LINQ searching, inventory threshold alerts, and file-based data persistence.
    /// </summary>
    internal class ProductManagement : GenericRepository<Product>
    {
        public int ProductCount => _storage.Count;
        public List<Product> Storage => _storage;

        /// <summary>
        /// Adds a manufactured product to the generic collection.
        /// </summary>
        public bool AddProduct(Product product)
        {
            if (_storage.Any(p => p.Id == product.Id))
            {
                Logger.Warning($"The product with the ID {product.Id} already exists in the company!");
                return false;
            }

            Add(product);
            Logger.Info($"The product with ID {product.Id} ({product.Name}) has been added.");

            return true;
        }

        /// <summary>
        /// Removes a product from storage (marks it as sold) and updates factory budget.
        /// </summary>
        public float SoldProduct(Product product, float income)
        {
            if (_storage.Count == 0)
            {
                Logger.Error("The storage was left without products!");
                return income;
            }

            if (_storage.Contains(product))
            {
                Remove(product);
                income = product.SellProduct(income);
                Logger.Info($"Product {product.Name} #{product.Id} has been sold for {product.Price} RON.");

                return income;
            }
            else
            {
                Logger.Info($"The product with the ID {product.Id} does not exist in the storage's company!");
                return income;
            }
        }

        /// <summary>
        /// Searches storage for a product by ID. Re-evaluates quality as a side effect.
        /// </summary>
        public int Search(int id)
        {
            var product = Find(p => p.Id == id);

            if (product != null)
            {
                product.TestProduct();
                Logger.Info($"The product {id} ({product.Name}) was found and it is functional!");
                return _storage.IndexOf(product);
            }

            Logger.Warning($"Product {id} was not found.");
            return -1;
        }
    }
}
