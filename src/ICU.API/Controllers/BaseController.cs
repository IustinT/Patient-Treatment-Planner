using ICU.Data;

namespace ICU.API.Controllers
{
    public class BaseController : Microsoft.AspNetCore.Mvc.ControllerBase
    {

        public BaseController(IcuContext context)
        {
            Context = context;
        }

        protected IcuContext Context { get; private set; }
    }
}
