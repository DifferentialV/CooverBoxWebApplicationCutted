using CooverBoxWebApplication.InputValues.Base;
using CooverBoxWebApplication.Models;
using CooverBoxWebApplication.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.InputValues.InputValuesInfo
{
    public class PaperInputValue : BaseInputValue
    {
        public string AddOption { get; set; }
        public bool AlwaysDefault { get; set; }
        public string TypeUsing { get; set; }

        public bool UserInput { get; set; }
        public string SelectForAllMain { get; set; }
        public string SelectForAll { get; set; }
        public PaperInputValue(System.Reflection.PropertyInfo valueInfo, string displayName) : base(valueInfo, displayName)
        {
            if (new[] { typeof(DesignPaper) }.Contains(valueInfo?.PropertyType) == false)
            {
                throw new Exception($"{displayName} valueInfo должен указывать на поля типа DesignPaper");
            }
        }

        public override void Bind(ModelBindingContext bindingContext, DBAppContext dbContext, MaterialsService materials)
        {
            if (string.IsNullOrEmpty(Roll) is false && bindingContext.HttpContext.User?.IsInRole(Roll) is false)
                return;
            if (UserInput == true && bindingContext.ValueProvider.GetValue($"status_{Name}").FirstValue?.Length > 0)
            {
                string name = null;
                double price;
                try
                {
                    name = bindingContext.ValueProvider.GetValue($"userpaper_name_{Name}").FirstValue?.Trim();
                    price = double.Parse(bindingContext.ValueProvider.GetValue($"userpaper_price_{Name}").FirstValue?.Trim());
                }
                catch
                {
                    bindingContext.ModelState.AddModelError(string.Empty, $"Поле {DisplayName} не удалось считать.");
                    return;
                }
                DesignPaper paper = new DesignPaper() { Name = name, X = 700, Y = 1100, Density = TypeUsing == PaperTypeUsing.Sticker? 150:350, Price = price, Thickness = 0.4, VolocnoX = false, FullColor = false };
                SetValue(bindingContext.Model, paper);
                return;
            }
            string text = bindingContext.ValueProvider.GetValue(Name).FirstValue?.Trim();
            int? resultId = null;
            if (int.TryParse(text, out int outresult))
            {
                resultId = outresult;
            }
            if (resultId == null)
            {
                if (Required == true && string.IsNullOrEmpty(AddOption))
                    bindingContext.ModelState.AddModelError(string.Empty, $"Поле {DisplayName} не удалось считать.");
                SetValue(bindingContext.Model, null);
                return;
            }
            DesignPaper result = materials.DesignPapers.FirstOrDefault(m => m.Id == resultId.Value);
            if (result == null)
            {
                if (Required == true && string.IsNullOrEmpty(AddOption))
                    bindingContext.ModelState.AddModelError(string.Empty, $"Поле {DisplayName} не удалось считать.");
                SetValue(bindingContext.Model, null);
                return;
            }
            SetValue(bindingContext.Model, result);
        }
    }
}