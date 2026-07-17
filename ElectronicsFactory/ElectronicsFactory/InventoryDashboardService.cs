using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsFactory
{
    internal class InventoryDashboardService: IInventoryDashboardService
    {
        private readonly ProductManagement _productManager;

        public InventoryDashboardService(ProductManagement productManager)
        {
            _productManager = productManager;
        }

        // Scans the warehouse storage and warns if any product category falls below the specified safety threshold.
        public void VerifyStockThresholds(int minimumAllowedUnits)
        {
            Logger.Info("Scanning inventory stocks...");
            bool alertTriggered = false;

            var productGroups = _productManager.Storage.GroupBy(p => p.ProductType).ToDictionary(g => g.Key, g => g.Count());

            foreach (ProductType_t type in Enum.GetValues(typeof(ProductType_t)))
            {
                int currentCount = productGroups.ContainsKey(type) ? productGroups[type] : 0;
                
                if (currentCount < minimumAllowedUnits)
                {
                    Logger.Warning($"Category '{type}' has dropped below critical levels! Current stock: {currentCount} units. Limit: {minimumAllowedUnits} units.");
                    alertTriggered = true;
                }
            }

            if (!alertTriggered)
            {
                Logger.Info("Inventory levels are safe.");
            }
        }

        public void DisplayAdvancedReports()
        {
            Logger.Info("Advanced Financial Report");

            float totalValue = _productManager.Storage.Sum(p => p.Price);
            Logger.Info($"Total Warehouse Inventory Value: {totalValue} RON");

            var mostExpensive = _productManager.Storage.OrderByDescending(p => p.Price).FirstOrDefault();
            if (mostExpensive != null)
            {
                Logger.Info($"Most Expensive Stocked Item: {mostExpensive.Name}. Price: {mostExpensive.Price} RON.");
            }
        }
    }
}
