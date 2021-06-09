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
    public class EnumDropListInputValue<TEnum> : BaseInputValue
    {
        public string[] Options { get {return Enum.GetNames(typeof(TEnum)); } }

        public bool AlwaysDefault { get; set; }
        public bool DisplayNullVisible { get; set; } = false;
        public EnumDropListInputValue(System.Reflection.PropertyInfo valueInfo, string displayName) : base(valueInfo, displayName)
        {
            if(!typeof(Enum).IsAssignableFrom(typeof(TEnum)))
            {
                throw new Exception($"{displayName} valueInfo должен указывать на поля типа Enum");
            }
            if (new[] { typeof(TEnum) }.Contains(valueInfo?.PropertyType) == false)
            {
                throw new Exception($"{displayName} valueInfo должен указывать на поля типа {typeof(TEnum).Name}");
            }
        }

        public override void Bind(ModelBindingContext bindingContext, DBAppContext dbContext, MaterialsService materials)
        {
            if (string.IsNullOrEmpty(Roll) is false && bindingContext.HttpContext.User?.IsInRole(Roll) is false)
                return;
            string text = bindingContext.ValueProvider.GetValue(Name).FirstValue?.Trim();
            dynamic result = null;
            if (Enum.TryParse(typeof(TEnum), text, out dynamic outresult))
            {
                result = outresult;
            }
            if (result == null || result.GetType() != typeof(TEnum))
            {
                if (Required == true)
                    bindingContext.ModelState.AddModelError(string.Empty, $"Поле {DisplayName} не удалось считать.");
                SetValue(bindingContext.Model, null);
                return;
            }
            SetValue(bindingContext.Model, result);
        }
    }
}
