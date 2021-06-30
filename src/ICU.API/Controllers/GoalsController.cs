using ICU.Data;
using ICU.Data.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ICU.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GoalsController : BaseController
    {
        public GoalsController(IcuContext context) : base(context)
        { }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Goal), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostAsync([FromBody] Goal value)
        {
            //remove existing main goal, if required
            if (value.IsMainGoal is true)
                Context.Goals.RemoveRange(Context.Goals.Where(goal => goal.PatientId == value.PatientId && goal.IsMainGoal == true));

            Context.Goals.Add(value);
            await Context.SaveChangesAsync();

            return Ok(value);
        }

        [HttpDelete()]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task DeleteAsync(Guid id)
        {
            var goal = await Context.Goals.FindAsync(id);

            //if the goal is missing then it was already deleted
            if (!(goal is null))
            {
                Context.Goals.Remove(goal);
                await Context.SaveChangesAsync();
            }
        }
    }
}
