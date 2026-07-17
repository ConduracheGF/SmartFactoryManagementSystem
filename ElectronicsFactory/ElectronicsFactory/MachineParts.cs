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

    // Abstract base class for any replaceable component that can be installed
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

        // Calculates the remaining income after replacing this component during a repair.
        public virtual float Replacement(float income)
        {
            return (income - currency);
        }
    }


    internal class Motor : MachineParts
    {
        private float powerEnergy;
        private int horsePower;

        // Power consumption of the motor, in watts
        public float PowerEnergy { get { return powerEnergy; } set { powerEnergy = value; } }

        // Mechanical power output of the motor, in horsepower
        public int HorsePower { get { return horsePower; } set { horsePower = value; } }

        
        public Motor(int currency = 0, string? brand = null, int energyClass = 0, ComponentsType_t component = ComponentsType_t.Motor, float powerEnergy = 0.0f, int horsePower = 0) : base(currency, brand, energyClass, component)
        {
            this.powerEnergy = powerEnergy;
            this.horsePower = horsePower;
        }

        // Motor-specific replacement formula, factoring in energy class and power output.
        public override float Replacement(float income)
        {
            return (income - ((Currency * EnergyClass) - (PowerEnergy / EnergyClass)));
        }
    }

  
    internal class Senzor : MachineParts
    {
        private float percentAccuracy;
        private int frequency;

        // Measurement accuracy of the sensor, expressed as a fraction (0.0 - 1.0)
        public float PercentAccuracy { get { return percentAccuracy; } set { percentAccuracy = value; } }
        
        // Sampling frequency of the sensor, in Hz
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

    // Represents a controller component, responsible for coordinating machine logic
    internal class Controler : MachineParts
    {
        private int frequency;

        // Clock frequency of the controller, in MHz
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

        // Display resolution, in pixels
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

    // Represents a cooling fan component, used for thermal regulation of a machine
    internal class CoolingFan : MachineParts
    {
        private int speed;

        // Fan rotation speed, in RPM
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
