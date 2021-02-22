
using Prism.Magician;

namespace ICU.Planner.ViewModels
{
    public partial class IntermediaryViewModelBase : ViewModelBase
    {
        public IntermediaryViewModelBase() : base(null)
        { }

        protected IntermediaryViewModelBase(BaseServices baseServices) : base(baseServices)
        {
            IsNotBusy = true;
        }


    }
}