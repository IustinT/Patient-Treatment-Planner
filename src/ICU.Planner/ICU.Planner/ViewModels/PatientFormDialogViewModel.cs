using System;
using System.Threading.Tasks;
using System.Windows.Input;

using ICU.Data.Models;

using Prism.Commands;
using Prism.Magician;
using Prism.Navigation;
using Prism.Services.Dialogs;

using Shiny.Logging;

namespace ICU.Planner.ViewModels
{
    [AutoInitialize]
    public partial class PatientFormDialogViewModel : IntermediaryViewModelBase, IDialogAware
    {
        private ICommand _saveCommand;
        private Patient patient;

        public PatientFormDialogViewModel()
        { }

        protected PatientFormDialogViewModel(BaseServices baseServices) : base(baseServices)
        {
            Title = "Patient Info";
            ClearIsBusy();
        }

        #region Properties

        [Bindable] public bool IsSaveButtonVisible { get; set; }
        [Bindable] public bool IsSubmitButtonVisible { get; set; }
        public Patient Patient { get => patient; set => SetProperty(ref patient, value); }

        #endregion

        #region Commands

        public ICommand SaveCommand => _saveCommand ??=
            new DelegateCommand(async () => await SaveCommandExecute(Patient), SaveCommandCanExecute)
                .ObservesProperty(() => IsBusy)
                .ObservesProperty(() => IsNotBusy)
                .ObservesProperty(() => Patient);

        private bool SaveCommandCanExecute() =>
           true;

        private async Task SaveCommandExecute(Patient patient)
        {
            if (patient is null || IsBusy) return;

            if (string.IsNullOrWhiteSpace(patient.PhoneNumber))
            {
                await DisplayDialog("Patient Identifier is required", "Please type in the Patient Identifier.");
                return;
            }

            if (string.IsNullOrWhiteSpace(patient.Name))
            {
                await DisplayDialog("Patient Name is required", "Please type in the Patient Name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(patient.Hospital))
            {
                await DisplayDialog("Hospital is required", "Please type in the Hospital Name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(patient.Ward))
            {
                await DisplayDialog("Ward is required", "Please type in the Ward Name.");
                return;
            }

            if (!patient.AdmissionDate.HasValue)
            {
                await DisplayDialog("Admission date is required", "Please select a date.");
                return;
            }

            RequestClose(new DialogParameters { { nameof(Patient), patient } });

        }

        #endregion

        #region Close Dialog

        protected override bool CloseDialogButtonPressedCommandCanExecute() => CanCloseDialog();
        protected override Task CloseDialogButtonPressedCommandExecute()
        {
            RaiseRequestClose();

            return base.CloseDialogButtonPressedCommandExecute();
        }

        #endregion

        #region Dialog

        public event Action<IDialogParameters> RequestClose;

        public bool CanCloseDialog() => IsNotBusy;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            const string patientKey = nameof(Patient);

            if (parameters.ContainsKey(patientKey) && parameters.ContainsKey(Constants.Keys.PatientIdentifierKey))
            {
                RaiseRequestClose();

                Log.Write($"{nameof(PatientFormDialogViewModel)} received bad parameters",
                    $"Received both {patientKey} and {Constants.Keys.PatientIdentifierKey} but was expencting only one of them.");

                return;
            }

            if (parameters.TryGetValue(patientKey, out Patient patient))
            {
                Patient = patient;
                IsSubmitButtonVisible = true;
            }

            if (patient is null
                && parameters.TryGetValue(Constants.Keys.PatientIdentifierKey, out string patientIdentifier)
                && !string.IsNullOrWhiteSpace(patientIdentifier))
            {
                Patient = new Patient { PhoneNumber = patientIdentifier.Trim(), AdmissionDate = DateTime.Today };
                IsSaveButtonVisible = true;
            }

            if (Patient is null)
            {
                RaiseRequestClose();
                Log.Write($"{nameof(PatientFormDialogViewModel)} received bad parameters",
                    $"{nameof(PatientFormDialogViewModel)} dis not receive a value for {patientKey} or {Constants.Keys.PatientIdentifierKey}");
            }

            return;
        }
        #endregion


    }
}