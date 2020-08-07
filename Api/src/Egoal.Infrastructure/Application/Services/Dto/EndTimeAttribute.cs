using System;

namespace Egoal.Application.Services.Dto
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class EndTimeAttribute : Attribute
    {
    }
}
