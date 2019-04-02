using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Ncs.Prototype.Web.Courses.ModelBinding
{
    public class CustomModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var viewName = bindingContext.HttpContext.Request.Query["viewName"].ToString();
            var viewNameFull = $"Ncs.Prototype.Web.Courses.Models.{viewName.Replace("/", string.Empty)}ViewModel";

            var modelType = Assembly.GetExecutingAssembly().GetType(viewNameFull);
            var model = Activator.CreateInstance(modelType);

            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
    }
}
