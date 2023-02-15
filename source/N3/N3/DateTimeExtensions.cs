namespace N3
{
    public static class DateTimeExtensions
    {
        public static DateOnly ToDateOnly(this DateTime date)
        {
            return new DateOnly(date.Year, date.Month, date.Day);
        }
    }
}
