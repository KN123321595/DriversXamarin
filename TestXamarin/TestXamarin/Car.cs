using System;
using System.Collections.Generic;
using System.Text;

namespace TestXamarin
{
    public class Car
    {
        public int id { get; set; }
        public string number { get; set; }
        public int mileage { get; set; }

        public Car(int id, string number, int mileage)
        {
            this.id = id;
            this.number = number;
            this.mileage = mileage;
        }
    }
}
