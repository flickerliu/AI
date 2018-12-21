﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WeatherSkill.Dialogs.WeatherForecastSkill.Resources;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Solutions.Extensions;
using Microsoft.Bot.Solutions.Resources;
using Microsoft.Bot.Solutions.Skills;
using Microsoft.Bot.Solutions.Dialogs;

namespace WeatherSkill
{
    public class WeatherWearDialog : WeatherSkillDialog
    {
        public WeatherWearDialog(
            ISkillConfiguration services,
            IStatePropertyAccessor<WeatherSkillState> weatherStateAccessor,
            IStatePropertyAccessor<DialogState> dialogStateAccessor,
            IServiceManager serviceManager)
            : base(nameof(WeatherWearDialog), services, weatherStateAccessor, dialogStateAccessor, serviceManager)
        {
            var forecast = new WaterfallStep[]
           {
                CollectLocation,
                CollectDate,
                ShowForecastInfo
           };

            // Define the conversation flow using a waterfall model.
            AddDialog(new WaterfallDialog(Actions.Forecast, forecast));
            InitialDialogId = Actions.Forecast;
        }

        public async Task<DialogTurnResult> CollectLocation(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var state = await WeatherStateAccessor.GetAsync(sc.Context);

                if (state.Locations.Count == 0)
                {
                    //set the default location as shanghai
                    state.Locations = new List<string>() { "shanghai" };
                }

                return await sc.NextAsync();
            }
            catch (Exception ex)
            {
                throw await HandleDialogExceptions(sc, ex);
            }
        }

        public async Task<DialogTurnResult> CollectDate(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var state = await WeatherStateAccessor.GetAsync(sc.Context);

                if (state.ForecastTimes.Count == 0)
                {
                    //set the default date as today
                    state.ForecastTimes = new List<ForecastTime> { new ForecastTime { StartTime = DateTime.Now, Type = ForecastType.Day } };
                }

                return await sc.NextAsync();
            }
            catch (Exception ex)
            {
                throw await HandleDialogExceptions(sc, ex);
            }
        }

        public async Task<DialogTurnResult> ShowForecastInfo(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var state = await WeatherStateAccessor.GetAsync(sc.Context);

                foreach (var location in state.Locations)
                {
                    if (state.ForecastTimes.Count > 0)
                    {
                        var text = await ServiceManager.WearSuggestionService.GenerateWearSuggestion(location, state.ForecastTimes[0].StartTime, state.Clothes.ToArray());

                        var replyMessage = sc.Context.Activity.CreateReply(text);
                        await sc.Context.SendActivityAsync(replyMessage);
                    }
                }

                return await sc.EndDialogAsync(true);
            }
            catch (Exception ex)
            {
                throw await HandleDialogExceptions(sc, ex);
            }
        }
    }
}