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
    public class PaperWithPrintInputValue : BaseInputValue
    {
        public string AddOption { get; set; }
        public bool AlwaysDefault { get; set; }
        public string TypeUsing { get; set; }

        public bool UserInput { get; set; }
        public string SelectForAllMain { get; set; }
        public string SelectForAll { get; set; }
        public PaperWithPrintInputValue(System.Reflection.PropertyInfo valueInfo, string displayName) : base(valueInfo, displayName)
        {
            if (new[] { typeof(PaperWithPrint) }.Contains(valueInfo?.PropertyType) == false)
            {
                throw new Exception($"{displayName} valueInfo должен указывать на поля типа PaperWithPrint");
            }
        }

        public override void Bind(ModelBindingContext bindingContext, DBAppContext dbContext, MaterialsService materials)
        {
            if (string.IsNullOrEmpty(Roll) is false && bindingContext.HttpContext.User?.IsInRole(Roll) is false)
                return;
            DesignPaper resultPaper = null;
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
                resultPaper = new DesignPaper() { Name = name, X = 700, Y = 1100, Density = TypeUsing == PaperTypeUsing.Sticker ? 150 : 350, Price = price, Thickness = 0.4, VolocnoX = false, FullColor = false };
            }
            else
            {
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
                resultPaper = materials.DesignPapers.FirstOrDefault(m => m.Id == resultId.Value);
            }
            if (resultPaper == null)
            {
                if (Required == true && string.IsNullOrEmpty(AddOption))
                    bindingContext.ModelState.AddModelError(string.Empty, $"Поле {DisplayName} не удалось считать.");
                SetValue(bindingContext.Model, null);
                return;
            }
            PaperWithPrint result = new PaperWithPrint() { Paper = resultPaper };
            if (bindingContext.ValueProvider.GetValue($"silk_count_{Name}").FirstValue?.Length > 0)
            {
                try
                {
                    int SilkX = int.Parse(bindingContext.ValueProvider.GetValue($"silk_x_{Name}").FirstValue?.Trim());
                    int SilkY = int.Parse(bindingContext.ValueProvider.GetValue($"silk_y_{Name}").FirstValue?.Trim());
                    int SilkWork = int.Parse(bindingContext.ValueProvider.GetValue($"silk_count_{Name}").FirstValue?.Trim());
                    result.SilkX = SilkX;
                    result.SilkY = SilkY;
                    result.SilkWork = SilkWork;
                }
                catch
                {
                    bindingContext.ModelState.AddModelError(string.Empty, $"Поле Шелк-{DisplayName} не удалось считать. (все поля должны быть заполнены)");
                }
            }
            if (bindingContext.ValueProvider.GetValue($"clishe_count_{Name}").FirstValue?.Length > 0)
            {
                try
                {
                    int ClicheX = int.Parse(bindingContext.ValueProvider.GetValue($"clishe_x_{Name}").FirstValue?.Trim());
                    int ClicheY = int.Parse(bindingContext.ValueProvider.GetValue($"clishe_y_{Name}").FirstValue?.Trim());
                    int ClicheWork = int.Parse(bindingContext.ValueProvider.GetValue($"clishe_count_{Name}").FirstValue?.Trim());
                    result.ClicheX = ClicheX;
                    result.ClicheY = ClicheY;
                    result.ClicheWork = ClicheWork;
                }
                catch
                {
                    bindingContext.ModelState.AddModelError(string.Empty, $"Поле Тиснение-{DisplayName} не удалось считать. (все поля должны быть заполнены)");
                }
            }
            SetValue(bindingContext.Model, result);
        }
    }
}