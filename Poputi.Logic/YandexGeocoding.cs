using NetTopologySuite.Geometries;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System;

namespace Poputi.Logic
{
    public class YandexGeocoding : IGeocodingService
    {
        private string key = "10424d5c-4d88-4067-86fc-52b0b9cc2d70";
        public async Task<(string error, Point)> GetGeocode(string address)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var uri = "https://geocode-maps.yandex.ru/1.x/?format=json&apikey=" + key + "&geocode=" + address;
                    var request = new HttpRequestMessage(HttpMethod.Get, uri);
                    var response = await client.SendAsync(request);

                    if (!response.IsSuccessStatusCode)
                        return (response.StatusCode.ToString(), null);

                    var body = await response.Content.ReadAsStringAsync();

                    JObject jObject = JObject.Parse(body);
                    JToken jToken = jObject["response"]["GeoObjectCollection"]["featureMember"][0]["GeoObject"]["Point"]["pos"];
                    var coords = jToken.ToString().Split();
                    var first = double.Parse(coords[0], CultureInfo.InvariantCulture);
                    var second = double.Parse(coords[1], CultureInfo.InvariantCulture);
                    return (null, new Point(first, second));
                }
                catch (Exception e)
                {
                    return (e.Message, null);
                }
            }
        }
    }
}
