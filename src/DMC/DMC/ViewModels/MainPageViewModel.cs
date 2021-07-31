
using Xamarin.Essentials.Interfaces;

namespace DMC.ViewModels
{
    public partial class MainPageViewModel : IntermediaryViewModelBase
    {
        #region Fields

        #endregion

        #region Ctor

        public MainPageViewModel(BaseServices baseServices,
                                 IMainThread mainThread)
                                : base(baseServices)
        {
            MainThread = mainThread;
            Title = "ICU Planner - Find Patient";


        }

        #endregion

        #region Properties

        public IMainThread MainThread { get; }

        #endregion

        #region Commands

        #endregion


        #region GetData

        #endregion

    }
}
