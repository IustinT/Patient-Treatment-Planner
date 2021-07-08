using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Flurl;
using Flurl.Http;

using Humanizer;

using ICU.Data.Models;

using Prism.Commands;
using Prism.Magician;
using Prism.Navigation;

using Xamarin.Essentials.Interfaces;
using System.IO;
using Prism.Logging;

namespace ICU.Planner.ViewModels
{
    public partial class PatientOverviewPageViewModel : IntermediaryViewModelBase
    {


        private ICommand editPatientCommand;
        private ICommand addMainGoalCommand;
        private ICommand addGoalCommand;
        private ICommand saveCpaxCommand;
        private ICommand deleteGoalCommand;
        private ICommand addImageCommand;
        private ICommand _deleteImageCommand;

        protected PatientOverviewPageViewModel(BaseServices baseServices,
                                               IMediaPicker mediaPicker,
                                               IFileSystem fileSystem)
            : base(baseServices)
        {
            Title = "Patient Page";

            MediaPicker = mediaPicker;
            FileSystem = fileSystem;

            _cpaxViewIsExpanded = _mainGoalViewIsExpanded =
                _miniGoalsViewIsExpanded = _personalInfoViewIsExpanded = true;
        }

        #region Properties

        [Bindable] public Patient Patient { get; set; }

        //these command properties don't need to be [Bindable] because Binding is set ToSource
        public ICommand PersonalInfoForceUpdateSizeCommand { get; set; }
        public ICommand MainGoalForceUpdateSizeCommand { get; set; }
        public ICommand MiniGoalsForceUpdateSizeCommand { get; set; }
        public ICommand CpaxForceUpdateSizeCommand { get; set; }

        [Bindable] public bool CpaxViewIsExpanded { get; set; }
        [Bindable] public bool PersonalInfoViewIsExpanded { get; set; }
        [Bindable] public bool MainGoalViewIsExpanded { get; set; }
        [Bindable] public bool MiniGoalsViewIsExpanded { get; set; }

        public IMediaPicker MediaPicker { get; }
        public IFileSystem FileSystem { get; }

        #endregion

        protected override async Task InitializeAsync(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(nameof(Patient), out Patient patient) && patient != null && patient.Id.HasValue)
            {
                Title = $"Patient - {patient.Name}";
                Patient = patient;

                PersonalInfoForceUpdateSizeCommand?.Execute(null);
                MainGoalForceUpdateSizeCommand?.Execute(null);
                MiniGoalsForceUpdateSizeCommand?.Execute(null);
                CpaxForceUpdateSizeCommand?.Execute(null);

                parameters.TryGetValue(Constants.Keys.IsNewPatientRecord, out bool isNewPatientRecord);

                //expand views if this is a new patient record
                //collapse views if this is not a new patient record
                CpaxViewIsExpanded =
                    MainGoalViewIsExpanded =
                    MiniGoalsViewIsExpanded =
                    PersonalInfoViewIsExpanded =
                    isNewPatientRecord;

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

                var r = await ShowDialogAsync(Navigation.DialogKeys.PatientFormDialog,
                    new Prism.Services.Dialogs.DialogParameters { { nameof(Patient), patient } });

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
                            Patient = newPatient;

                        }
                        catch (Exception postException)
                        {
                            shouldRetryPost = await DisplayDialog("Save Failed. Retry?", postException.Message, "Retry", "Cancel");
                            await Task.Delay(.5.Seconds());
                        }
                    } while (shouldRetryPost);

                    PersonalInfoForceUpdateSizeCommand?.Execute(null);

                }
            }
            catch (Exception e)
            {
                Logger.Log(e);
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
            .ObservesProperty(() => IsNotBusy)
            .ObservesProperty(() => Patient);

        private async Task DeleteGoalCommandExecute(Goal goal)
        {
            if (goal is null || IsBusy) return;

            try
            {
                await Constants.URLs.GoalsApi.AppendPathSegment(goal.Id).DeleteAsync();


                if (goal.IsMainGoal is true)
                    Patient.MainGoal = null;
                else
                    Patient.MiniGoals.Remove(goal);

                RaisePropertyChangedPatient();

                MainGoalForceUpdateSizeCommand?.Execute(null);
                MiniGoalsForceUpdateSizeCommand?.Execute(null);

            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
            finally { }
        }

        #endregion

        #region AddMainGoalCommand

        public ICommand AddMainGoalCommand => addMainGoalCommand ??=
            new DelegateCommand(async () => await AddMainGoalCommandExecute(), AddMainGoalCommandCanExecute)
            .ObservesProperty(() => IsNotBusy)
            .ObservesProperty(() => Patient);

        private bool AddMainGoalCommandCanExecute() => Patient != null && Patient.MainGoal is null;

        private async Task AddMainGoalCommandExecute()
        {
            try
            {
                var goalValue = await PageDialogService
                     .DisplayPromptAsync(
                    "New Main Goal",
                    "Please type in the patient's main goal",
                    maxLength: 450,
                    keyboardType: Prism.AppModel.KeyboardType.Plain);

                if (!string.IsNullOrWhiteSpace(goalValue))
                {
                    Patient.MainGoal = await Constants.URLs.GoalsApi
                        .PostJsonAsync(new Goal
                        {
                            PatientId = Patient.Id.Value,
                            IsMainGoal = true,
                            Value = goalValue
                        })
                        .ReceiveJson<Goal>();

                    RaisePropertyChangedPatient();
                    MainGoalForceUpdateSizeCommand?.Execute(null);
                }
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        #endregion

        #region AddGoalCommand

        public ICommand AddGoalCommand => addGoalCommand ??=
            new DelegateCommand(async () => await AddGoalCommandExecute())
            .ObservesProperty(() => IsNotBusy)
            .ObservesProperty(() => Patient);

        private async Task AddGoalCommandExecute()
        {
            try
            {
                var goalValue = await PageDialogService
                     .DisplayPromptAsync(
                    "New Goal",
                    "Please type in the patient's new goal",
                    maxLength: 450,
                    keyboardType: Prism.AppModel.KeyboardType.Plain);

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

                    if (Patient.MiniGoals is null)
                        Patient.MiniGoals = new List<Goal>();

                    Patient.MiniGoals.Add(newGoal);

                    RaisePropertyChangedPatient();
                    MiniGoalsForceUpdateSizeCommand?.Execute(null);
                }
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        #endregion

        #region SaveCpaxCommand

        public ICommand SaveCpaxCommand => saveCpaxCommand ??=
            new DelegateCommand(async () => await SaveCpaxCommandExecute())
            .ObservesProperty(() => IsNotBusy)
            .ObservesProperty(() => Patient);

        private async Task SaveCpaxCommandExecute()
        {
            try
            {
                SetIsBusy();

                var t = Constants.URLs.CpaxApi
                      .PostJsonAsync(new CpaxDTO
                      {
                          CurrentCpax = Patient.CurrentCPAX,
                          GoalCpax = Patient.GoalCPAX
                      });

                var payload = await t.ReceiveJson<CpaxDTO>();

                Patient.CurrentCPAX = payload.CurrentCpax;
                Patient.GoalCPAX = payload.GoalCpax;

                RaisePropertyChangedPatient();
                CpaxForceUpdateSizeCommand?.Execute(null);
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
            finally
            {
                ClearIsBusy();
            }
        }

        #endregion

        #region DeleteImageCommand

        public ICommand DeleteImageCommand => _deleteImageCommand ??=
            new DelegateCommand<ImageFile>(async imageUri => await DeleteImageCommandExecute(imageUri, Patient.Id.Value))
            .ObservesProperty(() => IsNotBusy)
            .ObservesProperty(() => Patient);

        private async Task DeleteImageCommandExecute(ImageFile imageFile, long patientId)
        {
            if (imageFile is null || IsBusy) return;

            try
            {
                await Constants.URLs.PatientImagesApi
                    .AppendPathSegments(patientId, imageFile.CategoryId, imageFile.FileName)
                    .DeleteAsync();

                var group = Patient.Images.FirstOrDefault(category => category.Id == imageFile.CategoryId);

                if (group != null)
                {
                    group.ImageFiles.Remove(imageFile);

                    RaisePropertyChangedPatient();
                }
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
            finally { }
        }

        #endregion

        #region AddImageCommand
        public ICommand AddImageCommand => addImageCommand ??=
            new DelegateCommand<ImageCategory>(async imageCategory => await AddImageCommandExecute(imageCategory, Patient.Id.Value))
            .ObservesProperty(() => IsNotBusy)
            .ObservesProperty(() => Patient);

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
                    .ReceiveJson<List<ImageFile>>())
                    .FirstOrDefault(); //we're uploading only one image

                //display the added image
                if (serverFileName != null)
                {
                    var group = Patient.Images.FirstOrDefault(f => f.Id == imageCategory.Id);
                    if (group != null)
                    {
                        group.ImageFiles.Add(serverFileName);

                        RaisePropertyChangedPatient();
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

        #endregion

        private void RaisePropertyChangedPatient()
        {
            //RaisePropertyChanged(nameof(this.Patient)); //does not work for nested objects

            var temp = Patient;
            Patient = null;
            Patient = temp;

        }
    }
}