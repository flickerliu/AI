using Luis;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Graph;
using System;
using System.Collections.Generic;

namespace WeatherSkill
{
    public class WeatherSkillState
    {
        public WeatherSkillState()
        {

        }

        public Luis.Weather LuisResult { get; set; }

        public Luis.General GeneralLuisResult { get; set; }
    }
}
