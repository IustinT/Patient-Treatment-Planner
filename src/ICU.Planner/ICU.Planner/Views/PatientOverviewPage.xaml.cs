using System;
using System.Collections.Generic;
using System.Linq;
using DynamicData;
using DynamicData.Binding;
using ICU.Data.Models;
using ICU.Planner.Controls;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace ICU.Planner.Pages
{
    public partial class PatientOverviewPage
    {
        private static readonly int[] RepetitionsInPlanOptions = Enumerable.Range(1, 15).ToArray();

        public PatientOverviewPage()
        {
            InitializeComponent();
        }

        private void ExercisesListContainer_OnBindingContextChanged(object sender, EventArgs e)
        {
            if (sender is not Layout<View> layout) return;

            layout.Children.Clear();

            layout.Children.Add(GetViewForExercises(layout));
        }

        private View GetViewForExercises(BindableObject layout)
        {
            if (layout.BindingContext is null
                || (layout.BindingContext is IList<Exercise> emptyList && !emptyList.Any()))
                return new Label { Text = "No Exercises" };

            if (layout.BindingContext is IList<Exercise> exercises)
            {
                var colDef = new ColumnDefinitionCollection
                {
                    new() { Width = GridLength.Star },
                    new() { Width = new GridLength(1, GridUnitType.Star) },
                    new() { Width = GridLength.Auto }
                };

                var rowDefinitions = new RowDefinitionCollection();
                rowDefinitions.AddRange(
                    Enumerable.Range(0, exercises.Count + 1)
                        .Select(_ => new RowDefinition
                        {
                            Height = new GridLength(1, GridUnitType.Star)
                        })
                );

                var grid = new Grid
                {
                    ColumnDefinitions = colDef,
                    ColumnSpacing = 5,
                    RowDefinitions = rowDefinitions
                };

                //add header elements
                var nameHeader = new Label { Text = "Name", HorizontalTextAlignment = TextAlignment.Center };
                Grid.SetColumn(nameHeader, 0);

                var repetitionsHeader = new Label
                    { Text = "Repetitions", HorizontalTextAlignment = TextAlignment.Center };
                Grid.SetColumn(repetitionsHeader, 1);

                var checkboxHeader = new Label { Text = "In Plan", HorizontalTextAlignment = TextAlignment.Center };
                Grid.SetColumn(checkboxHeader, 2);

                grid.Children.Add(nameHeader);
                grid.Children.Add(repetitionsHeader);
                grid.Children.Add(checkboxHeader);

                for (var index = 0; index < exercises.Count; index++)
                {
                    var rowNumber = index + 1;
                    var exercise = exercises[index];

                    var nameLabel = new Label { Text = exercise.Name, BindingContext = exercise };
                    nameLabel.SetBinding(Label.TextProperty, nameof(Exercise.Name));

                    var inPlanCheckbox = new CheckBox
                        { BindingContext = exercise, HorizontalOptions = LayoutOptions.Center };
                    inPlanCheckbox.SetBinding(CheckBox.IsCheckedProperty, nameof(Exercise.IsIncludedInPlan));

                    var repetitionsPicker = new BorderlessPicker
                    {
                        BindingContext = exercise,
                        ItemsSource = RepetitionsInPlanOptions,
                        HorizontalTextAlignment = TextAlignment.Center,
                        Margin = 0,
                        //set repetitions value, default to the first entry in the options list
                        SelectedItem = exercise.RepetitionsInPlan is 0
                            ? RepetitionsInPlanOptions[0]
                            : exercise.RepetitionsInPlan,

                    };

                    repetitionsPicker.PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName == IsVisibleProperty.PropertyName && sender is View v)
                            Grid.SetColumnSpan(nameLabel, v.IsVisible ? 1 : 2);
                    };

                    //set binding
                    repetitionsPicker.SetBinding(Picker.SelectedItemProperty, nameof(Exercise.RepetitionsInPlan));

                    //bind IsVisible to be displayed only when included in plan
                    repetitionsPicker.SetBinding(IsVisibleProperty,
                        new Binding(nameof(CheckBox.IsChecked), BindingMode.OneWay, source: inPlanCheckbox));

                    //position in Grid

                    Grid.SetColumn(nameLabel, 0);
                    Grid.SetColumn(repetitionsPicker, 1);
                    Grid.SetColumn(inPlanCheckbox, 2);

                    Grid.SetRow(nameLabel, rowNumber);
                    Grid.SetRow(repetitionsPicker, rowNumber);
                    Grid.SetRow(inPlanCheckbox, rowNumber);

                    //todo test without name and google
                    grid.Children.Add(nameLabel);
                    grid.Children.Add(repetitionsPicker);
                    grid.Children.Add(inPlanCheckbox);

                }

                return grid;
            }

            var type = layout.BindingContext.GetType();
            return new Label { Text = $"Unable to display exercises: {type}" };


        }
    }
}
