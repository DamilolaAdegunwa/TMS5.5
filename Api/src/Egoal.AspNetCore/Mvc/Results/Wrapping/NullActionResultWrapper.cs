using Microsoft.AspNetCore.Mvc.Filters;

namespace Egoal.Mvc.Results.Wrapping
{
    public class NullActionResultWrapper : IActionResultWrapper
    {
        public void Wrap(ResultExecutingContext actionResult)
        {

        }
    }
}
