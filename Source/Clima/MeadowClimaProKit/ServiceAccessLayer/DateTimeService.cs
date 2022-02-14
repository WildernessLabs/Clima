using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MeadowClimaProKit.ServiceAccessLayer
{
    public static class DateTimeService
    {
        static string City = "Vancouver";
        static string clockDataUri = $"http://worldtimeapi.org/api/timezone/America/{City}/";

        static DateTimeService() { }

        public static async Task GetTimeAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.Timeout = new TimeSpan(0, 5, 0);

                    HttpResponseMessage response = await client.GetAsync($"{clockDataUri}");

                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    response.EnsureSuccessStatusCode();
                    string json = await response.Content.ReadAsStringAsync();
                    var values = JsonSerializer.Deserialize<DateTimeEntity>(json);

                    stopwatch.Stop();

                    var dateTimeOffset = values.datetime.Add(stopwatch.Elapsed);
                    var dateTime = new DateTime(
                        dateTimeOffset.Year,
                        dateTimeOffset.Month,
                        dateTimeOffset.Day,
                        dateTimeOffset.Hour,
                        dateTimeOffset.Minute,
                        dateTimeOffset.Second
                    );

                    MeadowApp.Device.SetClock(dateTime);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Request timed out.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Request went sideways: {e.Message}");
                }
            }
        }
    }

    public class DateTimeEntity
    {
        public string abbreviation { get; set; }
        public string client_ip { get; set; }
        public DateTimeOffset datetime { get; set; }
        public long day_of_week { get; set; }
        public long day_of_year { get; set; }
        public bool dst { get; set; }
        public object dst_from { get; set; }
        public long dst_offset { get; set; }
        public object dst_until { get; set; }
        public long raw_offset { get; set; }
        public string timezone { get; set; }
        public long unixtime { get; set; }
        public DateTimeOffset utc_datetime { get; set; }
        public string utc_offset { get; set; }
        public long week_number { get; set; }
    }
}
