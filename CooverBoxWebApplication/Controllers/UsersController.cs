using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CooverBoxWebApplication.Models;
using CooverBoxWebApplication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Microsoft.EntityFrameworkCore;
using CooverBoxWebApplication.Models.WorkRecord;

namespace CooverBoxWebApplication.Controllers
{
    [Authorize(Roles = "master")]
    public class UsersController : Controller
    {
        UserManager<User> _userManager;
        RoleManager<IdentityRole> _roleManager;
        private DBAppContext _context;
        public UsersController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, DBAppContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            List<User> users = _userManager.Users.ToList();
            List<UserViewModel> usersview = new List<UserViewModel>();
            foreach(User var in users)
            {
                usersview.Add(new UserViewModel {UserName = var.UserName,Id = var.Id, Roles = (List<string>)await _userManager.GetRolesAsync(var),Departments = _context.Departments.Include(k=>k.UsersAndDepartnments).Where(d=>d.UsersAndDepartnments.Any(u=>u.UserId==var.Id)).Select(d=>d.Name).ToList()});
            }
            return View(usersview);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { UserName = model.UserName };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            User qw = await _userManager.FindByIdAsync(id);
            User user = await _userManager.FindByNameAsync(qw.UserName);
            if (user == null)
            {
                return NotFound();
            }
            string code = await _userManager.GeneratePasswordResetTokenAsync(user);
            return View(new Dictionary<string, string> { { "userId", $"{user.Id}" }, { "code", code } });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                var allDepartments = _context.Departments.ToList();
                var userDepartments = _context.Users.Include(k => k.UsersAndDepartnments).First(u=>u.Id==user.Id).UsersAndDepartnments.Where(ud =>ud.DepartmentId != null).Select(ud =>ud.DepartmentId.Value).ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    UserRoles = userRoles,
                    AllRoles = allRoles,
                    AllDepartments = allDepartments,
                    UserDepartments = userDepartments
                };
                return View(model);
            }

            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string id, List<string> roles,List<int> departnments)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);
                await _userManager.AddToRolesAsync(user, addedRoles);
                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                var ud = _context.Users.Include(k => k.UsersAndDepartnments).First(u =>u.Id == user.Id);
                ud.UsersAndDepartnments.RemoveAll(d => !departnments.Contains(d.Id));
                foreach(var d in departnments.Where(id => ud.UsersAndDepartnments.All(qw => qw.DepartmentId!=id)))
                {
                    ud.UsersAndDepartnments.Add(new UsersAndDepartnments() {UserId = ud.Id,DepartmentId = d }); 
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return NotFound();
        }

        [HttpGet]
        public IActionResult AddRoll()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddRoll(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim();
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("RollsList");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(name);
        }
        [HttpGet]
        public IActionResult RollsList()
        {
            return View(_roleManager.Roles.ToList());
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRoll(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction("RollsList");
        }
    }
}
