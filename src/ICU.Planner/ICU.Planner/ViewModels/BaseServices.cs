
using Prism.Events;
using Prism.Magician;
using Prism.Services;

namespace ICU.Planner.ViewModels
{
    [BaseServices]
    public partial class BaseServices
    {
        public IDeviceService DeviceService { get; set; }
        public IEventAggregator EventAggregator { get; set; }

    }
}
