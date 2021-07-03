using ICU.Data;
using ICU.Data.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using System.IO;
using System;

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
        /// <summary>
        /// Gel all patient data in one API call
        /// </summary>
        /// <param name="id">Patient ID</param>
        /// <returns><see cref="Patient"/></returns>
        [HttpGet()]
        [Route("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Patient), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync(long id)
        {
            var patient = await GetPatient(id);
            if (patient is null) return BadRequest("Invalid patient ID");
            return Ok(patient);
        }


        [HttpPost]
        [Route("")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> PostAsync([FromBody] Patient newPatient)
        {
            Context.Patients.Add(newPatient);
            await Context.SaveChangesAsync();

            newPatient = await GetPatient(newPatient.Id.Value);

            return base.Ok(newPatient);
        }

        // PUT api/<PatientsController>/5
        [HttpPut()]
        [Route("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(Patient), StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put(long id, [FromBody] Patient value)
        {
            if (!Context.Patients.Any(a => a.Id == id)) return BadRequest("Patient not found. ID: " + id);

            Context.Patients.Update(value);
            Context.SaveChanges();

            value = await GetPatient(value.Id.Value);

            return Ok(value);
        }

        private async Task<Patient> GetPatient(long patientId)
        {

            var patient = await Context.Patients
                .Include(i => i.CPAXes)
                .Include(i => i.Goals)
                .Include(i => i.Achievemts)
                .FirstOrDefaultAsync(patient => patient.Id == patientId);

            if (patient is null) return null;

            patient.CurrentCPAX = patient.CPAXes.OrderByDescending(cpax => cpax.DateTime).FirstOrDefault(cpax => !cpax.IsGoal);
            patient.GoalCPAX = patient.CPAXes.OrderByDescending(cpax => cpax.DateTime).FirstOrDefault(cpax => cpax.IsGoal);

            patient.MiniGoals = patient.Goals.Where(goal => goal.IsMainGoal != true).ToList();
            patient.MainGoal = patient.Goals.FirstOrDefault(goal => goal.IsMainGoal is true);

            //get a list of all image categories and any image files for each image category
            var imageCategories = await Context.ImageCategories.ToListAsync();

            var imageCategoriesWithFiles = imageCategories.Select(category => new ImageCategoryWithFiles
            {
                Name = category.Name,
                Deleted = category.Deleted,
                Id = category.Id.Value,
                DisplayOrder = category.DisplayOrder
            })
                .ToList();

            var patientDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), Constants.PatientImagesFolder, patientId.ToString());

            //get files for each image
            if (Directory.Exists(patientDirectoryPath))
            {
                foreach (var category in imageCategoriesWithFiles)
                {
                    //find the directory fir this category, for this patient
                    var categoryDirectoryForPatient = Path.Combine(patientDirectoryPath, category.Id.ToString());

                    if (Directory.Exists(categoryDirectoryForPatient))
                        //add the image file names to the list
                        category.ImageFiles = Directory
                                        .EnumerateFiles(categoryDirectoryForPatient)
                                        //get the file name with extension
                                        .Select(filePath => Path.GetFileName(filePath))
                                        //only image files
                                        .Where(fileName => Constants.ImgExtensions.Any(a => a == Path.GetExtension(fileName)))
                                        .Select(fileName => new ImageFile
                                        {
                                            CategoryId = category.Id.Value,
                                            PatientId = patientId,
                                            FileName = fileName,

                                            Uri = Uri.EscapeUriString(
                                                Constants.CreatePatientImageUri(patientId, fileName, category.Id.Value, HttpContext.Request.Scheme, HttpContext.Request.Host.Value)
                                            ).ToString()
                                        }
                                        )
                                        .ToList();

                }

            }

            //remove image categories that were deleted and have no image files
            imageCategoriesWithFiles.RemoveAll(o => o.Deleted && o.ImageFiles is null);

            patient.Images = imageCategoriesWithFiles;

            return patient;
        }
    }
}
