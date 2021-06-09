using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Models
{
    //бумага тишью
    public class TissuePaper
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public double Price { get; set; }

        public override string ToString()
        {
            return $"{Name} {Color}";
        }
    }
}
