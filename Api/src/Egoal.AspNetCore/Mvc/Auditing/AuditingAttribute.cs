using Egoal.Auditing;
using Egoal.Mvc.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Egoal.Mvc.Auditing
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class AuditingAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            AuditingOptions auditingOptions = context.HttpContext.RequestServices.GetRequiredService<IOptions<AuditingOptions>>().Value;
            IAuditingHelper auditingHelper = context.HttpContext.RequestServices.GetRequiredService<IAuditingHelper>();
            IAuditSerializer auditSerializer = context.HttpContext.RequestServices.GetRequiredService<IAuditSerializer>();

            if (!ShouldSaveAudit(context, auditingHelper))
            {
                await next();
                return;
            }

            var auditInfo = auditingHelper.CreateAuditInfo(
                    context.ActionDescriptor.AsControllerActionDescriptor().ControllerTypeInfo.AsType(),
                    context.ActionDescriptor.AsControllerActionDescriptor().MethodInfo,
                    context.ActionArguments
                );

            var stopwatch = Stopwatch.StartNew();

            ActionExecutedContext result = null;
            try
            {
                result = await next();
                if (result.Exception != null && !result.ExceptionHandled)
                {
                    auditInfo.Exception = result.Exception;
                }
            }
            catch (Exception ex)
            {
                auditInfo.Exception = ex;
                throw;
            }
            finally
            {
                stopwatch.Stop();
                auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);

                if (auditingOptions.SaveReturnValues && result != null)
                {
                    switch (result.Result)
                    {
                        case ObjectResult objectResult:
                            auditInfo.ReturnValue = auditSerializer.Serialize(objectResult.Value);
                            break;

                        case JsonResult jsonResult:
                            auditInfo.ReturnValue = auditSerializer.Serialize(jsonResult.Value);
                            break;

                        case ContentResult contentResult:
                            auditInfo.ReturnValue = contentResult.Content;
                            break;
                    }
                }

                await auditingHelper.SaveAsync(auditInfo);
            }
        }

        private bool ShouldSaveAudit(ActionExecutingContext actionContext, IAuditingHelper auditingHelper)
        {
            return actionContext.ActionDescriptor.IsControllerAction() &&
                   auditingHelper.ShouldSaveAudit(actionContext.ActionDescriptor.GetMethodInfo(), true);
        }
    }
}
