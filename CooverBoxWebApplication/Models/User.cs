using CooverBoxWebApplication.Models.WorkRecord;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Models
{
    public class User : IdentityUser
    {
        public List<BoxOrder> BoxOrders { get; set; }
        public List<BoxDBData> BoxSaved { get; set; }
        public List<UsersAndDepartnments> UsersAndDepartnments { get;set;}
        public User()
        {
            BoxSaved = new List<BoxDBData>();
            BoxOrders = new List<BoxOrder>();
            UsersAndDepartnments = new List<UsersAndDepartnments>();
        }
    }
}
