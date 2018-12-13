using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DarkSky;
using DarkSky.Services;
using static DarkSky.Services.DarkSkyService;

namespace WeatherSkill.ServiceClients
{
    public class DarkSkyWeatherService : IWeatherForecast
    {
        private DarkSkyService _darkSky;

        public DarkSkyWeatherService(string key)
        {
            _darkSky = new DarkSkyService(key);
        }

        async public Task<string> GenerateForcastMessageDaily(string location, DateTime? startdate = null)
        {
            try
            {
                var geo = await ServiceManager.GeoService.QueryGeoInfoByLocation(location);
                if (geo != null)
                {
                    OptionalParameters param = null;
                    if (startdate != null && startdate.HasValue)
                    {
                        param = new OptionalParameters { ForecastDateTime = startdate, ExtendHourly = false };
                    }

                    var response = await _darkSky.GetForecast(geo.Latitude, geo.Longitude, param);
                    if (response.IsSuccessStatus)
                    {
                        string message = $"In {geo.Location}, {response.Response.Daily.Summary}";
                        if (startdate!=null && startdate.HasValue)
                        {
                            message = $"At {geo.Location} on {startdate.Value.ToString("d")}, {response.Response.Hourly.Summary}.";
                        }

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

        async public Task<string> GenerateForcastMessageHourly(string location, DateTime? starttime = null)
        {
            try
            {
                var geo = await ServiceManager.GeoService.QueryGeoInfoByLocation(location);
                if (geo != null)
                {
                    OptionalParameters param = null;
                    if (starttime != null && starttime.HasValue)
                    {
                        param = new OptionalParameters { ForecastDateTime = starttime, ExtendHourly = true };
                    }

                    var response = await _darkSky.GetForecast(geo.Latitude, geo.Longitude, param);
                    if (response.IsSuccessStatus)
                    {
                        string message = $"In {geo.Location}, {response.Response.Currently.Summary}";
                        if (starttime != null && starttime.HasValue)
                        {
                            message = $"At {geo.Location} from {starttime.Value.ToString("g")}, {response.Response.Hourly.Summary}";
                        }

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
