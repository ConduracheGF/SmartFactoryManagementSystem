using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace ElectronicsFactory
{
    public abstract class Product
    {
        private float valoare;
        private float consum;
        private float baterie;
        private string? brand;
        private string? calitate;

        public float Value { get { return valoare; } set { valoare = value; } }
        public float Consum { get { return consum; } set { consum = value; } }
        public float Baterie { get { return baterie; } set { baterie = value; } }
        public string? Brand { get { return brand; } set { brand = value; } }
        public string? Calitate { get { return calitate; } set { calitate = value; } }

        public Product(float valoare, float consum, float baterie, string? brand, string? calitate)
        {
            this.valoare = valoare;
            this.consum = consum;
            this.baterie = baterie;
            this.brand = brand;
            this.calitate = calitate;
        }

        public float VandProdus()
        {
            return valoare;
        }

        public virtual void TestezProdus()
        {
            float raport = baterie / consum;

            if (raport > 0 && raport <= 5)
            {
                calitate = "E";
            }
            else if (raport > 5 && raport <= 8)
            {
                calitate = "D";
            }
            else if (raport > 8 && raport <= 10)
            {
                calitate = "C";
            }
            else if (raport > 10 && raport <= 12)
            {
                calitate = "B";
            }
            else if ( raport > 12 && raport < 15)
            {
                calitate = "A";
            }
        }
    }

    internal class Phones : Product
    {
        private int anProductie;
        private string? procesor;

        public int AnProductie { get { return anProductie; } set { anProductie = value; } }
        public string? Procesor { get { return procesor; } set { procesor = value; } }

        public Phones(float valoare, float consum, float baterie, string? brand, string? calitate, int anProductie, string? procesor) : base(valoare, consum, baterie, brand, calitate)
        {
            this.anProductie = anProductie;
            this.procesor = procesor;
        }

        public override void TestezProdus()
        {
            float raport = Baterie / Consum;

            if (raport > 0 && raport <= 5)
            {
                Calitate = "E";
            }
            else if (raport > 5 && raport <= 8)
            {
                Calitate = "D";
            }
            else if (raport > 8 && raport <= 10)
            {
                Calitate = "C";
            }
            else if (raport > 10 && raport <= 12)
            {
                Calitate = "B";
            }
            else if (raport > 12 && raport < 15)
            {
                Calitate = "A";
            }

            Console.WriteLine($"Telefonul este de calitate de tipul: {Calitate}");
        }

        public void Start()
        {
            Console.WriteLine("Telefonul a pornit si poate efectua apeluri!");
        }

        public void Stop()
        {
            Console.WriteLine("Telefonul s-a oprit si orice contact este oprit!");
        }
    }

    internal class Tablets : Product
    {
        private int anProductie;
        private string? procesor;

        public int AnProductie { get { return anProductie; } set { anProductie = value; } }
        public string? Procesor { get { return procesor; } set { procesor = value; } }

        public Tablets(float valoare, float consum, float baterie, string? brand, string? calitate, int anProductie, string? procesor) : base(valoare, consum, baterie, brand, calitate)
        {
            this.anProductie = anProductie;
            this.procesor = procesor;
        }

        public override void TestezProdus()
        {
            float raport = Baterie / Consum;

            if (raport > 0 && raport <= 6)
            {
                Calitate = "E";
            }
            else if (raport > 6 && raport <= 9)
            {
                Calitate = "D";
            }
            else if (raport > 9 && raport <= 12)
            {
                Calitate = "C";
            }
            else if (raport > 12 && raport <= 15)
            {
                Calitate = "B";
            }
            else if (raport > 15 && raport < 24)
            {
                Calitate = "A";
            }

            Console.WriteLine($"Tableta este de calitate de tipul: {Calitate}");
        }

        public void Start()
        {
            Console.WriteLine("Tableta a pornit si poate efectua sarcini!");
        }

        public void Stop()
        {
            Console.WriteLine("Tableta a fost oprita si orice sarcina este nefunctionala!");
        }
    }

    internal class Computers : Product
    {
        private int anProductie;
        private int greutate;
        private string? procesor;

        public int AnProductie { get { return anProductie; } set { anProductie = value; } }
        public int Greutate { get { return greutate; } set { greutate = value; } }
        public string? Procesor { get { return procesor; } set { procesor = value; } }

        public Computers(float valoare, float consum, float baterie, string? brand, string? calitate, int anProductie, string? procesor, int greutate) : base(valoare, consum, baterie, brand, calitate)
        {
            this.anProductie = anProductie;
            this.procesor = procesor;
            this.greutate = greutate;
        }

        public override void TestezProdus()
        {
            float raport = Baterie / Consum;

            if (raport > 0 && raport <= 7)
            {
                Calitate = "E";
            }
            else if (raport > 7 && raport <= 10)
            {
                Calitate = "D";
            }
            else if (raport > 10 && raport <= 14)
            {
                Calitate = "C";
            }
            else if (raport > 14 && raport <= 20)
            {
                Calitate = "B";
            }
            else if (raport > 20 && raport < 36)
            {
                Calitate = "A";
            }

            Console.WriteLine($"Computerul este de calitate de tipul: {Calitate}");
        }

        public void Start()
        {
            Console.WriteLine("Computerul a pornit si poate efectua taskuri!");
        }

        public void Stop()
        {
            Console.WriteLine("Computerul s-a oprit si orice task este oprit!");
        }

        public void Search()
        {
            Console.WriteLine("Computerul poate efectua cautari!");
        }
    }

    internal class Headphones : Product
    {
        private int anProductie;
        private int putere;

        public int AnProductie { get { return anProductie; } set { anProductie = value; } }
        public int Putere { get { return putere; } set { putere = value; } }

        public Headphones(float valoare, float consum, float baterie, string? brand, string? calitate, int anProductie, int putere) : base(valoare, consum, baterie, brand, calitate)
        {
            this.anProductie = anProductie;
            this.putere = putere;
        }

        public override void TestezProdus()
        {
            float sunet = CalitateSunet();
            float raport = (Baterie / Consum) - sunet;

            if (raport > 0 && raport <= 5)
            {
                Calitate = "E";
            }
            else if (raport > 5 && raport <= 8)
            {
                Calitate = "D";
            }
            else if (raport > 8 && raport <= 10)
            {
                Calitate = "C";
            }
            else if (raport > 10 && raport <= 12)
            {
                Calitate = "B";
            }
            else if (raport > 12 && raport < 15)
            {
                Calitate = "A";
            }

            Console.WriteLine($"Telefonul este de calitate de tipul: {Calitate}");
        }

        public float CalitateSunet()
        {
            return Putere / Consum;
        }
    }
}