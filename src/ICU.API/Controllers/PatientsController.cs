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
    public class PatientsController : BaseController
    {
        public PatientsController(IcuContext context) : base(context)
        { }

        [HttpGet]
        [Route("Search/{phoneNumber}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(IEnumerable<Patient>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<Patient>> SearchAsync(string phoneNumber)
        {
            if (phoneNumber.Length > 11) phoneNumber = phoneNumber.Substring(0, 11);

            var query = Context.Patients
                .Where(patient => patient.PhoneNumber.Contains(phoneNumber))
                .OrderByDescending(patient => patient.AdmissionDate);

            var task = (phoneNumber.Length == 11) switch
            {
                true => query.ToListAsync(),
                _ => query.Take(20).ToListAsync()
            };

            return await task;

        }

        // GET api/<PatientsController>/5
        [HttpGet()]
        [Route("{phoneNumber}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync(long id)
        {
            var patient = await Context.Patients.FindAsync(id);

            if (patient is null) return BadRequest("Invalid patient phone number");

            var dataTransfer = new DataTransferObject
            {
                Patient = patient,
                Goals = await Context.Goals.Where(goal => goal.PatientId == patient.Id).ToListAsync(),

            };

            return Ok(dataTransfer);
        }

        [HttpPost]
        [Route("")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> PostAsync([FromBody] Patient newPatient)
        {
            Context.Patients.Add(newPatient);
            await Context.SaveChangesAsync();

            return Ok(newPatient);
        }

        // PUT api/<PatientsController>/5
        [HttpPut()]
        [Route("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Put(long id, [FromBody] Patient value)
        {
            if (!Context.Patients.Any(a => a.Id == id)) return BadRequest("Patient not found. ID: " + id);

            Context.Patients.Update(value);
            Context.SaveChanges();

            return Ok(value);
        }
    }
}
