using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Models
{
    //изолон
    public class Isolon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Thickness { get; set; }
        public string Type { get; set; }
        public bool Roll { get; set; }
        public double Price { get; set; }

        public override string ToString()
        {
            return $"{Name} {Color} {Thickness}мм.";
        }


    }
}
