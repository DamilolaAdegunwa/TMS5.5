using Egoal.Application.Services.Dto;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Egoal.Mvc.ModelBinding
{
    public class DateTimeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType != typeof(DateTime) &&
                context.Metadata.ModelType != typeof(DateTime?))
            {
                return null;
            }

            if (context.Metadata.ContainerType == null)
            {
                return null;
            }

            var isEndTime = context.Metadata.ContainerType.GetProperty(context.Metadata.PropertyName).IsDefined(typeof(EndTimeAttribute), true);
            if (isEndTime)
            {
                return new EndTimeModelBinder(context.Metadata.ModelType);
            }

            return null;
        }
    }
}
