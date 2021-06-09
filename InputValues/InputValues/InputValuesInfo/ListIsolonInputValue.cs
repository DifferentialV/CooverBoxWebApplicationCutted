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
    public class ListIsolonInputValue : BaseInputValue
    {
        public string AddOption { get; set; }
        public bool AlwaysDefault { get; set; }
        public int Count { get; set; } = 1;
        public ListIsolonInputValue(System.Reflection.PropertyInfo valueInfo, string displayName) : base(valueInfo, displayName)
        {
            if (new[] { typeof(List<Isolon>) }.Contains(valueInfo?.PropertyType) == false)
            {
                throw new Exception($"{displayName} valueInfo должен указывать на поля типа List<Isolon>");
            }
        }

        public override void Bind(ModelBindingContext bindingContext, DBAppContext dbContext, MaterialsService materials)
        {
            if (string.IsNullOrEmpty(Roll) is false && bindingContext.HttpContext.User?.IsInRole(Roll) is false)
                return;
            List<Isolon> resultlist = new List<Isolon>();
            for(int i=0;i<Count;i++)
            {
                string text = bindingContext.ValueProvider.GetValue($"{Name}_{i}").FirstValue?.Trim();
                int? resultId = null;
                if (int.TryParse(text, out int outresult))
                {
                    resultId = outresult;
                }
                if (resultId == null)
                {
                    if (Required == true && string.IsNullOrEmpty(AddOption))
                        bindingContext.ModelState.AddModelError(string.Empty, $"Поле {DisplayName} не удалось считать.");
                    continue;
                }
                Isolon result = materials.Isolons.FirstOrDefault(m => m.Id == resultId.Value);
                if (result == null)
                {
                    if (Required == true && string.IsNullOrEmpty(AddOption))
                        bindingContext.ModelState.AddModelError(string.Empty, $"Поле {DisplayName} не удалось считать.");
                    continue;
                }
                resultlist.Add(result);
            }
            
            SetValue(bindingContext.Model, resultlist);
        }
    }
}
