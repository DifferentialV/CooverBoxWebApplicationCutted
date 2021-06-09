using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Departments { get; set; }
    }
}
