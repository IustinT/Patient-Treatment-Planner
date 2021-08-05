using System;
using System.Collections.Generic;
using System.Linq;
using DynamicData;
using ICU.Data.Models;
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
                    new() { Width = GridLength.Auto },
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
                    Grid.SetColumn(nameLabel, 0);
                    Grid.SetRow(nameLabel, rowNumber);

                    var inPlanCheckbox = new CheckBox { BindingContext = exercise };
                    inPlanCheckbox.SetBinding(CheckBox.IsCheckedProperty, nameof(Exercise.IsIncludedInPlan));
                    Grid.SetColumn(inPlanCheckbox, 2);
                    Grid.SetRow(inPlanCheckbox, rowNumber);

                    var repetitionsPicker = new Picker
                    {
                        BindingContext = exercise,
                        ItemsSource = RepetitionsInPlanOptions,
                        Margin = 0
                    };
                    repetitionsPicker.SetBinding(Picker.SelectedItemProperty, nameof(Exercise.RepetitionsInPlan));
                    repetitionsPicker.SetBinding(Picker.IsVisibleProperty,
                        new Binding(nameof(CheckBox.IsChecked), BindingMode.OneWay, source: inPlanCheckbox));
                    repetitionsPicker.SelectedItem = RepetitionsInPlanOptions[0];
                    Grid.SetColumn(repetitionsPicker, 1);
                    Grid.SetRow(repetitionsPicker, rowNumber);

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
