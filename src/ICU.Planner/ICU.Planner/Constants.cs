﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ICU.Planner
{
    public class Constants
    {

        public static class URLs
        {
            public static string ApiBaseUri =>

                "https://comp-3000-icu-api.azurewebsites.net/"
            //"http://localhost/ICU.API/"
            //"http://192.168.0.3/ICU.API/"
            ;

            public static string PatientsApi = $"{ApiBaseUri}Patients";
            public static string GoalsApi = $"{ApiBaseUri}Goals";
            public static string CpaxApi = $"{ApiBaseUri}CPAX";
            public static string PatientImagesApi = $"{ApiBaseUri}PatientImages";
            public static string SystemConfigApi = $"{ApiBaseUri}SystemConfig";
            public static string ExercisesApi = $"{ApiBaseUri}Exercises";
            public static string TreatmentPlanApi = $"{ApiBaseUri}TreatmentPlan";
        }
        public static readonly string[] FormsFlags = { };

        public static string DefaultHospitalValue { get; internal set; } = "Plymouth";
        public static string DefaultWardValue { get; internal set; } = "ICU";
        public const int MinimumDigitsForPatientSearch = 5;

        public static class Keys
        {

            public const string PatientIdentifierKey = "PatientIdentifier";

            public const string IsNewPatientRecord = "IsNewPatientRecord";
        }

        public static class ResourceKeys
        {
            public const string SafeAreaInsetsResourceName = "SafeAreaInsets";
            public const string SafeAreaInsetsTopResourceName = "SafeAreaInsetsTop";
            public const string SafeAreaInsetsBottomResourceName = "SafeAreaInsetsBottom";
            public const string SafeAreaInsetsLeftResourceName = "SafeAreaInsetsLeft";
            public const string SafeAreaInsetsRightResourceName = "SafeAreaInsetsRight";
            public const string SafeAreaInsetsVerticalResourceName = "SafeAreaInsetsVertical";
            public const string SafeAreaInsetsHorizontalResourceName = "SafeAreaInsetsHorizontal";

            public const string DialogHeaderPaddingResourceName = "DialogHeaderPadding";
            public const string DialogHeaderPaddingDefaultResourceName = "DialogHeaderPaddingDefault";

            public const string DialogHeaderDefaultHeigthResourceName = "DialogHeaderDefaultHeigth";
            public const string DialogHeaderLandscapeHeigthResourceName = "DialogHeaderLandscapeHeigth";
            public const string DialogHeaderHeigthResourceName = "DialogHeaderHeigth";

            public const string MainContentPaddingResourceName = "MainContentPadding";
            public const string MainContentPaddingDefaultResourceName = "MainContentPaddingDefault";

            public static string IsInPortraitMode = "IsInPortraitMode";
            public static string IsInLandscapeMode = "IsInLandscapeMode";
        }

        public static class FlurlHttp
        {
            public static JsonSerializerSettings JsonSettings => new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Include,
                ObjectCreationHandling = ObjectCreationHandling.Auto,
                ContractResolver = new DefaultContractResolver(),
            };
        }
    }
}
