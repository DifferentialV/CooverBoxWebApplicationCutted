using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CooverBoxWebApplication.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PDFCore;
using PDFCore.Graphic;
using CooverBoxWebApplication.Models.Boxes.Helpers;

namespace CooverBoxWebApplication.Controllers
{
    //использовался для тестирования алгоритма упаковки изделий в коробочки
    public class PostPackageController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PostPackageController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(PostPackageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            PostLiker2 postLiker = new PostLiker2(model.X,model.Y,model.H,model.Weight,model.N);
            postLiker.Work();
            return View("PostLiker", postLiker);
        }

        [HttpPost]
        public FileResult Download(PostPackageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return null;
            }
            PostLiker2 postLiker = new PostLiker2(model.X, model.Y, model.H, model.Weight, model.N);
            string Path = $"{_webHostEnvironment.WebRootPath}\\FileTemp";
            string fullName = $"{ System.Guid.NewGuid() }";
            postLiker.Work(fullName,Path);
            Task.Run(() => DeleteCreationFiles($"{Path}\\Упаковка {fullName}.pdf"));
            System.IO.FileStream fs = new System.IO.FileStream($"{Path}\\Упаковка {fullName}.pdf", System.IO.FileMode.Open);
            return File(fs, "application/octet-stream", $"Упаковка {model.X}x{model.Y}x{model.H} -{model.N}.pdf");


            static void DeleteCreationFiles(string Path)
            {
                Thread.Sleep(TimeSpan.FromMinutes(1));
                System.IO.File.Delete($"{Path}.pdf");
            }
        }

    }
}
