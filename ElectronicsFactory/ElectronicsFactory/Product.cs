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
    /// Abstract base class representing any manufactured product in the factory
    /// Provides shared identity (auto-generated ID), pricing, and quality-testing behavior
    /// </summary>
    public abstract class Product
    {
        // Static counter shared across all Product instances, used to auto-generate unique IDs
        private static int _nextId = 1;

        private int _id;
        private string _name;
        private float _price;
        private float _consumption;
        private string? _quality;

        // Unique, auto-generated identifier assigned at construction time
        public int Id { get { return _id; } private set { _id = value; } }

        // Business-facing name of the product
        public string Name { get { return _name; } set { _name = value; } }

        // Selling price / production cost of the product, in RON
        public float Price { get { return _price; } set { _price = value; } }

        // Energy consumption metric used to compute the product's quality grade
        public float Consumption { get { return _consumption; } set { _consumption = value; } }

        // Current quality grade of the product (A-E)
        public string? Quality { get { return _quality; } set { _quality = value; } }

        // The product category (Phones, Tablets, Computers, Headphones)
        public ProductType_t ProductType { get; set; }

        // Initializes a new product with an auto-generated unique ID
        public Product(string name, float price, float consumption, string? quality, ProductType_t ProductType)
        {
            _id = _nextId++;
            this._name = name;
            this._price = price;
            this._consumption = consumption;
            this._quality = quality;
            this.ProductType = ProductType;
        }

        /// Computes the updated factory income after selling this product
        public float SellProduct(float income)
        {
            return Price + income;
        }

        /// Recalculates this product's quality grade based on its consumption ratio
        public abstract void TestProduct();

        /// Creates a new, independent instance of this product with the same attributes, but a freshly auto-generated ID
        /// Used when manufacturing multiple units from a single "template" product during a production run
        public abstract Product Clone();

        /// Converts the product data into a standardized CSV string format for file persistence
        public virtual string ToFileRow()
        {
            return $"{Id};{Name};{Price};{Consumption};{Quality};{ProductType}";
        }
    }

    /// <summary>
    /// Represents a manufactured smartphone product
    /// </summary>
    internal class Phones : Product
    {
        private int _yearOfProduction;
        private string? _processor;

        // Year the phone model was produced
        public int YearOfProduction { get { return _yearOfProduction; } set { _yearOfProduction = value; } }

        // Processor chip model used in the phone
        public string? Processor { get { return _processor; } set { _processor = value; } }

        /// Initializes a new Phones product
        public Phones(string name, float currency, float consumption, string? quality, ProductType_t productType, int yearOfProduction, string? processor) : base(name, currency, consumption, quality, productType)
        {
            this._yearOfProduction = yearOfProduction;
            this._processor = processor;
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
            return new Phones(Name, Price, Consumption, Quality, ProductType, YearOfProduction, Processor);
        }

        public override string ToFileRow()
        {
            return base.ToFileRow() + $";{YearOfProduction};{Processor}";
        }
    }

    /// <summary>
    /// Represents a manufactured tablet product
    /// </summary>
    internal class Tablets : Product
    {
        private int _yearOfProduction;
        private string? _processor;

        // Year the tablet model was produced
        public int YearOfProduction { get { return _yearOfProduction; } set { _yearOfProduction = value; } }

        // Processor chip model used in the tablet
        public string? Processor { get { return _processor; } set { _processor = value; } }

        // Initializes a new Tablets product
        public Tablets(string name, float currency, float consumption, string? quality, ProductType_t productType, int yearOfProduction, string? processor) : base(name, currency, consumption, quality, productType)
        {
            this._yearOfProduction = yearOfProduction;
            this._processor = processor;
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
            return new Tablets(Name, Price, Consumption, Quality, ProductType, YearOfProduction, Processor);
        }

        public override string ToFileRow()
        {
            return base.ToFileRow() + $";{YearOfProduction};{Processor}";
        }
    }

    /// <summary>
    /// Represents a manufactured computer product
    /// </summary>
    internal class Computers : Product
    {
        private int _weight;
        private string? _processor;

        // Physical weight of the computer, in kilograms
        public int Weight { get { return _weight; } set { _weight = value; } }

        // Processor chip model used in the computer
        public string? Processor { get { return _processor; } set { _processor = value; } }

        // Initializes a new Computers product
        public Computers(string name, float currency, float consumption, string? quality, ProductType_t productType, string? processor, int weight) : base(name, currency, consumption, quality, productType)
        {
            this._processor = processor;
            this._weight = weight;
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
            return new Computers(Name, Price, Consumption, Quality, ProductType, Processor, Weight);
        }

        public override string ToFileRow()
        {
            return base.ToFileRow() + $";{Weight};{Processor}";
        }
    }

    /// <summary>
    /// Represents a manufactured headphones product
    /// </summary>
    internal class Headphones : Product
    {
        private int _yearOfProduction;
        private int _power;

        // Year the headphones model was produced
        public int YearOfProduction { get { return _yearOfProduction; } set { _yearOfProduction = value; } }

        // Audio output power of the headphones, in milliwatts
        public int Power { get { return _power; } set { _power = value; } }

        // Initializes a new Headphones product
        public Headphones(string name, float currency, float consumption, string? quality, ProductType_t productType, int yearOfProduction, int power) : base(name, currency, consumption, quality, productType)
        {
            this._yearOfProduction = yearOfProduction;
            this._power = power;
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
            return new Headphones(Name, Price, Consumption, Quality, ProductType, YearOfProduction, Power);
        }

        public override string ToFileRow()
        {
            return base.ToFileRow() + $";{YearOfProduction};{Power}";
        }
    }
}