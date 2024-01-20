using System;
using System.Globalization;
using System.Windows.Data;

namespace FinanceApp.MVVM
{
    public static class DateRangeToChartSettingsConverter
    {
        public static (int ChartMajorStep, string ChartMajorStepUnit) GetChartSettings(DateTime startDate, DateTime endDate)
        {
            var differenceInDays = (endDate - startDate).TotalDays;

            if (differenceInDays <= 7) // Less than or equal to 1 week
            {
                return (1, "Day");
            }
            else if (differenceInDays <= 30) // Less than or equal to 1 month
            {
                return (7, "Day");
            }
            else // Default to 3 months
            {
                return (1, "Month");
            }
        }
    }
}
