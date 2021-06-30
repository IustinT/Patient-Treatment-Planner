using ICU.Data.Models;
using Xamarin.Forms;

namespace ICU.Planner.Controls
{
    public partial class CpaxView
    {

        public CpaxView()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty CurrentCpaxProperty =
            BindableProperty
            .Create(nameof(CurrentCpax), typeof(CPAX), typeof(CpaxView), null, BindingMode.OneWay);

        public static readonly BindableProperty GoalCpaxProperty =
            BindableProperty
            .Create(nameof(GoalCpax), typeof(CPAX), typeof(CpaxView), null, BindingMode.OneWay);

        public CPAX CurrentCpax
        {
            get => (CPAX)GetValue(CurrentCpaxProperty);
            set => SetValue(CurrentCpaxProperty, value);
        }

        public CPAX GoalCpax
        {
            get => (CPAX)GetValue(GoalCpaxProperty);
            set => SetValue(GoalCpaxProperty, value);
        }
    }
}