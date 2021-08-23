using System;
using System.Globalization;
using Xamarin.CommunityToolkit.Extensions.Internals;
using Xamarin.Forms;

namespace ICU.Planner.Converters
{
    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to a <see cref="int"/> value expressed in minutes.
    /// </summary>
    public class TimespanMinutesConverter : ValueConverterExtension, IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="int"/> (value should be in minutes) to a <see cref="TimeSpan"/> value.
        /// </summary>
        /// <param name="value">The <see cref="int"/> value (in minutes) to convert.</param>
        /// <param name="targetType">The type of the binding target property. This is not implemented.</param>
        /// <param name="parameter">Additional parameter for the converter to handle. This is not implemented.</param>
        /// <param name="culture">The culture to use in the converter. This is not implemented.</param>
        /// <returns>The <see cref="TimeSpan"/> value representing the converted <see cref="int"/> value.</returns>
        public object Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture) =>
            value switch
            {
                int intValue => TimeSpan.FromMinutes(intValue),
                _ => TimeSpan.Zero
            };

        /// <summary>
        /// Converts a <see cref="TimeSpan"/> to a <see cref="int"/> value expressed in minutes.
        /// </summary>
        /// <param name="value">The <see cref="TimeSpan"/> value to convert.</param>
        /// <param name="targetType">The type of the binding target property. This is not implemented.</param>
        /// <param name="parameter">Additional parameter for the converter to handle. This is not implemented.</param>
        /// <param name="culture">The culture to use in the converter. This is not implemented.</param>
        /// <returns>A <see cref="int"/> value expressed in seconds.</returns>
        public object ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture) =>
            value switch
            {
                TimeSpan timespan => System.Convert.ToInt32(Math.Round(timespan.TotalMinutes, MidpointRounding.ToEven)),
                _ => 0
            };

    }
}
