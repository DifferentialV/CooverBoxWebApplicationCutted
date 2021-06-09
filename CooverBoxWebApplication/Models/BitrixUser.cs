using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Models
{
    public class BitrixUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ChatId { get; set; }
        public string BitrixId { get; set; }
        public string Token { get; set; }
    }
}
