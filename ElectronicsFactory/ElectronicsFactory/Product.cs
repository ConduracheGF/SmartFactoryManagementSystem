using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace ElectronicsFactory
{
    public enum ProductType_t
    {
        Phones,
        Tablets,
        Computers,
        Headphones
    }

    internal class ProductManagement
    {
        private Product[] storage;
        private int productsCount = 0;

        public Product[] Storage { get { return storage; } set { storage = value; } }
        public int ProductsCount { get { return productsCount; } }
        public ProductManagement(int maxCapacity)
        {
            storage = new Product[maxCapacity];
            productsCount = 0;
        }
        public bool AddProduct(Product product)
        {
            if (productsCount >= storage.Length)
            {
                Logger.Error("The storage has reached its maximum products limit!");
                return false;
            }

            int index = Search(product.Id);

            if (index == -1)
            {
                storage[productsCount] = product;
                productsCount++;

                Logger.Info($"The product with ID {product.Id} has been added.");
                return true;
            }
            else
            {
                Logger.Warning($"The product with the ID {product.Id} already exists in the company!"); return false;
            }
        }
        public float SoldProduct(Product product, float income)
        {
            if (productsCount == 0)
            {
                Logger.Error("The storage was left without products!"); 
                return income;
            }

            int index = Search(product.Id);

            if (index != -1)
            {
                for (int i = index; i < storage.Length - 1; i++)
                {
                    storage[i] = storage[i + 1];
                }

                productsCount--;
                income = product.SellProduct(income);
                Logger.Info($"Product with ID {product.Id} has been sold with {product.Currency}.");
                return income;
            }
            else
            {
                Logger.Info($"The product with the ID {product.Id} does not exist in the storage's company!");
                return income;
            }
        }

        public int Search(int id)
        {
            if (productsCount == 0)
            {
                Logger.Info("There are no products in the storage's factory.");
                return -1;
            }

            for (int i = 0; i < productsCount; i++)
            {
                if (storage[i].Id == id)
                {
                    storage[i].TestProduct();
                    Logger.Info($"The product {id} was found and it is functional!");
                    return i;
                }
            }
            Logger.Warning($"Product {id} was not found.");
            return -1;
        }

        public float CalculateValue()
        {
            float value = 0;
            foreach(Product product in storage)
            {
                value += product.Currency;
            }
            return value;
        }
    }
    public abstract class Product
    {
        private static int nextId = 1;

        private int id;
        private float currency;
        private float consumption;
        private string? quality;

        public int Id { get { return id; } private set { id= value; } }
        public float Currency { get { return currency; } set { currency = value; } }
        public float Consumption { get { return consumption; } set { consumption = value; } }
        public string? Quality { get { return quality; } set { quality = value; } }
        public ProductType_t ProductType { get; set; }

        public Product(float currency, float consumption, string? quality, ProductType_t ProductType)
        {
            id = nextId++;
            this.currency = currency;
            this.consumption = consumption;
            this.quality = quality;
            this.ProductType = ProductType;
        }

        public float SellProduct(float income)
        {
            return currency + income;
        }

        public virtual void TestProduct()
        {
            float ratio = consumption;

            if (ratio > 0 && ratio <= 5)
            {
                quality = "E";
            }
            else if (ratio > 5 && ratio <= 8)
            {
                quality = "D";
            }
            else if (ratio > 8 && ratio <= 10)
            {
                quality = "C";
            }
            else if (ratio > 10 && ratio <= 12)
            {
                quality = "B";
            }
            else if ( ratio > 12 && ratio < 15)
            {
                quality = "A";
            }
        }

        public abstract Product Clone();
    }

    internal class Phones : Product
    {
        private int yearOfProduction;
        private string? processor;

        public int YearOfProduction { get { return yearOfProduction; } set { yearOfProduction = value; } }
        public string? Processor { get { return processor; } set { processor = value; } }

        public Phones(float currency, float consumption, string? quality, ProductType_t productType, int yearOfProduction, string? processor) : base(currency, consumption, quality, productType)
        {
            this.yearOfProduction = yearOfProduction;
            this.processor = processor;
        }

        public override void TestProduct()
        {
            float ratio = Consumption;

            if (ratio > 0 && ratio <= 5)
            {
                Quality = "E";
            }
            else if (ratio > 5 && ratio <= 8)
            {
                Quality = "D";
            }
            else if (ratio > 8 && ratio <= 10)
            {
                Quality = "C";
            }
            else if (ratio > 10 && ratio <= 12)
            {
                Quality = "B";
            }
            else if (ratio > 12 && ratio < 15)
            {
                Quality = "A";
            }

            Logger.Info($"The phone is of quality type: {Quality}");
        }

        public void DisplayFunctionality()
        {
            Logger.Info("The phone can make calls and perform various tasks!");
        }

        public override Product Clone()
        {
            return new Phones(Currency, Consumption, Quality, ProductType, YearOfProduction, Processor);
        }
    }

    internal class Tablets : Product
    {
        private int yearOfProduction;
        private string? processor;

        public int YearOfProduction { get { return yearOfProduction; } set { yearOfProduction = value; } }
        public string? Processor { get { return processor; } set { processor = value; } }

        public Tablets(float currency, float consumption, string? quality, ProductType_t productType, int yearOfProduction, string? processor) : base(currency, consumption, quality, productType)
        {
            this.yearOfProduction = yearOfProduction;
            this.processor = processor;
        }

        public override void TestProduct()
        {
            float ratio = Consumption;

            if (ratio > 0 && ratio <= 6)
            {
                Quality = "E";
            }
            else if (ratio > 6 && ratio <= 9)
            {
                Quality = "D";
            }
            else if (ratio > 9 && ratio <= 12)
            {
                Quality = "C";
            }
            else if (ratio > 12 && ratio <= 15)
            {
                Quality = "B";
            }
            else if (ratio > 15 && ratio < 24)
            {
                Quality = "A";
            }

            Logger.Info($"The tablet is of quality type: {Quality}");
        }

        public void DisplayFunctionality()
        {
            Logger.Info("The tablet can perform any task you want, as long as you download the application!");
        }

        public override Product Clone()
        {
            return new Tablets(Currency, Consumption, Quality, ProductType, YearOfProduction, Processor);
        }
    }

    internal class Computers : Product
    {
        private int weight;
        private string? processor;

        public int Weight { get { return weight; } set { weight = value; } }
        public string? Processor { get { return processor; } set { processor = value; } }

        public Computers(float currency, float consumption, string? quality, ProductType_t productType, string? processor, int weight) : base(currency, consumption, quality, productType)
        {
            this.processor = processor;
            this.weight = weight;
        }

        public override void TestProduct()
        {
            float ratio = Consumption;

            if (ratio > 0 && ratio <= 7)
            {
                Quality = "E";
            }
            else if (ratio > 7 && ratio <= 10)
            {
                Quality = "D";
            }
            else if (ratio > 10 && ratio <= 14)
            {
                Quality = "C";
            }
            else if (ratio > 14 && ratio <= 20)
            {
                Quality = "B";
            }
            else if (ratio > 20 && ratio < 36)
            {
                Quality = "A";
            }

            Logger.Info($"The computer is of quality type: {Quality}");
        }

        public void WifiConectionDescription()
        {
            Logger.Info("The computer can perform searches and any connection to Wifi!");
        }

        public override Product Clone()
        {
            return new Computers(Currency, Consumption, Quality, ProductType, Processor, Weight);
        }
    }

    internal class Headphones : Product
    {
        private int yearOfProduction;
        private int power;

        public int YearOfProduction { get { return yearOfProduction; } set { yearOfProduction = value; } }
        public int Power { get { return power; } set { power = value; } }

        public Headphones(float currency, float consumption, string? quality, ProductType_t productType, int yearOfProduction, int power) : base(currency, consumption, quality, productType)
        {
            this.yearOfProduction = yearOfProduction;
            this.power = power;
        }

        public override void TestProduct()
        {
            float sunet = QualitySound();
            float ratio = (Consumption - sunet);

            if (ratio > 0 && ratio <= 5)
            {
                Quality = "E";
            }
            else if (ratio > 5 && ratio <= 8)
            {
                Quality = "D";
            }
            else if (ratio > 8 && ratio <= 10)
            {
                Quality = "C";
            }
            else if (ratio > 10 && ratio <= 12)
            {
                Quality = "B";
            }
            else if (ratio > 12 && ratio < 15)
            {
                Quality = "A";
            }

            Logger.Info($"The headphone is of quality type: {Quality}");
        }

        public float QualitySound()
        {
            return Power / Consumption;
        }

        public override Product Clone()
        {
            return new Headphones(Currency, Consumption, Quality, ProductType, YearOfProduction, Power);
        }
    }
}