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
    public class RubberInputValue : BaseInputValue
    {
        public string AddOption { get; set; }
        public bool AlwaysDefault { get; set; }
        public RubberInputValue(System.Reflection.PropertyInfo valueInfo, string displayName) : base(valueInfo, displayName)
        {
            if (new[] { typeof(Rubber) }.Contains(valueInfo?.PropertyType) == false)
            {
                throw new Exception($"{displayName} valueInfo должен указывать на поля типа Rubber");
            }
        }

        public override void Bind(ModelBindingContext bindingContext, DBAppContext dbContext, MaterialsService materials)
        {
            if (string.IsNullOrEmpty(Roll) is false && bindingContext.HttpContext.User?.IsInRole(Roll) is false)
                return;
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
            Rubber result = materials.Rubbers.FirstOrDefault(m => m.Id == resultId.Value);
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