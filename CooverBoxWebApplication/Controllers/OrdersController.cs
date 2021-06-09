using CooverBoxWebApplication.Models;
using CooverBoxWebApplication.Models.Boxes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CooverBoxWebApplication.Controllers
{
    //список сделок, и выборка по условию
    [Authorize(Roles = "master")]
    public class OrdersController : Controller
    {
        DBAppContext _context;

        public OrdersController(DBAppContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            ViewBag.BoxOrders = _context.BoxOrders.Include(p => p.User).OrderByDescending(f => f.DateCreated);
            return View();
        }
        [HttpGet]
        [Route("[controller]/Open/{OrderId}")]
        public IActionResult Open(int OrderId)
        {
            BoxOrder order = _context.BoxOrders.Find(OrderId);
            if(!order.Opening)
            {
                order.Opening = true;
                _context.SaveChanges();
            }
            string Id = order.BoxId;
            return RedirectToAction($"Preview", "Boxes",routeValues: new { Id });
        }
        [HttpGet]
        [Route("[controller]/Chosen/{fileID}")]
        public IActionResult Chosen(int fileID)
        {
            var order = _context.BoxOrders.Find(fileID);
            order.Chosen = !order.Chosen;
            _context.SaveChanges();
            return RedirectToAction($"Index");
        }
        [HttpGet]
        [Route("[controller]/Complete/{fileID}")]
        public IActionResult Complete(int fileID)
        {
            var order = _context.BoxOrders.Find(fileID);
            order.Complete = !order.Complete;
            _context.SaveChanges();
            return RedirectToAction($"Index");
        }
        [HttpGet]
        //удалить те которым больше 31 дня и они не открывались
        [Route("[controller]/RemoveTrash")]
        public IActionResult RemoveTrash()
        {
            foreach(var order in _context.BoxOrders.Where(o => o.DateCreated < DateTime.Now.AddDays(-31)))
            {
                _context.BoxOrders.Remove(order);
            }
            _context.SaveChanges();
            return RedirectToAction($"Index");
        }
    }
}
