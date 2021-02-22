
using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

namespace ICU.Planner.Extensions
{
    public static class ButtonExtension
    {
        public static BindableProperty FontImageSourceProperty =
            BindableProperty.CreateAttached("FontImageSource", typeof(FontImageSource),
                typeof(ButtonExtension), null, propertyChanged: HandleChanged);

        public static FontImageSource GetFontImageSourceProperty(BindableObject view)
        {
            return (FontImageSource)view.GetValue(FontImageSourceProperty);
        }

        public static void SetFontImageSourceProperty(BindableObject view, FontImageSource source)
        {
            view.SetValue(FontImageSourceProperty, source);
        }

        static void HandleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is Button button)
                button.ImageSource = (FontImageSource)newValue;
            else if (bindable is ImageButton imageButton)
                imageButton.Source = (FontImageSource)newValue;

        }
    }
}
