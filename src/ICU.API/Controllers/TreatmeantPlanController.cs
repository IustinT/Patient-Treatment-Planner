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
    public class TreatmentPlanController : BaseController
    {
        public TreatmentPlanController(IcuContext context) : base(context)
        { }

        [HttpPost]
        [Route("{patientId}/{mondayExerciseTime}/{tuesdayExerciseTime}/{wednesdayExerciseTime}/{thursdayExerciseTime}/{fridayExerciseTime}/{saturdayExerciseTime}/{sundayExerciseTime}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task PostAsync(
            [FromRoute] long patientId,
            [FromRoute] int mondayExerciseTime,
            [FromRoute] int tuesdayExerciseTime,
            [FromRoute] int wednesdayExerciseTime,
            [FromRoute] int thursdayExerciseTime,
            [FromRoute] int fridayExerciseTime,
            [FromRoute] int saturdayExerciseTime,
            [FromRoute] int sundayExerciseTime,
            [FromBody] List<ExerciseRepetition> payload)
        {
            var patient = await Context.Patients
                .Include(i => i.ExercisesAssignment)
                .FirstAsync(f => f.Id == patientId);

            patient.MondayExerciseTime = mondayExerciseTime;
            patient.TuesdayExerciseTime = tuesdayExerciseTime;
            patient.WednesdayExerciseTime = wednesdayExerciseTime;
            patient.ThursdayExerciseTime = thursdayExerciseTime;
            patient.FridayExerciseTime = fridayExerciseTime;
            patient.SaturdayExerciseTime = saturdayExerciseTime;
            patient.SunExerciseTime = sundayExerciseTime;

            if (patient.ExercisesAssignment != null)
            {
                //remove all existing exercise assignments for this patient
                //because we're going to save new assignments
                Context.PatientExercises.RemoveRange(patient.ExercisesAssignment);

                //save changes so we can add same primary keys, if any
                await Context.SaveChangesAsync();
            }

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
        }

    }
}
