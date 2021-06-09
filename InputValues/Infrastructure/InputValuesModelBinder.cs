using CooverBoxWebApplication.Models;
using CooverBoxWebApplication.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Infrastructure
{
    public class InputValuesModelBinder : IModelBinder
    {
        MaterialsService _materials;
        DBAppContext _dbContext;
        public InputValuesModelBinder(DBAppContext dbContext, MaterialsService materials)
        {
            _materials = materials;
            _dbContext = dbContext;
        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            Type modelType = null;
            try
            {
                string typeFullName = bindingContext.ValueProvider.GetValue("TypeFullName").FirstValue;
                modelType = Type.GetType(typeFullName);
                if (modelType == null)
                {
                    foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        modelType = asm.GetType(typeFullName);
                        if (modelType != null) break;
                    }
                }
            }
            catch { }
            if (modelType == null) modelType = bindingContext.ModelType;
            dynamic SAD;
            try
            {
                SAD = modelType.GetProperty("InputValues", typeof(List<InputValues.Base.BaseInputValue>)).GetGetMethod().Invoke(null, null);
            }
            catch {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }
            if(SAD is null)
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }
            
            bindingContext.Model = Activator.CreateInstance(modelType);
            foreach (InputValues.Base.BaseInputValue isdnput in SAD)
            {
                isdnput.Bind(bindingContext, _dbContext, _materials);
            }
            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;
        }
    }
}
