using Android.Content;
using ICU.Planner.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BorderlessPicker), typeof(ICU.Planner.Droid.Renderers.BorderlessPickerRenderer))]

namespace ICU.Planner.Droid.Renderers
{
    public class BorderlessPickerRenderer : PickerRenderer
    {
        public BorderlessPickerRenderer(Context context) : base(context)
        { }

    }
}
