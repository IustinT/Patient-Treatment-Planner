using System;
using System.Security.Cryptography.X509Certificates;

namespace ICU.API
{
    public static class Constants
    {
        public const string PatientImagesFolder = "PatientImages";
        public static readonly string[] ImgExtensions = { ".jpg", ".jpeg", ".bmp", ".gif", ".png" };

        public static string CreatePatientImageUri(long patientId, string fileName, int categoryId, string scheme, string host)
            => $"{scheme}://{host}/StaticFiles/PatientImages/{patientId}/{categoryId}/{fileName}";

    }
}
