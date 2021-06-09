using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Models
{
    //ленты
    public class Ribbon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Color { get; set; }
        public double Width { get; set; }
        public double Price { get; set; }

        public override string ToString()
        {
            return $"{Name} {Type} {Color} {Width}мм.";
        }
    }
}
