using Flurl;
using Flurl.Http;
using Humanizer;
using ICU.Data.Models;
using Prism.Commands;
using Prism.Logging;
using Prism.Magician;
using ShinyExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials.Interfaces;

namespace ICU.Planner.ViewModels
{
    public partial class MainPageViewModel : IntermediaryViewModelBase
    {

        #region Fields

        private readonly Subject<string> phoneNumberSubject = new Subject<string>();

        private ICommand _createNewPatientRecordCommand;
        private ICommand _patientSelectionChangedCommand;
        private ICommand _searchCommand;
        private string patientPhoneNumber;

        #endregion

        #region Ctor

        public MainPageViewModel(BaseServices baseServices,
                                 IMainThread mainThread)
            : base(baseServices)
        {
            Patients = new ObservableCollection<Patient>();
            MainThread = mainThread;
            Title = "ICU Planner - Find Patient";

            phoneNumberSubject
                .Select(phoneNumber => phoneNumber?.Trim())
                .DistinctUntilChanged() // - interferes with the pull to refresh as it never executes
                //.Where(phoneNumber => phoneNumber != null && phoneNumber.Length > 5)
                .Throttle(500.Milliseconds())
                .Subscribe(async phoneNumber =>
                {
                    if (phoneNumber is null || phoneNumber.Length < Constants.MinimumDigitsForPatientSearch)
                    {
                        if (Patients.Any())
                            await MainThread.InvokeOnMainThreadAsync(() => { Patients.Clear(); });
                    }
                    else if (phoneNumber.Length >= Constants.MinimumDigitsForPatientSearch)
                        await SearchPatientRecords(phoneNumber);
                })
                .DisposedBy(Disposables);

        }

        #endregion

        #region Properties

        public IMainThread MainThread { get; }

        /// <summary>
        /// The text displayed to the user when no patient records are displayed
        /// </summary>
        [Bindable] public string EmptyPatientsListText { get; set; }

        public string PatientPhoneNumber
        {
            get => patientPhoneNumber;
            set
            {
                SetProperty(ref patientPhoneNumber, value, () => phoneNumberSubject.OnNext(value));

                EmptyPatientsListText = string.IsNullOrEmpty(value) switch
                {
                    true => "Please type a (partial) phone number in the search box.",
                    false => value.Length switch
                    {
                        < Constants.MinimumDigitsForPatientSearch =>
                            $"Please type {Constants.MinimumDigitsForPatientSearch} or more digits.",

                        _ => null
                    }
                };
            }
        }

        [Bindable] public bool IsSearching { get; set; }

        public ObservableCollection<Patient> Patients { get; }

        #endregion

        #region Commands

        #region CreateNewPatientRecordCommand

        public ICommand CreateNewPatientRecordCommand => _createNewPatientRecordCommand ??=
            new DelegateCommand(async () => await CreateNewPatientRecordCommandExecute(PatientPhoneNumber),
                    GoCommandCanExecute)
                .ObservesProperty(() => IsNotBusy)
                .ObservesProperty(() => PatientPhoneNumber);

        private bool GoCommandCanExecute() =>
            IsNotBusy && !string.IsNullOrEmpty(PatientPhoneNumber) && PatientPhoneNumber.Length == 11;

        private async Task CreateNewPatientRecordCommandExecute(string phoneNumber)
        {
            if (IsBusy || string.IsNullOrWhiteSpace(phoneNumber)) return;

            if (phoneNumber.Length != 11)
            {
                await DisplayDialog("Patient Identifier", "Patient phone number must be 11 characters long");
                return;
            }

            try
            {
                SetIsBusy();

                //display the new patient dialog
                var parameters = new Prism.Services.Dialogs.DialogParameters
                    { { Constants.Keys.PatientIdentifierKey, phoneNumber } };

                var r = await ShowDialogAsync(Navigation.DialogKeys.PatientFormDialog, parameters);

                if (r.Exception != null) throw r.Exception;

                //try to get the new patient details and attempt to save them on server
                if (r.Parameters.TryGetValue(nameof(Patient), out Patient newPatientRecord))
                {
                    bool shouldRetryPost = true;
                    do
                    {
                        try
                        {
                            newPatientRecord = await Constants.URLs.PatientsApi
                                .PostJsonAsync(newPatientRecord)
                                .ReceiveJson<Patient>();
                            shouldRetryPost = false;
                        }
                        catch (Exception postException)
                        {
                            Logger.Log(postException);
                            shouldRetryPost = await DisplayDialog("Save Failed. Retry?", postException.Message, "Retry",
                                "Cancel");
                            await Task.Delay(.5.Seconds());
                        }
                    } while (shouldRetryPost);

                    //redirect to the patient overview page if record was saved. Otherwise display the phone number
                    if (newPatientRecord.Id.HasValue)
                    {
                        PatientPhoneNumber = null;

                        SetDefaultCpaxObjects(newPatientRecord);

                        await HandleNavigationRequest(Navigation.NavigationKeys.PatientOverviewPage,
                            (nameof(Patient), newPatientRecord),
                            (Constants.Keys.IsNewPatientRecord, true));
                    }
                    else
                        PatientPhoneNumber = newPatientRecord.PhoneNumber;
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

        #endregion

        #region PatientSelectionChangedCommand

        public ICommand PatientSelectionChangedCommand => _patientSelectionChangedCommand ??=
            new DelegateCommand<Patient>(async patient => await PatientSelectionChangedCommandExecute(patient),
                    PatientSelectionChangedCommandCanExecute)
                .ObservesProperty(() => IsNotBusy)
                .ObservesProperty(() => Patients);

        private bool PatientSelectionChangedCommandCanExecute(Patient patient) => IsNotBusy && patient != null;

        private async Task PatientSelectionChangedCommandExecute(Patient selectedPatient)
        {
            if (IsBusy || selectedPatient is null) return;

            try
            {
                SetIsBusy();

                //get all the patient data
                var patient = await Constants.URLs.PatientsApi
                    .AppendPathSegment(selectedPatient.Id)
                    .GetJsonAsync<Patient>();

                //get all the patient data
                var exercises = await Constants.URLs.ExercisesApi
                    .GetJsonAsync<List<ExerciseCategory>>();

                SetDefaultCpaxObjects(patient);

                var r = await HandleNavigationRequest(Navigation.NavigationKeys.PatientOverviewPage,
                    (nameof(Patient), patient), ("ExerciseCategories", exercises));

            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
            finally
            {
                PatientPhoneNumber = null;
                Patients.Clear();
                ClearIsBusy();
            }
        }

        #endregion

        #region SearchCommand

        public ICommand SearchCommand => _searchCommand ??=
            new DelegateCommand(() => phoneNumberSubject.OnNext(PatientPhoneNumber));

        #endregion

        #endregion


        #region GetData

        private async Task SearchPatientRecords(string phoneNumber)
        {
            try
            {
                IsSearching = true;

                var task = Constants.URLs.PatientsApi
                    .AppendPathSegments("Search", phoneNumber);

                var patientRecords = await task.GetJsonAsync<List<Patient>>();

                IsSearching = false;

                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (!patientRecords.Any())
                        EmptyPatientsListText = $@"No records found for ""{phoneNumber}""";

                    Patients.Clear();
                    patientRecords.ForEach(record => Patients.Add(record));
                });
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
            finally
            {
                IsSearching = false;
            }
        }

        #endregion

        private static void SetDefaultCpaxObjects(Patient patient)
        {
            if (patient.CurrentCPAX is null) patient.CurrentCPAX = new CPAX { PatientId = patient.Id.Value };
            if (patient.GoalCPAX is null) patient.GoalCPAX = new CPAX { PatientId = patient.Id.Value };
        }
    }
}
