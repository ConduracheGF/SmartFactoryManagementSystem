using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace ElectronicsFactory
{
    /// <summary>
    /// Category of manufactured electronic product
    /// </summary>
    public enum ProductType_t
    {
        Phones,
        Tablets,
        Computers,
        Headphones
    }

    /// <summary>
    /// Manages the factory's product inventory: adding, selling, and searching
    /// </summary>
    internal class ProductManagement
    {
        private Product[] storage;
        private int productsCount = 0;

        // Underlying fixed-size storage array for products (may contain unused trailing slots)
        public Product[] Storage { get { return storage; } set { storage = value; } }

        // Number of products currently in stock
        public int ProductsCount { get { return productsCount; } }

        // Initializes product storage with a fixed maximum capacity
        public ProductManagement(int maxCapacity)
        {
            storage = new Product[maxCapacity];
            productsCount = 0;
        }

        // Adds a manufactured product to storage, rejecting it if capacity is full or if a product with the same ID already exists
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

        // Removes a product from storage (marks it as sold)
        // Shifting remaining elements left to keep storage compact
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
                    // Shift all subsequent elements one position left, then shrink the logical count
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

        // Searches storage for a product by ID
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
                    // Re-evaluates the product's quality as a side effect when found
                    storage[i].TestProduct();
                    Logger.Info($"The product {id} was found and it is functional!");
                    return i;
                }
            }
            Logger.Warning($"Product {id} was not found.");
            return -1;
        }
    }

    /// <summary>
    /// Abstract base class representing any manufactured product in the factory
    /// Provides shared identity (auto-generated ID), pricing, and quality-testing behavior
    /// </summary>
    public abstract class Product
    {
        // Static counter shared across all Product instances, used to auto-generate unique IDs
        private static int nextId = 0;

        private int id;
        private float currency;
        private float consumption;
        private string? quality;

        // Unique, auto-generated identifier assigned at construction time
        public int Id { get { return id; } private set { id= value; } }

        // Selling price / production cost of the product, in RON
        public float Currency { get { return currency; } set { currency = value; } }

        // Energy consumption metric used to compute the product's quality grade
        public float Consumption { get { return consumption; } set { consumption = value; } }

        // Current quality grade of the product (A-E)
        public string? Quality { get { return quality; } set { quality = value; } }

        // The product category (Phones, Tablets, Computers, Headphones)
        public ProductType_t ProductType { get; set; }

        // Initializes a new product with an auto-generated unique ID
        public Product(float currency, float consumption, string? quality, ProductType_t ProductType)
        {
            id = nextId++;
            this.currency = currency;
            this.consumption = consumption;
            this.quality = quality;
            this.ProductType = ProductType;
        }

        /// Computes the updated factory income after selling this product
        public float SellProduct(float income)
        {
            return currency + income;
        }

        /// Recalculates this product's quality grade based on its consumption ratio
        /// Overridden by each subclass with its own grading thresholds
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

        /// Creates a new, independent instance of this product with the same attributes, but a freshly auto-generated ID
        /// Used when manufacturing multiple units from a single "template" product during a production run
        public abstract Product Clone();
    }

    /// <summary>
    /// Represents a manufactured smartphone product
    /// </summary>
    internal class Phones : Product
    {
        private int yearOfProduction;
        private string? processor;

        // Year the phone model was produced
        public int YearOfProduction { get { return yearOfProduction; } set { yearOfProduction = value; } }

        // Processor chip model used in the phone
        public string? Processor { get { return processor; } set { processor = value; } }

        /// Initializes a new Phones product
        public Phones(float currency, float consumption, string? quality, ProductType_t productType, int yearOfProduction, string? processor) : base(currency, consumption, quality, productType)
        {
            this.yearOfProduction = yearOfProduction;
            this.processor = processor;
        }

        // Phone-specific quality grading based on consumption thresholds
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

        // Displays the phone's core functionality description
        public void DisplayFunctionality()
        {
            Logger.Info("The phone can make calls and perform various tasks!");
        }

        public override Product Clone()
        {
            return new Phones(Currency, Consumption, Quality, ProductType, YearOfProduction, Processor);
        }
    }

    /// <summary>
    /// Represents a manufactured tablet product
    /// </summary>
    internal class Tablets : Product
    {
        private int yearOfProduction;
        private string? processor;

        // Year the tablet model was produced
        public int YearOfProduction { get { return yearOfProduction; } set { yearOfProduction = value; } }

        // Processor chip model used in the tablet
        public string? Processor { get { return processor; } set { processor = value; } }

        // Initializes a new Tablets product
        public Tablets(float currency, float consumption, string? quality, ProductType_t productType, int yearOfProduction, string? processor) : base(currency, consumption, quality, productType)
        {
            this.yearOfProduction = yearOfProduction;
            this.processor = processor;
        }

        // Tablet-specific quality grading based on consumption thresholds
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

        // Displays the tablet's core functionality description
        public void DisplayFunctionality()
        {
            Logger.Info("The tablet can perform any task you want, as long as you download the application!");
        }

        public override Product Clone()
        {
            return new Tablets(Currency, Consumption, Quality, ProductType, YearOfProduction, Processor);
        }
    }

    /// <summary>
    /// Represents a manufactured computer product
    /// </summary>
    internal class Computers : Product
    {
        private int weight;
        private string? processor;

        // Physical weight of the computer, in kilograms
        public int Weight { get { return weight; } set { weight = value; } }

        // Processor chip model used in the computer
        public string? Processor { get { return processor; } set { processor = value; } }

        // Initializes a new Computers product
        public Computers(float currency, float consumption, string? quality, ProductType_t productType, string? processor, int weight) : base(currency, consumption, quality, productType)
        {
            this.processor = processor;
            this.weight = weight;
        }

        // Computer-specific quality grading based on consumption thresholds
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

        // Displays the computer's Wi-Fi connectivity description
        public void WifiConectionDescription()
        {
            Logger.Info("The computer can perform searches and any connection to Wifi!");
        }

        public override Product Clone()
        {
            return new Computers(Currency, Consumption, Quality, ProductType, Processor, Weight);
        }
    }

    /// <summary>
    /// Represents a manufactured headphones product
    /// </summary>
    internal class Headphones : Product
    {
        private int yearOfProduction;
        private int power;

        // Year the headphones model was produced
        public int YearOfProduction { get { return yearOfProduction; } set { yearOfProduction = value; } }

        // Audio output power of the headphones, in milliwatts
        public int Power { get { return power; } set { power = value; } }

        // Initializes a new Headphones product
        public Headphones(float currency, float consumption, string? quality, ProductType_t productType, int yearOfProduction, int power) : base(currency, consumption, quality, productType)
        {
            this.yearOfProduction = yearOfProduction;
            this.power = power;
        }

        // Headphones-specific quality grading, factoring in sound quality (power-to-consumption ratio) alongside raw consumption.
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

            Logger.Info($"The headphone is of quality type: {Quality} and it is optimal sound");
        }

        // Computes the sound quality metric as the ratio of output power to energy consumption
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