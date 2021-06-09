using CooverBoxWebApplication.Models.WorkRecord;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.ViewModels
{
    public class ChangeRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<IdentityRole> AllRoles { get; set; }
        public IList<string> UserRoles { get; set; }
        public List<Department> AllDepartments { get; set; }
        public List<int> UserDepartments { get; set; }
        public ChangeRoleViewModel()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
            AllDepartments = new List<Department>();
            UserDepartments = new List<int>();
        }
    }
}
