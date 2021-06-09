using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CooverBoxWebApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using CooverBoxWebApplication.Models.Boxes;
using Microsoft.Extensions.Primitives;
using CooverBoxWebApplication.ViewModels;
using CooverBoxWebApplication.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CooverBoxWebApplication.Controllers
{
    [Route("[controller]")]
    [Authorize(Roles = "master, admin, user")]
    public class BoxesController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private DBAppContext _context;

        public BoxesController(IWebHostEnvironment webHostEnvironment, DBAppContext context, ILogger<BoxesController> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }
        //Id может соотвествовать имении класса (тогда создаться новая модель по умолчанию) или имени файла (модель будет загружена из файла)
        //Id уникальный для каждого файла, и является частью строки запроса
        [HttpGet("{Id}")]
        public IActionResult Index(string Id)
        {
            BaseBox model = _context.GetDBBox(Id);
            if (model != null)
            {
                //если пользователь который открыл модел не является её создателем или мастером, сбрасывается Id модели, чтобы в при переходе дальше создаласть копия
                if(model.UserName != User.Identity.Name && !User.IsInRole("master")) model.Id = $"{model.GetType().Name}";
                return View(model);
            }
            return RedirectToAction("FunError", "Home");
        }
        [HttpPost("{Id}")]
        public IActionResult Index(string Id, BaseBox model)
        {
            if (!ModelState.IsValid)
            {
                model.Id = Id;
                return View(model);
            }
            if (model == null) return RedirectToAction("FunError", "Home");
            if(model.Id == null || model.Id == $"{model.GetType().Name}")
            {
                if(Id == $"{model.GetType().Name}")
                    model.Id = $"{Guid.NewGuid()}";
                else
                    model.Id = Id;
            }
            //задает создателя
            if (string.IsNullOrEmpty(model.UserName)) model.UserName = User?.Identity?.Name;
            //если Identity не содежит имя пользователя
            if (string.IsNullOrEmpty(model.UserName)) { model.UserName = "erroruser"; }
            _context.SaveDBBox(model);

#if RELEASE
            {
                BoxDBDataTest boxDB = _context.BoxesTestSaved.FirstOrDefault(b => b.Id == model.Id);
                if (boxDB is null)
                {
                    boxDB = new BoxDBDataTest()
                    {
                        Id = model.Id,
                        FullName = model.FullName,
                        BitrixLink = model.BitrixLink,
                        TypeBox = model.TypeBoxe,
                        UserId = _context.Users.First(u => u.UserName == User.Identity.Name).Id,
                        DateCreated = DateTime.Now
                    };
                    _context.BoxesTestSaved.Add(boxDB);
                }
                else
                {
                    boxDB.FullName = model.FullName;
                    boxDB.DateCreated = DateTime.Now;
                }
                _context.SaveChanges();
            }
#endif
            Id = model.Id;
            if (model == null) return RedirectToAction("FunError", "Home");
            return RedirectToAction($"Preview", routeValues: new { Id });
        }
        //создает клона модели, просто стирает Id и вписывает нового создателя
        //НИКТО НЕ ПОЛЬЗУЕТСЯ
        [HttpGet("Clone/{Id}")]
        public IActionResult Clone(string Id)
        {
            BaseBox model = _context.GetDBBox(Id);
            if (model != null)
            {
                model.UserName = User?.Identity?.Name;
                if (string.IsNullOrEmpty(model.UserName)) { model.UserName = "erroruser"; }
                model.Id = $"{model.GetType().Name}";
                return View("~/Views/Boxes/Index.cshtml", model);
            }
            return RedirectToAction("FunError", "Home");
        }
        //выводит станицу с параметрами модели и себестоимостью
        [HttpGet("Preview/{Id}")]
        public IActionResult Preview(string Id)
        {
            BaseBox model = _context.GetDBBox(Id);
            if (model == null) return RedirectToAction("FunError", "Home");
            return View(model);
        }
        //страница Preview выводит только параметры без себестоимости
        //себестоимоть выводится через ajax запрос к методу ниже
        //если ajax-у вернулся error то пытается вызвать этот метод, который выводит всё сразу
        [HttpGet("PreviewAJAXERROR/{Id}")]
        public IActionResult PreviewAJAXERROR(string Id)
        {
            BaseBox model = _context.GetDBBox(Id);
            if (model == null) return RedirectToAction("FunError", "Home");
            return View(model);
        }
        //метод для обработки ajax запроса со страницы Preview
        [HttpGet("CostViewJquery")]
        public IActionResult CostViewJquery(string Id)
        {
            BaseBox model = _context.GetDBBox(Id);

            if (model == null) return RedirectToAction("FunError", "Home");
            model.Create();

            return View(model);
        }
        
        //сохранить в личный список заказов
        [HttpGet("Save/{Id}")]
        public IActionResult Save(string Id)
        {
            BaseBox model = _context.GetDBBox(Id);
            if (model == null) return RedirectToAction("FunError", "Home");
            model.Id = $"{System.Guid.NewGuid()}";
            _context.SaveDBBox(model);
            BoxDBData boxDB = new BoxDBData()
            {
                Id = model.Id,
                FullName = model.FullName,
                BitrixLink = model.BitrixLink,
                TypeBox = model.TypeBoxe,
                UserId = _context.Users.First(u => u.UserName == User.Identity.Name).Id,
                DateCreated = DateTime.Now
            };
            _context.BoxesSaved.Add(boxDB);
            _context.SaveChanges();
            return RedirectToAction($"Index", "JSONBox");
        }
        //скачать чертежи, раскрой, упаковку
        [HttpGet("Download/{Id}")]
        public FileResult Download(string Id)
        {
            string Path = $"{_webHostEnvironment.WebRootPath}\\FileTemp\\{System.Guid.NewGuid()}";
            BaseBox model = _context.GetDBBox(Id);
            if (model == null) return null;
            if (Directory.CreateDirectory(Path) != null)
            {
                model.Create(Path);
                ZipFile.CreateFromDirectory(Path, $"{Path}.zip");
                Task.Run(() => DeleteCreationFiles(Path));
                System.IO.FileStream fs = new System.IO.FileStream($"{Path}.zip", System.IO.FileMode.Open);
                return File(fs, "application/octet-stream", $"{model.FullName}.zip");
            }
            return null;
            //удаляет созданую папку и архив через минутя 
            static void DeleteCreationFiles(string Path)
            {
                Thread.Sleep(TimeSpan.FromMinutes(1));
                Directory.Delete(Path, true);
                System.IO.File.Delete($"{Path}.zip");
            }
        }
        //скачать крой для дизайнеров
        [HttpGet("DownloadDisign/{Id}")]
        public FileResult DownloadDisign(string Id)
        {
            string Path = $"{_webHostEnvironment.WebRootPath}\\FileTemp\\{System.Guid.NewGuid()}.pdf";
            BaseBox model = _context.GetDBBox(Id);
            if (model == null) return null;
            string PathConst = $"{_webHostEnvironment.WebRootPath}\\BoxesDisigns\\{model.GetType().Name}.pdf";

            model.CreateDisignPDF(PathConst, Path);
            Task.Run(() => DeleteCreationFile(Path));
            System.IO.FileStream fs = new System.IO.FileStream($"{Path}", System.IO.FileMode.Open);
            return File(fs, "application/pdf", $"{model.FullName}.pdf");

            static void DeleteCreationFile(string Path)
            {
                Thread.Sleep(TimeSpan.FromMinutes(1));
                System.IO.File.Delete($"{Path}");
            }

        }


        //создать сделку, запрос для получения ссылки на битрикс
        [HttpGet("DownloadOrder/{Id}")]
        public IActionResult DownloadOrder(string Id)
        {
            BaseBox model = _context.GetDBBox(Id);
            if (model == null) return RedirectToAction("FunError", "Home");

            ViewBag.BoxId = Id;
            ViewBag.BoxName = model.FullName;
            ViewBag.BitrixLink = model.BitrixLink;
            return View("~/Views/Boxes/DownloadOrder.cshtml");
        }
        [HttpPost("DownloadOrder/{Id}")]
        public IActionResult DownloadOrder(string Id, string BitrixLink)
        {
            BaseBox model = _context.GetDBBox(Id);
            if (model == null) return null;
            if (string.IsNullOrEmpty(BitrixLink) || !Uri.IsWellFormedUriString(BitrixLink, UriKind.Absolute) || !(new[] { "bitrix24", "likepackage" }.Any(s => BitrixLink.Contains(s) is true)))
            {
                ModelState.AddModelError("", "ссылка на bitrix24 обязательна");
                ViewBag.BoxId = Id;
                ViewBag.BoxName = model.FullName;
                return View("~/Views/Boxes/DownloadOrder.cshtml");
            }
            model.BitrixLink = BitrixLink;
            model.Create();
            _context.SaveDBBox(model);

            model.UserName = null;
            model.Id = $"{Guid.NewGuid()}";
            _context.SaveDBBox(model);
            BoxOrder order = new BoxOrder()
            {
                FullName = model.FullName,
                BoxId = model.Id,
                TypeBox = model.TypeBoxe,
                BitrixLink = model.BitrixLink,
                DateCreated = DateTime.Now,
                DateShipment = model.DateOut,
                UserId = _context.Users.First(u => u.UserName == User.Identity.Name).Id
            };
            _context.BoxOrders.Add(order);
            _context.SaveChanges();

            //создает jpg на основе html
            string HtmlString = RenderRazorViewToString(this, "BoxToPngView", model, order.Id);
            var converter = new CoreHtmlToImage.HtmlConverter();
            var htmlBytes = converter.FromHtmlString(HtmlString, format: CoreHtmlToImage.ImageFormat.Jpg, width: 960);
            return File(htmlBytes, "application/Jpg", $"{model.FullName}.Jpg");

            static string RenderRazorViewToString(Controller controller, string viewName, BaseBox model, int IdBoxFile)
            {
                controller.ViewBag.ModelBox = model;
                controller.ViewBag.BoxFileId = IdBoxFile;
                var sw = new StringWriter();
                Microsoft.AspNetCore.Mvc.ViewEngines.ViewEngineResult viewResult = null;
                var engine = controller.HttpContext.RequestServices.GetService(typeof(Microsoft.AspNetCore.Mvc.ViewEngines.ICompositeViewEngine)) as Microsoft.AspNetCore.Mvc.ViewEngines.ICompositeViewEngine;
                viewResult = engine.FindView(controller.ControllerContext, viewName, true);
                var viewContext = new Microsoft.AspNetCore.Mvc.Rendering.ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw, new Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelperOptions());
                viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
