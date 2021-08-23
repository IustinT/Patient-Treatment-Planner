using ICU.Data;
using ICU.Data.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace ICU.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExercisesController : BaseController
    {
        public ExercisesController(IcuContext context) : base(context)
        { }

        [HttpGet()]
        [Route("")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IEnumerable<Exercise>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync()
        {
            var results = await Context.ExerciseCategories
                .Include(i => i.Exercises)
                .OrderBy(o => o.Name)
                .ToListAsync();

            foreach (var category in results)
            {
                category.Exercises = category.Exercises.OrderBy(o => o.Name);
            }

            return Ok(results);
        }

    }
}
