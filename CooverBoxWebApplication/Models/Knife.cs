using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Models
{
    public class Knife
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? KnifeTypeId { get; set; }
        public KnifeType KnifeType { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double H { get; set; }
        public double T { get; set; }
        public string NumberKnife { get; set; }
    }
}
