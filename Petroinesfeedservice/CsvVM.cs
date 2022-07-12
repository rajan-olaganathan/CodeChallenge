namespace Petroineosfeedservice
{
    public class CsvVM
    {
        public CsvVM(int period, double volume)
        {
            LocalTime = CalculateLocatTime(period);
            Volume = volume;
        }
        public TimeSpan LocalTime { get; }

        public double Volume { get; }

        private TimeSpan CalculateLocatTime(int period)
        {
            int startHour = 23;
            int resolvedHour = (startHour + (period - 1)) % 24;
            return TimeSpan.FromHours(resolvedHour);
        }
    }
}