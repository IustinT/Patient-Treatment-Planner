using ICU.Data;
using ICU.Data.Models;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;

namespace ICU.API.Controllers
{
    [Route("[controller]")]
    public class PatientImagesController : BaseController
    {
        public PatientImagesController(IcuContext context) : base(context)
        { }

        [HttpPost]
        [Route("{patientId}/{imageCategoryId}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(IEnumerable<ImageFile>), StatusCodes.Status200OK)]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadAsync(long patientId, int imageCategoryId)
        {
            var filesList = Request.Form.Files.ToList();

            if (filesList.Count is 0) return NoContent();

            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), Constants.PatientImagesFolder, patientId.ToString(), imageCategoryId.ToString());

            if (filesList.Any(f => f.Length == 0)) return BadRequest();

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            var fileNames = new List<ImageFile>();

            foreach (var file in filesList)
            {
                //rename the file to a numbers only guid, this helps when creating URLs
                var newFileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

                //save in the patient/category folder
                var fullPath = Path.Combine(directoryPath, newFileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(stream);

                fileNames.Add(new ImageFile
                {
                    FileName = Path.GetFileName(fullPath),
                    CategoryId = imageCategoryId,
                    PatientId = patientId,

                    Uri = Uri.EscapeUriString(
                        Constants.CreatePatientImageUri(
                            patientId,
                            newFileName,
                            imageCategoryId,
                            HttpContext.Request.Scheme,
                            HttpContext.Request.Host.Value)
                        ).ToString()
                });
            }

            return Ok(fileNames);
        }

        [HttpDelete]
        [Route("{patientId}/{imageCategoryId}/{fileName}")]
        public void Delete(long patientId, int imageCategoryId, string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), Constants.PatientImagesFolder, patientId.ToString(), imageCategoryId.ToString(), fileName);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }
    }
}
