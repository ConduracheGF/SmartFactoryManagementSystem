using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsFactory
{
    public enum ComponentsType_t
    {
        Motor,
        Senzor,
        Controler,
        Display,
        CoolingFan
    }

    public abstract class MachineParts
    {
        private float currency;
        private string? brand;
        private int energyClass;
        private ComponentsType_t component;

        public float Currency { get { return currency; } set { currency = value; } }
        public string? Brand { get { return brand; } private set { brand = value; } }
        public int EnergyClass { get { return energyClass; } private set { energyClass = value; } }
        public ComponentsType_t Component { get { return component; } set { component = value; } }
        public MachineParts(float currency = 0, string? brand = null, int energyClass = 0, ComponentsType_t component = ComponentsType_t.Motor)
        {
            this.currency = currency;
            this.Brand = brand;
            this.energyClass = energyClass;
            this.component = component;
        }

        public virtual float Replacement(float income)
        {
            return (income - currency);
        }
    }

    internal class Motor : MachineParts
    {
        private float powerEnergy;
        private int horsePower;
        public float PowerEnergy { get { return powerEnergy; } set { powerEnergy = value; } }
        public int HorsePower { get { return horsePower; } set { horsePower = value; } }

        public Motor(int currency = 0, string? brand = null, int energyClass = 0, ComponentsType_t component = ComponentsType_t.Motor, float powerEnergy = 0.0f, int horsePower = 0) : base(currency, brand, energyClass, component)
        {
            this.powerEnergy = powerEnergy;
            this.horsePower = horsePower;
        }
        public override float Replacement(float income)
        {
            return (income - ((Currency * EnergyClass) - (PowerEnergy / EnergyClass)));
        }
    }

    internal class Senzor : MachineParts
    {
        private float percentAccuracy;
        private int frequency;
        public float PercentAccuracy { get { return percentAccuracy; } set { percentAccuracy = value; } }
        public int Frequency { get { return frequency; } set { frequency = value; } }

        public Senzor(int currency = 0, string? brand = null, int energyClass = 0, ComponentsType_t component = ComponentsType_t.Senzor, float percentAccuracy = 0.0f, int frequency = 0) : base(currency, brand, energyClass, component)
        {
            this.frequency = frequency;
            this.percentAccuracy = percentAccuracy;
        }

        public override float Replacement(float income)
        {
            return (income - (percentAccuracy * Currency * EnergyClass));
        }
    }

    internal class Controler : MachineParts
    {
        private int frequency;
        public int Frequency { get { return frequency; } set { frequency = value; } }

        public Controler(int currency = 0, string? brand = null, int energyClass = 0, ComponentsType_t component = ComponentsType_t.Controler, int frequency = 0) : base(currency, brand, energyClass, component)
        {
            this.frequency = frequency;
        }

        public override float Replacement(float income)
        {
            return base.Replacement(income);
        }
    }

    internal class Display : MachineParts
    {
        private float rezolution;
        public float Rezolution { get { return rezolution; } private set { rezolution = value; } }

        public Display(int currency = 0, string? brand = null, int energyClass = 0, ComponentsType_t component = ComponentsType_t.Display, float rezolution = 0.0f) : base(currency, brand, energyClass, component)
        {
            this.rezolution = rezolution;
        }

        public override float Replacement(float income)
        {
            return base.Replacement(income);
        }
    }
    internal class CoolingFan : MachineParts
    {
        private int speed;

        public int Speed { get { return speed; } private set { speed = value; } }

        public CoolingFan(int currency = 0, string? brand = null, int energyClass = 0, ComponentsType_t component = ComponentsType_t.CoolingFan, int speed = 0) : base(currency, brand, energyClass, component)
        {
            this.speed = speed;
        }

        public override float Replacement(float income)
        {
            return base.Replacement(income);
        }
    }
}
