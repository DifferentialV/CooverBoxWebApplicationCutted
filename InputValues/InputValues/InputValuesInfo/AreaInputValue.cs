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
    public class AreaInputValue : BaseInputValue
    {
        public List<BaseInputValue> Items { get; set; }
        public AreaInputValue() : base(null, null) { Items = new List<BaseInputValue>(); }

        public bool Card { get; set; } = false;
        public string Header { get; set; }

        public override void Bind(ModelBindingContext bindingContext, DBAppContext dbContext, MaterialsService materials)
        {
            if (string.IsNullOrEmpty(Roll) is false && bindingContext.HttpContext.User?.IsInRole(Roll) is false)
                return;
            foreach (var item in Items)
                item.Bind(bindingContext, dbContext, materials);
        }
    }
}
