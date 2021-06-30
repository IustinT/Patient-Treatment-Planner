using Xamarin.Forms;

namespace ICU.Planner.Controls
{
    public partial class CpaxRadioButtonsView
    {

        public CpaxRadioButtonsView()
        {
            InitializeComponent();

        }

        public static readonly BindableProperty ValueProperty =
          BindableProperty
          .Create(nameof(Value), typeof(int), typeof(CpaxRadioButtonsView), null, BindingMode.TwoWay, propertyChanged: OnValueChanged);


        public static readonly BindableProperty ChechedBgColorProperty =
          BindableProperty
          .Create(nameof(Value), typeof(Color), typeof(CpaxRadioButtonsView), Color.Orange);

        public static readonly BindableProperty ChechedFontColorProperty =
          BindableProperty
          .Create(nameof(Value), typeof(Color), typeof(CpaxRadioButtonsView), Color.Black);

        /*
         
            selected bg orange green

            Unselected font color black
            Selected font color black white?

            Unselected border color gray gray
            Selected border color red green?


         */


        public Color ChechedFontColor
        {
            get => (Color)GetValue(ChechedFontColorProperty);
            set => SetValue(ChechedFontColorProperty, value);
        }

        public Color ChechedBgColor
        {
            get => (Color)GetValue(ChechedBgColorProperty);
            set => SetValue(ChechedBgColorProperty, value);
        }

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CpaxRadioButtonsView control && newValue is int intValue)
            {

                var radioButtonToCheck = intValue switch
                {
                    1 => control.Option1Button,
                    2 => control.Option2Button,
                    3 => control.Option3Button,
                    4 => control.Option4Button,
                    5 => control.Option5Button,

                    _ => null
                };

                if (radioButtonToCheck is not null && !radioButtonToCheck.IsChecked)
                    radioButtonToCheck.IsChecked = true;

            }
        }

        void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is RadioButton rb && rb.IsChecked && rb.Value is int intValue && intValue != Value)
                Value = intValue;
        }
    }
}