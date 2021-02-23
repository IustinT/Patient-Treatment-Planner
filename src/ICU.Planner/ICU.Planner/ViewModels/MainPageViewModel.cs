﻿using Flurl;
using Flurl.Http;

using Humanizer;

using ICU.Data.Models;

using Prism.Commands;
using Prism.Magician;

using Shiny;
using Shiny.Logging;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
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

        public MainPageViewModel(BaseServices baseServices, IMainThread mainThread)
            : base(baseServices)
        {
            Patients = new ObservableCollection<Patient>();
            MainThread = mainThread;
            Title = "ICU Planner - Find Patient";

            phoneNumberSubject
                .DistinctUntilChanged()
                .Where(phoneNumber => phoneNumber != null && phoneNumber.Length > 5)
                .Throttle(500.Milliseconds())
                .SubscribeAsync(phoneNumber => Log.SafeExecute(() => SearchPatientRecords(phoneNumber)))
                .DisposedBy(Disposables);

        }

        #endregion

        #region Properties

        public IMainThread MainThread { get; }

        public string PatientPhoneNumber
        {
            get => patientPhoneNumber;
            set => SetProperty(ref patientPhoneNumber, value, () => phoneNumberSubject.OnNext(value));
        }

        [Bindable] public bool IsSearching { get; set; }

        public ObservableCollection<Patient> Patients { get; }
        #endregion

        #region Commands

        #region CreateNewPatientRecordCommand

        public ICommand CreateNewPatientRecordCommand => _createNewPatientRecordCommand ??=
            new DelegateCommand(async () => await CreateNewPatientRecordCommandExecute(PatientPhoneNumber), GoCommandCanExecute)
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
                var parameters = new Prism.Services.Dialogs.DialogParameters { { Constants.Keys.PatientIdentifierKey, phoneNumber } };

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
                            Log.Write(postException);
                            shouldRetryPost = await DisplayDialog("Save Failed. Retry?", postException.Message, "Retry", "Cancel");
                            await Task.Delay(.5.Seconds());
                        }
                    } while (shouldRetryPost);

                    //redirect to the patient overview page if record was saved. Otherwise display the phone number
                    if (newPatientRecord.Id.HasValue)
                    {
                        PatientPhoneNumber = null;
                        await HandleNavigationRequest(Navigation.NavigationKeys.PatientOverviewPage, (nameof(Patient), newPatientRecord));
                    }
                    else
                        PatientPhoneNumber = newPatientRecord.PhoneNumber;
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

        #endregion

        #region PatientSelectionChangedCommand

        public ICommand PatientSelectionChangedCommand => _patientSelectionChangedCommand ??=
            new DelegateCommand<Patient>(async patient => await PatientSelectionChangedCommandExecute(patient), PatientSelectionChangedCommandCanExecute)
            .ObservesProperty(() => IsNotBusy)
            .ObservesProperty(() => Patients);

        private bool PatientSelectionChangedCommandCanExecute(Patient patient) => IsNotBusy && patient != null;

        private async Task PatientSelectionChangedCommandExecute(Patient selectedPatient)
        {
            if (IsBusy || selectedPatient is null) return;


            try
            {
                SetIsBusy();
                var r = await HandleNavigationRequest(Navigation.NavigationKeys.PatientOverviewPage, (nameof(Patient), selectedPatient));

            }
            catch (Exception e)
            {
                Log.Write(e);
            }
            finally
            {
                PatientPhoneNumber = null;
                ClearIsBusy();
            }
        }

        #endregion

        public ICommand SearchCommand => _searchCommand ??=
            new DelegateCommand(() => phoneNumberSubject.OnNext(PatientPhoneNumber));

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
                    Patients.Clear();
                    patientRecords.ForEach(record => Patients.Add(record));
                });
            }
            catch (Exception e)
            {
                Log.Write(e);
            }
            finally
            {
                IsSearching = false;
            }
        }

        #endregion

    }
}