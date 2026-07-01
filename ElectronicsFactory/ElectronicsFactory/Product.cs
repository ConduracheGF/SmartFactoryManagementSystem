using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace ElectronicsFactory
{
    public abstract class Product
    {
        private float currency;
        private float consumption;
        private float battery;
        private string? brand;
        private string? quality;

        public float Currency { get { return currency; } set { currency = value; } }
        public float Consumption { get { return consumption; } set { consumption = value; } }
        public float Battery { get { return battery; } set { battery = value; } }
        public string? Brand { get { return brand; } set { brand = value; } }
        public string? Quality { get { return quality; } set { quality = value; } }

        public Product(float currency, float consumption, float battery, string? brand, string? quality)
        {
            this.currency = currency;
            this.consumption = consumption;
            this.battery = battery;
            this.brand = brand;
            this.quality = quality;
        }

        public float SellProduct(float income)
        {
            return currency + income;
        }

        public virtual void TestProduct()
        {
            float ratio = battery / consumption;

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
    }

    internal class Phones : Product
    {
        private int yearOfProduction;
        private string? processor;

        public int YearOfProduction { get { return yearOfProduction; } set { yearOfProduction = value; } }
        public string? Processor { get { return processor; } set { processor = value; } }

        public Phones(float currency, float consumption, float battery, string? brand, string? quality, int yearOfProduction, string? processor) : base(currency, consumption, battery, brand, quality)
        {
            this.yearOfProduction = yearOfProduction;
            this.processor = processor;
        }

        public override void TestProduct()
        {
            float ratio = Battery / Consumption;

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

            Console.WriteLine($"Telefonul este de quality de tipul: {Quality}");
        }

        public void DisplayFunctionality()
        {
            Console.WriteLine("Telefonul poate efectua apeluri si realiza diverse taskuri!");
        }
    }

    internal class Tablets : Product
    {
        private int yearOfProduction;
        private string? processor;

        public int YearOfProduction { get { return yearOfProduction; } set { yearOfProduction = value; } }
        public string? Processor { get { return processor; } set { processor = value; } }

        public Tablets(float currency, float consumption, float battery, string? brand, string? quality, int yearOfProduction, string? processor) : base(currency, consumption, battery, brand, quality)
        {
            this.yearOfProduction = yearOfProduction;
            this.processor = processor;
        }

        public override void TestProduct()
        {
            float ratio = Battery / Consumption;

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

            Logger.Info($"Tableta este de quality de tipul: {Quality}");
        }

        public void DisplayFunctionality()
        {
            Logger.Info("Tableta poate efectua orice task dorit, atata timp cat descarci aplicatia!");
        }
    }

    internal class Computers : Product
    {
        private int yearOfProduction;
        private int weight;
        private string? processor;

        public int Weight { get { return weight; } set { weight = value; } }
        public string? Processor { get { return processor; } set { processor = value; } }

        public Computers(float currency, float consumption, float battery, string? brand, string? quality, string? processor, int weight) : base(currency, consumption, battery, brand, quality)
        {
            this.processor = processor;
            this.weight = weight;
        }

        public override void TestProduct()
        {
            float ratio = Battery / Consumption;

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

            Logger.Info($"Computerul este de quality de tipul: {Quality}");
        }

        public void WifiConectionDescription()
        {
            Logger.Info("Computerul poate efectua cautari si orice conectare la Wifi!");
        }
    }

    internal class Headphones : Product
    {
        private int yearOfProduction;
        private int power;

        public int YearOfProduction { get { return yearOfProduction; } set { yearOfProduction = value; } }
        public int Power { get { return power; } set { power = value; } }

        public Headphones(float currency, float consumption, float battery, string? brand, string? quality, int yearOfProduction, int power) : base(currency, consumption, battery, brand, quality)
        {
            this.yearOfProduction = yearOfProduction;
            this.power = power;
        }

        public override void TestProduct()
        {
            float sunet = QualitySound();
            float ratio = (Battery / Consumption) - sunet;

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

            Logger.Info($"Telefonul este de quality de tipul: {Quality}");
        }

        public float QualitySound()
        {
            return Power / Consumption;
        }
    }
}