using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CooverBoxWebApplication.Models;
using CooverBoxWebApplication.Models.Boxes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CooverBoxWebApplication.Controllers
{
    [Authorize(Roles = "admin, user")]
    public class JSONBoxController : Controller
    {
        DBAppContext _context;

        public JSONBoxController(DBAppContext context)
        {
            _context = context;
        }
        //персональный список сохраненных заказов
        public IActionResult Index()
        {
            ViewBag.BoxesSaved = _context.BoxesSaved.Include(p => p.User).Where(b => b.User.UserName == User.Identity.Name).OrderByDescending(f =>f.DateCreated);
            return View();
        }
        //удалить заказ
        [HttpGet]
        [Route("[controller]/Delete/{Id}")]
        public IActionResult Delete(string Id)
        {
            BoxDBData order = _context.BoxesSaved.Find(Id);
            _context.BoxesSaved.Remove(order);
            _context.SaveChanges();
            return RedirectToAction("Index", "JSONBox");
        }
        //список всех созданых коробочек
        [Authorize(Roles = "master")]
        public IActionResult TextFileView()
        {
            ViewBag.BoxesTestSaved = _context.BoxesTestSaved.Include(p => p.User).OrderByDescending(f => f.DateCreated);
            return View();
        }
    }
}
