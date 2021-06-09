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
    public class TextInputValue : BaseInputValue
    {
        public TextInputValue(System.Reflection.PropertyInfo valueInfo, string displayName) : base(valueInfo, displayName) 
        {
            if (new[] { typeof(string) }.Contains(valueInfo?.PropertyType) == false)
            {
                throw new Exception($"{displayName} valueInfo должен указывать на поля типа string");
            }
        }

        public string Placeholder { get; set; }
        public int? LengthMin { get; set; }
        public int? LengthMax { get; set; }
        public string RegularExpression { get; set; }

        public override void Bind(ModelBindingContext bindingContext,DBAppContext dbContext, MaterialsService materials)
        {
            if (string.IsNullOrEmpty(Roll) is false && bindingContext.HttpContext.User?.IsInRole(Roll) is false)
                return;
            string result = bindingContext.ValueProvider.GetValue(Name).FirstValue?.Trim();
            if (string.IsNullOrEmpty(result))
            {
                if (Required == true)
                    bindingContext.ModelState.AddModelError(string.Empty, $"Поле {DisplayName} обязательно.");
                SetValue(bindingContext.Model, null);
                return;
            }
            if (LengthMin != null && result.Length < LengthMin)
                bindingContext.ModelState.AddModelError(string.Empty, $"Поле {DisplayName} должно быть длиной больше или равно {LengthMin}");

            if (LengthMax != null && result.Length > LengthMax)
                bindingContext.ModelState.AddModelError(string.Empty, $"Поле {DisplayName} должно быть длиной меньше или равно {LengthMax}");
            
            
            SetValue(bindingContext.Model, result);

        }
    }
}
