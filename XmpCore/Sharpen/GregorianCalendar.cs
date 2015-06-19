using System;

namespace Sharpen
{
    public enum GregorianCalendarEnum
    {
        January = 0
    }

    public class GregorianCalendar : Calendar
    {
        public const int January = (int) GregorianCalendarEnum.January;

        public GregorianCalendar()
        {
        }

        public GregorianCalendar(TimeZoneInfo timeZoneInfo) : base(timeZoneInfo)
        {
        }

        public GregorianCalendar(int year, int month, int day) : base(year, month, day)
        {
        }

        public GregorianCalendar(int year, int month, int dayOfMonth, int hourOfDay, int minute, int second)
            : base(year, month, dayOfMonth, hourOfDay, minute, second)
        {
        }

        public void SetGregorianChange(DateTime date)
        {
        }

        public override int GetMaximum(CalendarEnum field)
        {
            switch (field)
            {
                case CalendarEnum.Year:
                    return DateTime.MaxValue.Year;

                case CalendarEnum.Month:
                    return 11;

                case CalendarEnum.MonthOneBased:
                    return 12;

                case CalendarEnum.DayOfMonth:
                    return DateTime.DaysInMonth(GetTime().Year, GetTime().Month);

                case CalendarEnum.Hour:
                    return 23;

                case CalendarEnum.HourOfDay:
                    return 23;

                case CalendarEnum.Minute:
                    return 60;

                case CalendarEnum.Second:
                    return 60;

                case CalendarEnum.Millisecond:
                    return 999;
            }

            throw new NotSupportedException();
        }
    }
}