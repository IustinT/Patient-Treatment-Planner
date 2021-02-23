using ICU.Data;
using ICU.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ICU.API.Controllers
{
    [Route("[controller]")]
    [ProducesResponseType(typeof(IEnumerable<SystemConfig>), StatusCodes.Status200OK)]
    public class SystemConfigController : BaseController
    {
        public SystemConfigController(IcuContext context) : base(context)
        { }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var configData = new SystemConfig
            {
                ImageCategories = await Context.ImageCategories.ToListAsync(),

            };

            return Ok(configData);
        }
    }
}
