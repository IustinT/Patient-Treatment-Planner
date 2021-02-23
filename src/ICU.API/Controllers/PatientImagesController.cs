using ICU.Data;
using ICU.Data.Models;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ICU.API.Controllers
{
    [Route("[controller]")]
    public class PatientImagesController : BaseController
    {
        private const string PatientImagesFolder = "PatientImages";
        private readonly string[] imgExtensions = { ".jpg", ".jpeg", ".bmp", ".gif", ".png" };

        public PatientImagesController(IcuContext context) : base(context)
        { }

        [HttpGet]
        [Route("Urls/{patientId}")]
        public IActionResult UrlsdAsync(long patientId)
        {
            var patientFolderName = Path.Combine(PatientImagesFolder, patientId.ToString());
            var finalFolderPath = Path.Combine(Directory.GetCurrentDirectory(), patientFolderName);

            if (!Directory.Exists(finalFolderPath)) return NoContent();

            var results = new List<PatientImagesByCategoryId>();

            Directory
                .EnumerateDirectories(finalFolderPath)
                .ToList()
                .ForEach(categoryDirectory =>
                {
                    if (Path.EndsInDirectorySeparator(categoryDirectory))
                        categoryDirectory = categoryDirectory.Substring(0, categoryDirectory.Length - 1);

                    if (!int.TryParse(Path.GetFileName(categoryDirectory), out var categoryId))
                        return;

                    results.Add(new PatientImagesByCategoryId
                    {
                        CategoryId = categoryId,
                        Names = Directory
                        .EnumerateFiles(categoryDirectory)
                        .Select(filePath => Path.GetFileName(filePath))
                        .Where(fileName => imgExtensions.Any(a => a == Path.GetExtension(fileName)))
                        .ToList()
                    });
                });

            return Ok(results);
        }

        [HttpPost]
        [Route("{patientId}/{imageCategoryId}")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadAsync(long patientId, int imageCategoryId)
        {
            var filesList = Request.Form.Files.ToList();

            if (filesList.Count is 0) return NoContent();

            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), PatientImagesFolder, patientId.ToString(), imageCategoryId.ToString());

            if (filesList.Any(f => f.Length == 0)) return BadRequest();

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            var fileNames = new List<string>();

            foreach (var file in filesList)
            {
                //rename the file to a numbers only guid, this helps when creating URLs
                var newFileName = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);

                //save in the patient/category folder
                var fullPath = Path.Combine(directoryPath, newFileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                file.CopyTo(stream);

                fileNames.Add(Path.GetFileName(fullPath));
            }

            return Ok(fileNames);
        }

        [HttpDelete]
        [Route("{patientId}/{imageCategoryId}/{fileName}")]
        public void Delete(long patientId, int imageCategoryId, string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), PatientImagesFolder, patientId.ToString(), imageCategoryId.ToString(), fileName);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }
    }
}
