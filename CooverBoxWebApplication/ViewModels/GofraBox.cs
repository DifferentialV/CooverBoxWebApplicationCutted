using CooverBoxWebApplication.Infrastructure;
using CooverBoxWebApplication.Models;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace CooverBoxWebApplication.ViewModels
{
    public class PostPackageViewModel
    {
        [Required]
        [Range(10,1200)]
        [Display(Name = "Длина, мм")]
        public double X { get; set; }
        [Required]
        [Range(10, 1200)]
        [Display(Name = "Ширина, мм")]
        public double Y { get; set; }
        [Required]
        [Range(10, 1200)]
        [Display(Name = "Высота, мм")]
        public double H { get; set; }

        [Required]
        [Range(10, 10000)]
        [Display(Name = "Вес, гр")]
        public double Weight { get; set; }

        [Required]
        [Range(1, 100000000)]
        [Display(Name = "Тираж")]
        public int N { get; set; }
    }
}
