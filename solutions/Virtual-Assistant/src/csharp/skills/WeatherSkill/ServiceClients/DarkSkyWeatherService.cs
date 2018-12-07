using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DarkSky;
using DarkSky.Services;

namespace WeatherSkill.ServiceClients
{
    public class DarkSkyWeatherService : IWeatherForecast
    {
        private DarkSkyService _darkSky;

        public DarkSkyWeatherService(string key)
        {
            _darkSky = new DarkSkyService(key);
        }

        async public Task<string> GenerateForcastMessage(string location)
        {
            try
            {
                var geo = await ServiceManager.GeoService.QueryGeoInfoByLocation(location);
                if (geo != null)
                {
                    var response = await _darkSky.GetForecast(geo.Latitude, geo.Longitude);
                    if (response.IsSuccessStatus)
                    {
                        string message = $"In {geo.Location}, {response.Response.Daily.Summary}";
                        return message;
                    }
                    else
                    {
                        return response.ResponseReasonPhrase;
                    }
                }
                else throw new Exception($"Failed to find a location as {location}.");

            }
            catch (Exception err)
            {
                return err.Message;
            }
        }
    }
}
