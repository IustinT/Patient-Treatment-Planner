using Xamarin.Forms;

namespace ICU.Planner.Controls
{
    public partial class ExpanderIconView
    {
        public ExpanderIconView()
        {
            InitializeComponent();

        }

        public static readonly BindableProperty IsExpandedProperty =
            BindableProperty
            .Create(nameof(IsExpanded), typeof(bool), typeof(ExpanderIconView));

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }
    }
}