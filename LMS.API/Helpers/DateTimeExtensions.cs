using System;

namespace LMS.API.Helpers
{
    public static class DateTimeExtensions
    {
        public static int GetCurrentAge(this DateTime date)
        {
            var currentDate = DateTime.Now;
            var age = currentDate.Year - date.Year;

            if (currentDate < date.AddYears(age))
            {
                age--;
            }

            return age;
        }
    }
}