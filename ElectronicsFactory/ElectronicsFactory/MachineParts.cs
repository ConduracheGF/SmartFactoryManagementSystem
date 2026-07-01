using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsFactory
{
    public abstract class MachineParts
    {
        private float currency;
        private string? brand;
        private int energyClass;

        public float Currency { get { return currency; } set { currency = value; } }
        public string? Brand { get { return brand; } private set { brand = value; } }
        public int EnergyClass { get { return energyClass; } private set { energyClass = value; } }
        public MachineParts(float currency = 0, string? brand = null, int energyClass = 0)
        {
            this.currency = currency;
            this.brand = brand;
            this.energyClass = energyClass;
        }

        public virtual float Expense(float income)
        {
            return income - currency;
        }
    }

    internal class Motor : MachineParts
    {
        private float power;
        private int horsePower;
        public float Power { get { return power; } set { power = value; } }
        public int HorsePower { get { return horsePower; } set { horsePower = value; } }

        public Motor(int currency = 0, string? brand = null, int energyClass = 0, float power = 0.0f, int horsePower = 0) : base(currency, brand, energyClass)
        {
            this.power = power;
            this.horsePower = horsePower;
        }
        public override float Expense(float income)
        {
            return income - ((Currency * EnergyClass) - (Power / EnergyClass));
        }
    }

    internal class Senzor : MachineParts
    {
        private float percentAccuracy;
        private int frequency;
        public float PercentAccuracy { get { return percentAccuracy; } set { percentAccuracy = value; } }
        public int Frequency { get { return frequency; } set { frequency = value; } }

        public Senzor(int currency = 0, string? brand = null, int energyClass = 0, float percentAccuracy = 0.0f, int frequency = 0) : base(currency, brand, energyClass)
        {
            this.frequency = frequency;
            this.percentAccuracy = percentAccuracy;
        }

        public override float Expense(float income)
        {
            return ((income - (percentAccuracy * Currency * EnergyClass));
        }
    }

    internal class Controler : MachineParts
    {
        private int frequency;
        public int Frequency { get { return frequency; } set { frequency = value; } }

        public Controler(int currency = 0, string? brand = null, int energyClass = 0, int frequency = 0) : base(currency, brand, energyClass)
        {
            this.frequency = frequency;
        }

        public override float Expense(float income)
        {
            return base.Expense(income);
        }
    }

    internal class Display : MachineParts
    {
        private float rezolution;
        public float Rezolution { get { return rezolution; } private set { rezolution = value; } }

        public Display(int currency = 0, string? brand = null, int energyClass = 0, float rezolution = 0.0f) : base(currency, brand, energyClass)
        {
            this.rezolution = rezolution;
        }

        public override float Expense(float income)
        {
            return base.Expense(income);
        }
    }
    internal class CoolingFan : MachineParts
    {
        private int speed;

        public int Speed { get { return speed; } private set { speed = value; } }

        public CoolingFan(int currency = 0, string? brand = null, int energyClass = 0, int speed = 0) : base(currency, brand, energyClass)
        {
            this.speed = speed;
        }

        public override float Expense(float income)
        {
            return base.Expense(income);
        }
    }
}
