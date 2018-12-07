using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherSkill.ServiceClients
{
    public interface IWeatherForecast
    {
        Task<string> GenerateForcastMessage(string location);
    }
}
