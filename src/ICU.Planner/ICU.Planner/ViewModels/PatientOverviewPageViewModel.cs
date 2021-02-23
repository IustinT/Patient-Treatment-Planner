using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;

using Flurl;
using Flurl.Http;

using Humanizer;

using ICU.Data.Models;

using Prism.Commands;
using Prism.Magician;
using Prism.Navigation;
using ICU.Planner.Models;
using Shiny;
using Shiny.Logging;
using Xamarin.Essentials.Interfaces;
using System.IO;

namespace ICU.Planner.ViewModels
{
    [AutoInitialize]
    public partial class PatientOverviewPageViewModel : IntermediaryViewModelBase
    {
        private readonly Subject<long> patientIdChangedSubject = new Subject<long>();

        private ICommand editPatientCommand;
        private ICommand addMainGoalCommand;
        private ICommand addGoalCommand;
        private ICommand deleteGoalCommand;
        private ICommand addImageCommand;
        private ICommand _deleteImageCommand;
        private List<Goal> _goals;
        private List<ImagesByCategoryModel> imagesByCategory;

        protected PatientOverviewPageViewModel(BaseServices baseServices, IMediaPicker mediaPicker, IFileSystem fileSystem) : base(baseServices)
        {
            MediaPicker = mediaPicker;
            FileSystem = fileSystem;
            _goals = new List<Goal>();
            imagesByCategory = SystemConfig.ImageCategories
                .Where(category => !category.Deleted)
                .OrderBy(category => category.DisplayOrder)
                .Select(category => new ImagesByCategoryModel { Category = category })
                .ToList();

            Title = "Patient Page";

            patientIdChangedSubject
                .Throttle(.5.Seconds())
                .SubscribeAsync(id => Log.SafeExecute(() => Task.WhenAll(RefreshGoals(id), RefreshImages(id))))
                .DisposedBy(Disposables);
        }

        #region Properties

        [Bindable] public Patient Patient { get; set; }

        public List<Goal> Goals { get => _goals; set => SetProperty(ref _goals, value); }
        [Bindable] public Goal MainGoal { get; set; }

        public List<ImagesByCategoryModel> ImagesByCategory { get => imagesByCategory; set => SetProperty(ref imagesByCategory, value); }

        public IMediaPicker MediaPicker { get; }
        public IFileSystem FileSystem { get; }

        #endregion

        protected override async Task InitializeAsync(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(nameof(Patient), out Patient patient) && patient != null && patient.Id.HasValue)
            {
                Patient = patient;
                patientIdChangedSubject.OnNext(patient.Id.Value);
            }
            else
                await HandleGoBackRequest(parameters);
        }

        #region Commands

        #region EditPatientCommand

        public ICommand EditPatientCommand => editPatientCommand ??=
            new DelegateCommand(async () => await EditPatientCommandExecute(Patient),
                                EditPatientCommandCanExecute)
            .ObservesProperty(() => Patient)
            .ObservesProperty(() => IsBusy);

        private async Task EditPatientCommandExecute(Patient patient)
        {
            if (patient is null || IsBusy) return;

            try
            {
                SetIsBusy();

                var r = await ShowDialogAsync(Navigation.DialogKeys.PatientFormDialog, new Prism.Services.Dialogs.DialogParameters { { nameof(Patient), patient } });

                if (r.Exception != null) throw r.Exception;

                if (r.Parameters.TryGetValue(nameof(Patient), out Patient newPatient))
                {

                    bool shouldRetryPost = true;
                    do
                    {
                        try
                        {
                            newPatient = await Constants.URLs.PatientsApi
                                .AppendPathSegment(newPatient.Id)
                                .PutJsonAsync(newPatient)
                                .ReceiveJson<Patient>();

                            shouldRetryPost = false;
                            Patient = patient;

                            RaisePropertyChanged(nameof(Patient));
                        }
                        catch (Exception postException)
                        {
                            shouldRetryPost = await DisplayDialog("Save Failed. Retry?", postException.Message, "Retry", "Cancel");
                            await Task.Delay(.5.Seconds());
                        }
                    } while (shouldRetryPost);

                }
            }
            catch (Exception e)
            {
                Log.Write(e);
            }
            finally
            {
                ClearIsBusy();
            }
        }

        private bool EditPatientCommandCanExecute() =>
            Patient != null && IsNotBusy;

        #endregion

        #region DeleteGoalCommand

        public ICommand DeleteGoalCommand => deleteGoalCommand ??=
            new DelegateCommand<Goal>(async goal => await DeleteGoalCommandExecute(goal))
            .ObservesProperty(() => Goals);

        private async Task DeleteGoalCommandExecute(Goal goal)
        {
            if (goal is null || IsBusy) return;

            try
            {
                await Constants.URLs.GoalsApi.AppendPathSegment(goal.Id).DeleteAsync();

                Goals.Remove(goal);

                if (goal.IsMainGoal is true)
                    MainGoal = null;
                else
                    Goals = Goals.ToList();

            }
            catch (Exception e)
            {
                Log.Write(e);
            }
            finally { }
        }

        #endregion 

        #region AddMainGoalCommand

        public ICommand AddMainGoalCommand => addMainGoalCommand ??=
            new DelegateCommand(async () => await AddMainGoalCommandExecute(), AddMainGoalCommandCanExecute)
            .ObservesProperty(() => MainGoal);

        private bool AddMainGoalCommandCanExecute() => MainGoal is null;

        private async Task AddMainGoalCommandExecute()
        {
            try
            {
                var goalValue = await PageDialogService
                     .DisplayPromptAsync(
                    "New Main Goal",
                    "Please type in the patient's main goal",
                    maxLength: 450,
                    keyboard: Xamarin.Forms.Keyboard.Plain);

                if (!string.IsNullOrWhiteSpace(goalValue))
                {
                    MainGoal = await Constants.URLs.GoalsApi
                        .PostJsonAsync(new Goal
                        {
                            PatientId = Patient.Id.Value,
                            IsMainGoal = true,
                            Value = goalValue
                        })
                        .ReceiveJson<Goal>();

                }
            }
            catch (Exception e)
            {
                Log.Write(e);
            }
        }

        #endregion

        #region AddGoalCommand

        public ICommand AddGoalCommand => addGoalCommand ??=
            new DelegateCommand(async () => await AddGoalCommandExecute());

        private async Task AddGoalCommandExecute()
        {
            try
            {
                var goalValue = await PageDialogService
                     .DisplayPromptAsync(
                    "New Goal",
                    "Please type in the patient's new goal",
                    maxLength: 450,
                    keyboard: Xamarin.Forms.Keyboard.Plain);

                if (!string.IsNullOrWhiteSpace(goalValue))
                {
                    var newGoal = await Constants.URLs.GoalsApi
                        .PostJsonAsync(new Goal
                        {
                            PatientId = Patient.Id.Value,
                            IsMainGoal = false,
                            Value = goalValue
                        })
                        .ReceiveJson<Goal>();

                    Goals.Add(newGoal);

                    Goals = Goals.ToList();
                }
            }
            catch (Exception e)
            {
                Log.Write(e);
            }
        }

        #endregion

        #region DeleteImageCommand

        public ICommand DeleteImageCommand => _deleteImageCommand ??=
            new DelegateCommand<Uri>(async imageUri => await DeleteImageCommandExecute(imageUri, Patient.Id.Value));

        private async Task DeleteImageCommandExecute(Uri uri, long patientId)
        {
            if (uri is null || IsBusy) return;

            try
            {
                //TODO: the uri migth require unescaping
                string[] reversedSegments = uri.Segments.Reverse().Select(s => s.Replace("/", "").Trim()).ToArray();
                var fileName = reversedSegments[0] as string;

                if (string.IsNullOrWhiteSpace(fileName) || !(reversedSegments.Length > 1) || !int.TryParse(reversedSegments[1], out var categoryId)) return;

                await Constants.URLs.PatientImagesApi
                    .AppendPathSegments(patientId, categoryId, fileName)
                    .DeleteAsync();

                var group = ImagesByCategory.FirstOrDefault(category => category.CategoryId == categoryId);

                if (group != null)
                {
                    group.Names.Remove(fileName);
                    group.Uris.Remove(uri);

                    ImagesByCategory = ImagesByCategory.ToList();
                }
            }
            catch (Exception e)
            {
                Log.Write(e);
            }
            finally { }
        }

        #endregion

        #region AddImageCommand
        public ICommand AddImageCommand => addImageCommand ??=
            new DelegateCommand<ImageCategory>(async imageCategory => await AddImageCommandExecute(imageCategory, Patient.Id.Value));

        private async Task AddImageCommandExecute(ImageCategory imageCategory, long patientId)
        {
            if (imageCategory is null || IsBusy) return;

            try
            {
                var source = MediaPicker.IsCaptureSupported switch
                {
                    true => await PageDialogService.DisplayActionSheetAsync("Select source:", "Cancel", "Gallery", "Camera"),
                    _ => "Gallery"
                };

                var photo = source switch
                {
                    "Camera" => await MediaPicker.CapturePhotoAsync(),
                    "Gallery" => await MediaPicker.PickPhotoAsync(new Xamarin.Essentials.MediaPickerOptions { Title = imageCategory.Name ?? "Select a Photo" }),
                    _ => null
                };

                // canceled
                if (photo == null) return;

                // save the file into local storage
                var newFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                {
                    using var stream = await photo.OpenReadAsync();
                    using var newStream = File.OpenWrite(newFilePath);
                    await stream.CopyToAsync(newStream);
                }

                var serverFileName = (
                    await Constants.URLs.PatientImagesApi
                    .AppendPathSegments(patientId, imageCategory.Id.Value)
                    .PostMultipartAsync(builder => builder.AddFile("file1", newFilePath))
                    .ReceiveJson<List<string>>())
                    .FirstOrDefault(); //we're uploading only one image

                //display the added image
                if (!string.IsNullOrWhiteSpace(serverFileName))
                {
                    var group = ImagesByCategory.FirstOrDefault(f => f.CategoryId == imageCategory.Id);
                    if (group != null)
                    {
                        group.Names.Add(serverFileName);
                        group.Uris.Add(CreateImageUrl(patientId, serverFileName, imageCategory.Id.Value).ToUri());

                        ImagesByCategory = ImagesByCategory.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }

        #endregion
        #endregion

        #region GetData

        private async Task RefreshGoals(long patientId)
        {
            var goals = await Constants.URLs.GoalsApi.AppendPathSegment(patientId)
                .GetJsonAsync<List<Goal>>();

            MainGoal = goals.FirstOrDefault(goal => goal.IsMainGoal is true);
            Goals = goals.Where(goal => goal.IsMainGoal != true).ToList();
        }

        private async Task RefreshImages(long patientId)
        {
            var fromServer = await Constants.URLs.PatientImagesApi
                            .AppendPathSegments("Urls", patientId)
                            .GetJsonAsync<List<PatientImagesByCategoryId>>();

            var groupList = SystemConfig
                .ImageCategories
                .Select(category => new ImagesByCategoryModel
                {
                    Category = category,
                    CategoryId = category.Id.Value,

                    Names = fromServer
                    .Where(w => w.CategoryId == category.Id.Value)
                    .SelectMany(s => s.Names)
                    .ToList()
                })
                .ToList();

            foreach (var group in groupList)
                group.Uris = group
                    .Names
                    .Select(fileName => CreateImageUrl(patientId, fileName, group.CategoryId).ToUri())
                    .ToList();

            ImagesByCategory = groupList;
        }

        private static Url CreateImageUrl(long patientId, string fileName, int imageCategoryId) =>
            Constants.URLs.ApiBaseUri
            .AppendPathSegments("StaticFiles", "PatientImages", patientId, imageCategoryId, fileName);

        #endregion
    }
}