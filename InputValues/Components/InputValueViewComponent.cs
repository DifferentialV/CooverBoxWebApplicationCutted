using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CooverBoxWebApplication.Components
{
    public class InputValueViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(dynamic model, InputValues.Base.BaseInputValue inputValue = null)
        {
            if (inputValue == null)
            {
                try
                {
                    List<InputValues.Base.BaseInputValue> inputValues = (List<InputValues.Base.BaseInputValue>)model.GetType().GetProperty("InputValues").GetValue(null);
                    ViewBag._model = model;
                    return View($"~/InputValues/Base/BaseView.cshtml", inputValues);
                }
                catch(Exception e)
                {
                    throw new Exception($"{model.GetType()} не содержит статическое поле List<InputValues.Base.BaseInputValue> InputValues. {e.Message}");
                }

            }
            else
            {
                if(string.IsNullOrEmpty(inputValue.Roll) is false && User?.IsInRole(inputValue.Roll) is false)
                {
                    return Content(string.Empty);
                }
                ViewBag._model = model;
                return View($"~/InputValues/InputValuesView/{inputValue.GetType().Name.Replace("InputValue", "").Replace("`1", "")}.cshtml", inputValue);
            }
        }
    }
}
