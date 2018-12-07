using Luis;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;

namespace WeatherSkill
{
    public class WeatherSkillState
    {
        public List<string> Locations;
        public List<DateTime> ForcastDates;

        public WeatherSkillState()
        {
            Locations = new List<string>();
            ForcastDates = new List<DateTime>();
        }

        public void Clear()
        {
            Locations.Clear();
            ForcastDates.Clear();
        }

        public Luis.Weather LuisResult { get; set; }

        public Luis.General GeneralLuisResult { get; set; }
    }
}
