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
    public class LabelInputValue : BaseInputValue
    {
        public string Text { get; set; }
        public LabelInputValue(string text) : base(null, null) { Text = text; }
        public override void Bind(ModelBindingContext bindingContext, DBAppContext dbContext, MaterialsService materials)
        {
            
        }
    }
}
