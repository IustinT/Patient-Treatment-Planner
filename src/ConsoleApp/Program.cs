using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Microsoft.EntityFrameworkCore;
using Humanizer;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using ICU.Data.Models;
using ICU.Data;
using Microsoft.Extensions.Configuration;
using ConsoleApp.Models;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddEnvironmentVariables()
              .AddCommandLine(args)
              .Build();

            Console.WriteLine("Paste path to Execises file:");
            var jsonFilePath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(jsonFilePath))
                throw new Exception(nameof(jsonFilePath));

            if (!File.Exists(jsonFilePath))
                throw new FileNotFoundException(jsonFilePath);

            var fileContent = await File.ReadAllTextAsync(jsonFilePath);

            var mainObject = JObject.Parse(fileContent);

            var imgDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "ExerciseImages");
            if (!Directory.Exists(imgDirectoryPath))
                Directory.CreateDirectory(imgDirectoryPath);

            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            var dbOptionsBuilder = new DbContextOptionsBuilder<IcuContext>()
                  .UseSqlServer(connectionString);

            Console.WriteLine(imgDirectoryPath);
            Console.WriteLine(connectionString);

            await using var context = new IcuContext(dbOptionsBuilder.Options);
            await context.ExerciseCategories.LoadAsync();

            foreach (var categoryProperty in mainObject.Properties())
            {
                var categoryName = categoryProperty.Name.Pascalize();
                var exercises = JsonConvert.DeserializeObject<List<ExerciseModel>>(JArray.FromObject(categoryProperty.Value).ToString());

                var categoryEntity = await context.ExerciseCategories.FirstOrDefaultAsync(f => f.Name == categoryName);

                if (categoryEntity == null)
                    categoryEntity = context.ExerciseCategories.Add(new ExerciseCategory { Name = categoryName }).Entity;

                //add all exercises to db
                foreach (var exercise in exercises)
                {
                    exercise.Category = categoryEntity;

                    context.Exercises.Add(exercise);

                }

                //save the new exercise so we get Ids for exercises
                await context.SaveChangesAsync();

                //save images
                foreach (var exercise in exercises)
                {
                    if (!string.IsNullOrWhiteSpace(exercise.Img))
                    {
                        var imgBytes = Convert.FromBase64String(exercise.Img);
                        var imgPath = Path.Combine(imgDirectoryPath, $"{exercise.Id}.jpg");

                        //don't check if file already exists
                        await File.WriteAllBytesAsync(imgPath, imgBytes);
                    }
                }
            }

            Console.WriteLine("Done");

            await Task.Delay(5000);

        }
    }
}
