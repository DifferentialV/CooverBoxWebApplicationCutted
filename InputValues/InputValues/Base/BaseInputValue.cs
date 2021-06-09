using CooverBoxWebApplication.Infrastructure;
using CooverBoxWebApplication.Models;
using CooverBoxWebApplication.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.InputValues.Base
{
    public abstract class BaseInputValue
    {
        public BaseInputValue(PropertyInfo valueInfo,string displayName)
        {
            ValueInfo = valueInfo;
            DisplayName = displayName;
        }
        public string DisplayName { get; }
        public PropertyInfo ValueInfo { get; }
        
        public string Name { get { return ValueInfo?.Name; } }
        public Type ValueType { get { return ValueInfo?.PropertyType; } }
        
        public bool Required { get; set; } = true;

        public string Roll { get; set; }

        public dynamic GetValue(dynamic model)
        {
            return ValueInfo?.GetValue(model);
        }
        public void SetValue(dynamic model,dynamic value)
        {
            ValueInfo?.SetValue(model, value);
        }
        public abstract void Bind(ModelBindingContext bindingContext, DBAppContext dbContext, MaterialsService materials);

    }
}
