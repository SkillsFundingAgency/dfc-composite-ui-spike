using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ncs.Prototype.Web.Courses.ModelBinding
{
    public class CustomModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ParameterName == "model")
            {
                return new CustomModelBinder();
            }

            return null;
        }
    }
}
