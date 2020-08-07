using Egoal.Extensions;
using Egoal.Mvc.Uow;
using Egoal.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Egoal.Web.Api.Controllers
{
    [Route("/Api/[controller]/[action]")]
    [ApiController]
    [UnitOfWork]
    public class TmsControllerBase : ControllerBase
    {
        protected JsonResult Json(object value)
        {
            return new JsonResult(value);
        }

        protected JsonResult Json(object value, JsonSerializerSettings serializerSettings)
        {
            return new JsonResult(value, serializerSettings);
        }

        protected FileContentResult Excel(byte[] fileContents)
        {
            var provider = new FileExtensionContentTypeProvider();
            var contentType = provider.Mappings[".xlsx"];

            return File(fileContents, contentType);
        }

        protected async Task UpdateModelAsync<TModel>(TModel model, string prefix, IValueProvider valueProvider) where TModel : class
        {
            var bindingSuccess = await TryUpdateModelAsync(model, string.Empty, valueProvider);
            if (!bindingSuccess)
            {
                if (!ModelState.IsValid)
                {
                    foreach (var state in ModelState)
                    {
                        if (!state.Value.Errors.IsNullOrEmpty())
                        {
                            throw new UserFriendlyException(state.Value.Errors[0].ErrorMessage);
                        }
                    }
                }
            }
        }
    }
}
