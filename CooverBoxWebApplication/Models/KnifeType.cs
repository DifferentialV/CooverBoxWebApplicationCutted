using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Models
{
    public class KnifeType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Knife> Knifes { get; set; }
        public KnifeType()
        {
            Knifes = new List<Knife>();
        }
    }
}
