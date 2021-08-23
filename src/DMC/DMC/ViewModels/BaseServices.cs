
using Prism.Events;
using Prism.Magician;
using Prism.Services;

namespace DMC.ViewModels
{
    [BaseServices]
    public partial class BaseServices
    {
        public IDeviceService DeviceService { get; set; }
        public IEventAggregator EventAggregator { get; set; }

    }
}
