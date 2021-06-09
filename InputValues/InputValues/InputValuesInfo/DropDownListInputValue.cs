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
    public class DropDownListInputValue : BaseInputValue
    {
        public string Options { get; set; }

        public bool AlwaysDefault { get; set; }

        public DropDownListInputValue(System.Reflection.PropertyInfo valueInfo, string displayName) : base(valueInfo, displayName)
        {
            if (new[] { typeof(string) }.Contains(valueInfo?.PropertyType) == false)
            {
                throw new Exception($"{displayName} valueInfo должен указывать на поля типа string");
            }
        }

        public override void Bind(ModelBindingContext bindingContext, DBAppContext dbContext, MaterialsService materials)
        {
            if (string.IsNullOrEmpty(Roll) is false && bindingContext.HttpContext.User?.IsInRole(Roll) is false)
                return;
            string result = bindingContext.ValueProvider.GetValue(Name).FirstValue?.Trim();
            if (string.IsNullOrEmpty(result) && Required == true)
            {
                bindingContext.ModelState.AddModelError(string.Empty, $"Поле {DisplayName} обязательно.");
            }

            SetValue(bindingContext.Model, result);
        }
    }
}
