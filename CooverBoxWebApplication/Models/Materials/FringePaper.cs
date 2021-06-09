using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Models
{
    public class FringePaper
    {
        //бумажная бахрома
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Density { get; set; }
        public double Price { get; set; }

        public override string ToString()
        {
            return $"{Name} {Color}";
        }
    }
}
