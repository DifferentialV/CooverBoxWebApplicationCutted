using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.ViewModels
{
    public class AddKnifeViewModel
    {
        [Required(ErrorMessage = "Не указано Примечание")]
        [Display(Name = "Введите Примечание", Prompt = "Примечание")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Выберите Тип")]
        [Display(Name = "Выберите Тип")]
        public string KnifeType { get; set; }
        [Range(1,1000,ErrorMessage = "X в пределах[1;1000]")]
        [Display(Name = "X")]
        public double X { get; set; }
        [Range(1, 1000, ErrorMessage = "Y в пределах[1;1000]")]
        [Display(Name = "Y")] 
        public double Y { get; set; }
        [Range(1, 1000, ErrorMessage = "H в пределах[1;1000]")]
        [Display(Name = "H")]
        public double H { get; set; }
        [Range(0.0005, 5, ErrorMessage = "T в пределах(0;5]")]
        [Display(Name = "Толщина картона")]
        public double T { get; set; }
        [Required(ErrorMessage = "Не указано Номер штанцформы")]
        [Display(Name = "Введите Номер штанцформы", Prompt = "Номер штанцформы")]
        public string NumberKnife { get; set; }
    }
}
