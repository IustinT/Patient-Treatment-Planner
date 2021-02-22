using System.Windows.Input;

using Xamarin.Forms;

namespace ICU.Planner.Controls
{
    public partial class DialogHeader
    {
        public DialogHeader()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TitleTextProperty =
            BindableProperty.Create(nameof(TitleText), typeof(string), typeof(DialogHeader));

        public static readonly BindableProperty IsTitleTextVisibleProperty =
            BindableProperty.Create(nameof(IsTitleTextVisible), typeof(bool), typeof(DialogHeader), true);

        public static readonly BindableProperty LeftSideButtonCommandProperty =
            BindableProperty.Create(nameof(LeftSideButtonCommand), typeof(ICommand), typeof(DialogHeader));

        public static readonly BindableProperty IsLeftSideButtonVisibleProperty =
            BindableProperty.Create(nameof(IsLeftSideButtonVisible), typeof(bool), typeof(DialogHeader), true);

        public static readonly BindableProperty RightSideButtonCommandProperty =
            BindableProperty.Create(nameof(RightSideButtonCommand), typeof(ICommand), typeof(DialogHeader));

        public static readonly BindableProperty IsRightSideButtonVisibleProperty =
            BindableProperty.Create(nameof(IsRightSideButtonVisible), typeof(bool), typeof(DialogHeader), true);

        public static readonly BindableProperty RightSideButtonTextProperty =
            BindableProperty.Create(nameof(RightSideButtonText), typeof(string), typeof(DialogHeader));

        public string TitleText
        {
            get => (string)GetValue(TitleTextProperty);
            set => SetValue(TitleTextProperty, value);
        }

        public bool IsTitleTextVisible
        {
            get => (bool)GetValue(IsTitleTextVisibleProperty);
            set => SetValue(IsTitleTextVisibleProperty, value);
        }

        public ICommand LeftSideButtonCommand
        {
            get => (ICommand)GetValue(LeftSideButtonCommandProperty);
            set => SetValue(LeftSideButtonCommandProperty, value);
        }

        public bool IsLeftSideButtonVisible
        {
            get => (bool)GetValue(IsLeftSideButtonVisibleProperty);
            set => SetValue(IsLeftSideButtonVisibleProperty, value);
        }

        public ICommand RightSideButtonCommand
        {
            get => (ICommand)GetValue(RightSideButtonCommandProperty);
            set => SetValue(RightSideButtonCommandProperty, value);
        }

        public bool IsRightSideButtonVisible
        {
            get => (bool)GetValue(IsRightSideButtonVisibleProperty);
            set => SetValue(IsRightSideButtonVisibleProperty, value);
        }

        public string RightSideButtonText
        {
            get => (string)GetValue(RightSideButtonTextProperty);
            set => SetValue(RightSideButtonTextProperty, value);
        }


    }
}