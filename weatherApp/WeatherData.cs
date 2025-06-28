namespace weatherApp;

public record Header(string currentTemp, string? descr, int iconID);
public record TimeStamp(string time, string temperature, int iconID, string? humidity);
public record DayStamp(string weekDay, string date, string dayTemp, string nightTemp, int iconID);

public class WeatherData {
    public Header weatherHeader { get; }
    public TimeStamp[] timeStamps { get; } = new TimeStamp[5];
    public DayStamp[] dayStamps { get; } = new DayStamp[5];

    public WeatherData(Header weatherHeader, TimeStamp[] timeStamps, DayStamp[] dayStamps) {
        this.weatherHeader = weatherHeader;
        this.timeStamps = timeStamps;
        this.dayStamps = dayStamps;
    }
}