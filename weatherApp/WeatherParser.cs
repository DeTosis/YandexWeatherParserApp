using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace weatherApp;
public class WeatherParser {
    static string url = "https://yandex.ru/pogoda/ru/";
    public async Task<WeatherData> ParseAsync(string state = "yekaterinburg") {
        Header header;
        TimeStamp[] timeStamps;
        DayStamp[] dayStamps = new DayStamp[5];

        IDocument document = await GetPage(state);

        header = GetHeader(document);
        timeStamps = GetTimeStamps(document);
        dayStamps = GetDayStamps(document);


        return new WeatherData(header, timeStamps, dayStamps);
    }

    private DayStamp[] GetDayStamps(IDocument doc) {
        var dayElem = doc.QuerySelectorAll(".AppShortForecastContainer_days__wwS6z > div");
        DayStamp[] stamps = new DayStamp[5];

        int stampId = 0;
        for (int x = 0; x < dayElem.Length; x++) {
            if (stampId == 5)
                break;

            string weekDay;
            string date;
            string dayTemp;
            string nightTemp;
            int iconId;

            var day = dayElem[x].QuerySelectorAll("span");
            var ico = dayElem[x].QuerySelector("div > a > div");

            if (day.Count() < 4) {
                continue;
            }

            if (ico is not IElement)
                continue;

            weekDay = day[0].TextContent;
            if (x == 0) {
                date = DateTime.Now.Date.Day.ToString();
            } else {
                date = day[1].TextContent;
            }
            dayTemp = day[2].TextContent;
            nightTemp = day[3].TextContent;

            iconId = GetIconIDFromElementOuterHtml(ico.OuterHtml);

            stamps[stampId] = new(weekDay, date, dayTemp, nightTemp, iconId);
            stampId++;
        }

        return stamps;
    }

    private TimeStamp[] GetTimeStamps(IDocument doc) {
        var timeElem = doc.QuerySelectorAll(".AppHourly_list__gXAeN li");
        TimeStamp[] stamps = new TimeStamp[5];

        int stampId = 0;
        for (int x = 0; x < timeElem.Length; x++) {
            if (stampId == 5)
                break;

            string time;
            string temp;
            int iconId;
            string? humidity;

            var t = timeElem[x].QuerySelector(".AppHourlyItem_time__515ZC");
            var p = timeElem[x].QuerySelector("div > p");
            var i = timeElem[x].QuerySelector("div > div > div");
            var h = timeElem[x].QuerySelector("div > div > span");

            if (t is not IElement) {
                continue;
            }

            if (p is not IElement)
                continue;

            if (i is not IElement)
                continue;

            time = t.TextContent;

            if (time.Split(':')[1] != "00") {
                continue;
            }

            temp = p.TextContent;
            iconId = GetIconIDFromElementOuterHtml(i.OuterHtml);

            if (h is not IElement) {
                humidity = null;
            } else {
                humidity = h.TextContent;
            }

            stamps[stampId] = new(time, temp, iconId, humidity);
            stampId++;
        }


        return stamps;
    }

    private Header GetHeader(IDocument doc) {
        IElement? headerElement = doc.QuerySelector(".AppFact_wrap__N4SYB");
        if (headerElement is not IElement)
            throw new NotImplementedException();

        string temp = "";
        string desc = "";
        int iconId = -1;

        var p = headerElement.QuerySelectorAll("p");
        if (p.Count() < 2)
            throw new NotImplementedException();

        foreach (var part in p[0].QuerySelectorAll("span")) {
            temp += part.TextContent;
        }

        desc = p[1].TextContent;

        var idData = headerElement.QuerySelector("span div");
        if (idData is not IElement)
            throw new NotImplementedException();

        iconId = GetIconIDFromElementOuterHtml(idData.OuterHtml);

        Header header = new(temp, desc, iconId);
        return header;
    }

    private int GetIconIDFromElementOuterHtml(string outer) {
        int tempId = outer.LastIndexOf(':');

        var t0 = int.TryParse($"{outer[tempId + 1]}", out int id0);
        var t1 = int.TryParse($"{outer[tempId + 2]}", out int id1);

        if (t0 && t1) {
            return Convert.ToInt32($"{id0}{id1}");
        } else {
            return Convert.ToInt32($"{id0}");
        }
    }

    private async Task<IDocument> GetPage(string state) {
        IConfiguration config = Configuration.Default.WithDefaultLoader();
        IBrowsingContext context = BrowsingContext.New(config);
        IDocument document = await context.OpenAsync(url + state);

        return document;
    }
}
