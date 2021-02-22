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

using Shiny;
using Shiny.Logging;

namespace ICU.Planner.ViewModels
{
    [AutoInitialize]
    public partial class PatientOverviewPageViewModel : IntermediaryViewModelBase
    {
        private readonly Subject<long> patientIdChangedSubject = new Subject<long>();

        private ICommand editPatientCommand;
        private ICommand deleteGoalCommand;
        private ICommand addMainGoalCommand;
        private DelegateCommand addGoalCommand;
        private List<Goal> _goals;

        protected PatientOverviewPageViewModel(BaseServices baseServices) : base(baseServices)
        {
            Title = "Patient Page";
            _goals = new List<Goal>();

            patientIdChangedSubject
                .Throttle(.5.Seconds())
                .SubscribeAsync(id => Log.SafeExecute(() => RefreshGoals(id)))
                .DisposedBy(Disposables);
        }

        #region Properties

        [Bindable] public Patient Patient { get; set; }

        public List<Goal> Goals { get => _goals; set => SetProperty(ref _goals, value); }
        [Bindable] public Goal MainGoal { get; set; }

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

        #endregion

        #region GetData
        private async Task RefreshGoals(long patientId)
        {
            var goals = await Constants.URLs.GoalsApi.AppendPathSegment(patientId)
                .GetJsonAsync<List<Goal>>();

            MainGoal = goals.FirstOrDefault(goal => goal.IsMainGoal is true);
            Goals = goals.Where(goal => goal.IsMainGoal != true).ToList();
        }
        #endregion
    }
}