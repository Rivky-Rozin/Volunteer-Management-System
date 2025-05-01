using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
namespace Helpers
{
    internal static class Tools
    {
        private const string LocationIqApiKey = "YOUR_API_KEY_HERE"; // שים כאן את ה-API KEY שלך מ-LocationIQ

        internal static (double Latitude, double Longitude) GetCoordinatesFromAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("כתובת לא יכולה להיות ריקה.");

            string url = $"https://us1.locationiq.com/v1/search?key={LocationIqApiKey}&q={Uri.EscapeDataString(address)}&format=json";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("YourAppName/1.0");

            try
            {
                var response = client.GetAsync(url).Result;

                if (!response.IsSuccessStatusCode)
                    throw new Exception("שגיאה בשליפת נתוני מיקום מהשרת.");

                string content = response.Content.ReadAsStringAsync().Result;

                var results = JsonSerializer.Deserialize<LocationIqResult[]>(content);

                if (results == null || results.Length == 0)
                    throw new Exception("הכתובת לא נמצאה.");

                var first = results[0];

                return (double.Parse(first.lat), double.Parse(first.lon));
            }
            catch (Exception ex)
            {
                throw new Exception("הכתובת שסיפקת אינה תקינה או שלא ניתן לאתר אותה.", ex);
            }
        }

        private class LocationIqResult
        {
            public string lat { get; set; }
            public string lon { get; set; }
            public string display_name { get; set; }
        }
        public static string ToStringProperty<T>(this T t)
        {
            if (t == null)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            Type type = t.GetType();
            PropertyInfo[] properties = type.GetProperties();

            foreach (var prop in properties)
            {
                object value = prop.GetValue(t);

                if (value == null)
                {
                    sb.AppendLine($"{prop.Name}: null");
                }
                else if (value is IEnumerable enumerable && !(value is string))
                {
                    sb.AppendLine($"{prop.Name}: [");

                    foreach (var item in enumerable)
                    {
                        if (item == null)
                        {
                            sb.AppendLine("  null");
                        }
                        else
                        {
                            // אם הפריט הוא טיפוס פשוט - מציגים אותו ישירות
                            Type itemType = item.GetType();
                            if (IsSimpleType(itemType))
                            {
                                sb.AppendLine($"  {item}");
                            }
                            else
                            {
                                // אם הפריט הוא אובייקט מורכב - קוראים לו רקורסיבית
                                sb.AppendLine($"  {item.ToStringProperty()}");
                            }
                        }
                    }

                    sb.AppendLine("]");
                }
                else
                {
                    sb.AppendLine($"{prop.Name}: {value}");
                }
            }

            return sb.ToString();
        }

        private static bool IsSimpleType(Type type)
        {
            return
                type.IsPrimitive ||
                type.IsEnum ||
                type.Equals(typeof(string)) ||
                type.Equals(typeof(decimal)) ||
                type.Equals(typeof(DateTime)) ||
                type.Equals(typeof(DateTimeOffset)) ||
                type.Equals(typeof(TimeSpan)) ||
                type.Equals(typeof(Guid));
        }

        // פונקציה לחישוב המרחק בין מתנדב לקריאה בעזרת נוסחת Haversine
        internal static double GetDistance(DO.Volunteer volunteer, DO.Call call)
        {
            // רדיוס כדור הארץ בקילומטרים
            const double EarthRadius = 6371;

            // בדיקת תקינות קואורדינטות המתנדב
            if (volunteer.Latitude == null || volunteer.Longitude == null)
            {
                throw new ArgumentNullException("Volunteer latitude or longitude cannot be null.");
            }

            double lat1 = DegreesToRadians(volunteer.Latitude.Value);
            double lon1 = DegreesToRadians(volunteer.Longitude.Value);
            double lat2 = DegreesToRadians(call.Latitude);
            double lon2 = DegreesToRadians(call.Longitude);

            // חישוב ההפרש בקווי רוחב ובאורך
            double dLat = lat2 - lat1;
            double dLon = lon2 - lon1;

            // חישוב המרחק בעזרת נוסחת Haversine
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            // חישוב המרחק (רדיוס כדור הארץ * זווית הקשת)
            double distance = EarthRadius * c;

            return distance;
        }
        // פונקציה להמרת מעלות לרדיאנים.
        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }

}

