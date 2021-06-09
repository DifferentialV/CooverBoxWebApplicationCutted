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
    public class DateTimeInputValue : BaseInputValue
    {
        public string DateTimeType { get; set; } = DateTimeTypes.DateTime;


        public static class DateTimeTypes
        {
            public const string DateTime = "date-time";
            public const string Date = "date";
            public const string Time = "time";
        }

        public DateTimeInputValue(System.Reflection.PropertyInfo valueInfo, string displayName) : base(valueInfo, displayName)
        {
            if (new[] { typeof(DateTime) }.Contains(valueInfo?.PropertyType) == false)
            {
                throw new Exception($"{displayName} valueInfo должен указывать на поля типа DateTime");
            }
        }
        public override void Bind(ModelBindingContext bindingContext, DBAppContext dbContext, MaterialsService materials)
        {
            if (string.IsNullOrEmpty(Roll) is false && bindingContext.HttpContext.User?.IsInRole(Roll) is false)
                return;
            string date = bindingContext.ValueProvider.GetValue(Name).FirstValue?.Trim();
            DateTime? result = null;
            if (DateTime.TryParse(date, out DateTime outresult))
            {
                result = outresult;
            }
            if (result == null)
            {
                if (Required == true)
                    bindingContext.ModelState.AddModelError(string.Empty, $"Поле {DisplayName} не удалось считать.");
                return;
            }
            SetValue(bindingContext.Model, result.Value);

        }
    }
}
