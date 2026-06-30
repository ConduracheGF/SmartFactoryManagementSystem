using System;
using System.Collections.Generic;
using System.Text;

namespace ElectronicsFactory
{
    public abstract class MachineParts
    {
        private float valoare;
        private string? brand;
        private int clasaConsum;

        public float Valoare { get { return valoare; } set { valoare = value; } }
        public string? Brand { get { return brand; } private set { brand = value; } }
        public int ClasaConsum { get { return clasaConsum; } private set { clasaConsum = value; } }
        public MachineParts(float valoare = 0, string? brand = null, int clasaConsum = 0)
        {
            this.valoare = valoare;
            this.brand = brand;
            this.clasaConsum = clasaConsum;
        }

        public virtual float Cheltuiala()
        {
            return valoare;
        }
    }

    internal class Motor : MachineParts
    {
        private float power;
        private int caiPutere;
        public float Power { get { return power; } set { power = value; } }
        public int CaiPutere { get { return caiPutere; } set { caiPutere = value; } }

        public Motor(int valoare = 0, string? brand = null, int clasaConsum = 0, float power = 0.0f, int caiPutere = 0) : base(valoare, brand, clasaConsum)
        {
            this.power = power;
            this.caiPutere = caiPutere;
        }
        public override float Cheltuiala()
        {
            return ((Valoare * ClasaConsum) - (Power / ClasaConsum));
        }
    }

    internal class Senzor : MachineParts
    {
        private float percentAccuracy;
        private int frequency;
        public float PercentAccuracy { get { return percentAccuracy; } set { percentAccuracy = value; } }
        public int Frequency { get { return frequency; } set { frequency = value; } }

        public Senzor(int valoare = 0, string? brand = null, int clasaConsum = 0, float percentAccuracy = 0.0f, int frequency = 0) : base(valoare, brand, clasaConsum)
        {
            this.frequency = frequency;
            this.percentAccuracy = percentAccuracy;
        }

        public override float Cheltuiala()
        {
            return ((base.Cheltuiala() / ClasaConsum) - (percentAccuracy * frequency * ClasaConsum));
        }
    }

    internal class Controler : MachineParts
    {
        private int frequency;
        public int Frequency { get { return frequency; } set { frequency = value; } }

        public Controler(int valoare = 0, string? brand = null, int clasaConsum = 0, int frequency = 0) : base(valoare, brand, clasaConsum)
        {
            this.frequency = frequency;
        }

        public override float Cheltuiala()
        {
            return base.Cheltuiala();
        }
    }

    internal class Display : MachineParts
    {
        private float rezolutie;
        public float Rezolutie { get { return rezolutie; } private set { rezolutie = value; } }

        public Display(int valoare = 0, string? brand = null, int clasaConsum = 0, float rezolutie = 0.0f) : base(valoare, brand, clasaConsum)
        {
            this.rezolutie = rezolutie;
        }

        public override float Cheltuiala()
        {
            return base.Cheltuiala();
        }
    }
    internal class CoolingFan : MachineParts
    {
        private int viteza;

        public int Viteza { get { return viteza; } private set { viteza = value; } }

        public CoolingFan(int valoare = 0, string? brand = null, int clasaConsum = 0, int viteza = 0) : base(valoare, brand, clasaConsum)
        {
            this.viteza = viteza;
        }

        public override float Cheltuiala()
        {
            return base.Cheltuiala();
        }
    }
}
