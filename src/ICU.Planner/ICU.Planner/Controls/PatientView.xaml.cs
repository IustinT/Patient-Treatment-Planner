using ICU.Data.Models;

using System;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace ICU.Planner.Controls
{
    public partial class PatientView
    {
        readonly bool isIos;
        public IDeviceInfo DeviceInfo { get; }

        public PatientView()
        {
            InitializeComponent();
            DeviceInfo = Shiny.ShinyHost.Resolve<IDeviceInfo>();
            isIos = DeviceInfo.Platform == DevicePlatform.iOS;
        }

        public static readonly BindableProperty PatientProperty =
            BindableProperty.Create(nameof(Patient), typeof(Patient), typeof(PatientView));

        public Patient Patient { get => (Patient)GetValue(PatientProperty); set => SetValue(PatientProperty, value); }
        private void DatePickerContainer_OnFocused(object sender, EventArgs e)
        {
            if (!DatePicker.IsFocused)
                DatePicker.Focus();
        }

        private async void DateValueLabel_Focused(object sender, FocusEventArgs e)
        {
            if (!DatePicker.IsVisible)
            {
                DatePicker.IsVisible = true;

                if (!DatePicker.IsFocused)
                {
                    if (isIos)
                        await Task.Delay(250);

                    DatePicker.Focus();
                }
            }
        }

        private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            if (DatePicker.IsVisible)
                DatePicker.IsVisible = false;
        }

        private void DatePicker_Unfocused(object sender, FocusEventArgs e)
        {
            if (DatePicker.IsVisible)
                DatePicker.IsVisible = false;
        }
    }
}