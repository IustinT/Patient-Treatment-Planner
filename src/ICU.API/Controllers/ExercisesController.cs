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

        [HttpPost]
        [Route("{patientId}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PostAsync([FromRoute] long patientId, [FromBody] List<ExerciseRepetition> payload)
        {
            var patient = await Context.Patients.FindAsync(patientId);

            if (patient.ExercisesAssignment != null)
                Context.PatientExercises.RemoveRange(patient.ExercisesAssignment);

            if (payload.Any())
            {
                Context.PatientExercises.AddRange(
                    payload.Select(s => new PatientExercise
                    {
                        Patient = patient,
                        ExerciseId = s.Id,
                        Repetitions = s.Repetitions
                    }));
            }
            await Context.SaveChangesAsync();

            return Ok();
        }

    }
}
