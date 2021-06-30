using Xamarin.Forms;

namespace ICU.Planner.Controls
{
    public partial class CpaxPropertyView
    {

        public CpaxPropertyView()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty HeaderValueProperty =
            BindableProperty
            .Create(nameof(HeaderValue), typeof(string), typeof(CpaxPropertyView), null, BindingMode.OneTime);

        public static readonly BindableProperty CurrentValueProperty =
            BindableProperty
            .Create(nameof(CurrentValue), typeof(int), typeof(CpaxPropertyView), null, BindingMode.TwoWay);

        public static readonly BindableProperty GoalValueProperty =
            BindableProperty
            .Create(nameof(GoalValue), typeof(int), typeof(CpaxPropertyView), null, BindingMode.TwoWay);

        public string HeaderValue
        {
            get => (string)GetValue(HeaderValueProperty);
            set => SetValue(HeaderValueProperty, value);
        }

        public int CurrentValue
        {
            get => (int)GetValue(CurrentValueProperty);
            set => SetValue(CurrentValueProperty, value);
        }

        public int GoalValue
        {
            get => (int)GetValue(GoalValueProperty);
            set => SetValue(GoalValueProperty, value);
        }

    }
}