// <auto-generated>
// Code generated by LUISGen SAICVADemo_Weather.json -cs Luis.Weather -o 
// Tool github: https://github.com/microsoft/botbuilder-tools
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Solutions.Util;

namespace Luis
{
    public class Weather : IRecognizerConvert
    {
        public string Text;
        public string AlteredText;
        public enum Intent
        {
            None,
            Weather_ContextContinue,
            Weather_GetForecast,
            Weather_Wear
        };
        public Dictionary<Intent, IntentScore> Intents;

        public class _Entities
        {
            // Simple entities
            public string[] Wear_Clothes;
            public string[] Weather_Location;

            // Built-in entities
            public DateTimeSpec[] datetime;

            // Instance
            public class _Instance
            {
                public InstanceData[] Wear_Clothes;
                public InstanceData[] Weather_Location;
                public InstanceData[] datetime;
            }
            [JsonProperty("$instance")]
            public _Instance _instance;
        }
        public _Entities Entities;

        [JsonExtensionData(ReadData = true, WriteData = true)]
        public IDictionary<string, object> Properties { get; set; }

        public void Convert(dynamic result)
        {
            var app = JsonConvert.DeserializeObject<Weather>(JsonConvert.SerializeObject(result));
            Text = app.Text;
            AlteredText = app.AlteredText;
            Intents = app.Intents;
            Entities = app.Entities;
            Properties = app.Properties;
        }

        public (Intent intent, double score) TopIntent()
        {
            Intent maxIntent = Intent.None;
            var max = Util.ScoreThreshold;
            foreach (var entry in Intents)
            {
                if (entry.Value.Score > max)
                {
                    maxIntent = entry.Key;
                    max = entry.Value.Score.Value;
                }
            }
            return (maxIntent, max);
        }
    }
}
