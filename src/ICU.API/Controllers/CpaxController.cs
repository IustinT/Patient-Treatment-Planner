using ICU.Data;
using ICU.Data.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ICU.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CpaxController : BaseController
    {
        public CpaxController(IcuContext context) : base(context)
        { }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(CpaxDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostAsync([FromBody] CpaxDTO payload)
        {
            bool shouldSaveCurrentCpax = await ShouldAddCpaxToDb(payload.CurrentCpax, Context);
            var shouldSaveGoalCpax = await ShouldAddCpaxToDb(payload.GoalCpax, Context);

            if (shouldSaveCurrentCpax)
            {
                ResetFields(payload.CurrentCpax);
                Context.CPAXes.Add(payload.CurrentCpax);
            }

            if (shouldSaveGoalCpax)
            {
                ResetFields(payload.GoalCpax);
                Context.CPAXes.Add(payload.GoalCpax);
            }

            if (shouldSaveCurrentCpax || shouldSaveGoalCpax)
                await Context.SaveChangesAsync();

            return Ok(payload);
        }

        /// <summary>
        /// Reset some fields so the object will be saved as a new object to DB
        /// </summary>
        /// <param name="cpax"></param>
        private static void ResetFields(CPAX cpax)
        {
            cpax.DateTime = DateTime.Now;
            cpax.Id = null;
        }

        private static async Task<bool> ShouldAddCpaxToDb(CPAX cpax, IcuContext context)
            =>
            //don't save if the object is null or it has no values
            cpax != null && !cpax.IsEmpty() &&
            //save if it is a new object
            (cpax.Id is null ||
            //save it the cpax has any changes
            (await context.CPAXes.FindAsync(cpax.Id)).Equals(cpax) is false);
    }
}
