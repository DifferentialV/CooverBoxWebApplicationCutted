using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Models
{
    public class BoxDBData
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string TypeBox { get; set; }
        public string BitrixLink { get; set; }
        public User User { get; set; }
        public DateTime DateCreated { get; set; }

    }

    /*
     Id	nvarchar(450)	Unchecked
UserId	nvarchar(450)	Checked
FullName	nvarchar(MAX)	Checked
TypeBox	nvarchar(MAX)	Checked
BitrixLink	nvarchar(MAX)	Checked
DateCreated	datetime2(7)	Checked
     */
}
