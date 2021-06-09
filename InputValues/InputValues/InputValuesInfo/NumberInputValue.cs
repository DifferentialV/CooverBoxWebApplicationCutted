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
    public class NumberInputValue : BaseInputValue
    {
        public NumberInputValue(System.Reflection.PropertyInfo valueInfo, string displayName) : base(valueInfo, displayName)
        {
            if(new[] {typeof(double), typeof(int), typeof(double?), typeof(int?) }.Contains(valueInfo?.PropertyType) == false)
            {
                throw new Exception($"{displayName} valueInfo должен указывать на поля типа double или int");
            }
        }
        public double? Min { get; set; }
        public double? Max { get; set; }
        public string Placeholder { get; set; }
        public override void Bind(ModelBindingContext bindingContext, DBAppContext dbContext, MaterialsService materials)
        {
            if (string.IsNullOrEmpty(Roll) is false && bindingContext.HttpContext.User?.IsInRole(Roll) is false)
                return;
            string text = bindingContext.ValueProvider.GetValue(Name).FirstValue?.Trim();
            double? result = null;
            if(double.TryParse(text, out double outresult))
            {
                result = outresult;
            }
            if(result == null)
            {
                if(Required == true)
                    bindingContext.ModelState.AddModelError(string.Empty, $"Поле {DisplayName} не удалось считать.");
                if (new[] { typeof(double?), typeof(int?) }.Contains(ValueType) == true)
                    SetValue(bindingContext.Model, result);
                else
                    SetValue(bindingContext.Model, 0);
                return;
            }
            if (typeof(int) == ValueType)
                result =  (int)result.Value;
            if(Min != null && result < Min)
                bindingContext.ModelState.AddModelError(string.Empty, $"Поле {DisplayName} должно быть больше или равно {Min}");

            if (Max != null && result > Max)
                bindingContext.ModelState.AddModelError(string.Empty, $"Поле {DisplayName} должно быть меньше или равно {Max}");

            if (typeof(int) == ValueType) SetValue(bindingContext.Model, (int)result.Value);
            else if (typeof(int?) == ValueType) SetValue(bindingContext.Model, (int?)result.Value);
            else if (typeof(double?) == ValueType) SetValue(bindingContext.Model, result);
            else SetValue(bindingContext.Model, result.Value);

        }
    }
}
