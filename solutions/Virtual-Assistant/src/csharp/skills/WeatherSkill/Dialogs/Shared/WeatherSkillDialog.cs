using WeatherSkill.Dialogs.Shared.Resources;
using Luis;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Solutions;
using Microsoft.Bot.Solutions.Extensions;
using Microsoft.Bot.Solutions.Skills;
using Microsoft.Recognizers.Text;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherSkill
{
    public class WeatherSkillDialog : ComponentDialog
    {
        // Constants
        public const string SkillModeAuth = "SkillAuth";
        public const string LocalModeAuth = "LocalAuth";

        // Fields
        protected WeatherSkillResponseBuilder _responseBuilder = new WeatherSkillResponseBuilder();

        public WeatherSkillDialog(
            string dialogId,
            ISkillConfiguration services,
            IStatePropertyAccessor<WeatherSkillState> weatherStateAccessor,
            IStatePropertyAccessor<DialogState> dialogStateAccessor,
            IServiceManager serviceManager)
            : base(dialogId)
        {
            Services = services;
            WeatherStateAccessor = weatherStateAccessor;
            DialogStateAccessor = dialogStateAccessor;
            WeatherServiceManager = serviceManager;
        }

        protected WeatherSkillDialog(string dialogId)
            : base(dialogId)
        {
        }

        protected ISkillConfiguration Services { get; set; }

        protected IStatePropertyAccessor<WeatherSkillState> WeatherStateAccessor { get; set; }

        protected IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }

        protected IServiceManager WeatherServiceManager { get; set; }

        protected WeatherSkillResponseBuilder ResponseBuilder { get; set; } = new WeatherSkillResponseBuilder();

        protected override async Task<DialogTurnResult> OnBeginDialogAsync(DialogContext dc, object options, CancellationToken cancellationToken = default(CancellationToken))
        {
            var state = await WeatherStateAccessor.GetAsync(dc.Context);
            await DigestLuisResult(dc, state.LuisResult);
            return await base.OnBeginDialogAsync(dc, options, cancellationToken);
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var state = await WeatherStateAccessor.GetAsync(dc.Context);
            await DigestLuisResult(dc, state.LuisResult);
            return await base.OnContinueDialogAsync(dc, cancellationToken);
        }

        // Shared steps
        public async Task<DialogTurnResult> GetAuthToken(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var skillOptions = (WeatherSkillDialogOptions)sc.Options;

                // If in Skill mode we ask the calling Bot for the token
                if (skillOptions != null && skillOptions.SkillMode)
                {
                    // We trigger a Token Request from the Parent Bot by sending a "TokenRequest" event back and then waiting for a "TokenResponse"
                    // TODO Error handling - if we get a new activity that isn't an event
                    var response = sc.Context.Activity.CreateReply();
                    response.Type = ActivityTypes.Event;
                    response.Name = "tokens/request";

                    // Send the tokens/request Event
                    await sc.Context.SendActivityAsync(response);

                    // Wait for the tokens/response event
                    return await sc.PromptAsync(SkillModeAuth, new PromptOptions());
                }
                else
                {
                    return await sc.PromptAsync(LocalModeAuth, new PromptOptions());
                }
            }
            catch(Exception ex)
            {
                throw await HandleDialogExceptions(sc, ex);
            }
        }

        public async Task<DialogTurnResult> AfterGetAuthToken(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                // When the user authenticates interactively we pass on the tokens/Response event which surfaces as a JObject
                // When the token is cached we get a TokenResponse object.
                var skillOptions = (WeatherSkillDialogOptions)sc.Options;
                TokenResponse tokenResponse;
                if (skillOptions != null && skillOptions.SkillMode)
                {
                    var resultType = sc.Context.Activity.Value.GetType();
                    if (resultType == typeof(TokenResponse))
                    {
                        tokenResponse = sc.Context.Activity.Value as TokenResponse;
                    }
                    else
                    {
                        var tokenResponseObject = sc.Context.Activity.Value as JObject;
                        tokenResponse = tokenResponseObject?.ToObject<TokenResponse>();
                    }
                }
                else
                {
                    tokenResponse = sc.Result as TokenResponse;
                }

                if (tokenResponse != null)
                {
                    var state = await WeatherStateAccessor.GetAsync(sc.Context);
                }

                return await sc.NextAsync();
            }
            catch(Exception ex)
            {
                throw await HandleDialogExceptions(sc, ex);
            }
        }    

        // Validators
        private Task<bool> TokenResponseValidator(PromptValidatorContext<Activity> pc, CancellationToken cancellationToken)
        {
            var activity = pc.Recognized.Value;
            if (activity != null && activity.Type == ActivityTypes.Event)
            {
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        private Task<bool> AuthPromptValidator(PromptValidatorContext<TokenResponse> pc, CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

     
        // Helpers
        public async Task DigestLuisResult(DialogContext dc, Weather luisResult)
        {
            try
            {
                var state = await WeatherStateAccessor.GetAsync(dc.Context);

                // extract entities and store in state here.

                if (luisResult.Entities.Weather_Location!=null && luisResult.Entities.Weather_Location.Length>0)
                {
                    state.Locations.Clear();
                    foreach (var location in luisResult.Entities.Weather_Location)
                    {
                        if (!state.Locations.Contains(location))
                        {
                            state.Locations.Add(location);
                        }
                    }
                }

                if (luisResult.Entities.datetime!= null && luisResult.Entities.datetime.Length > 0)
                {
                    state.ForecastTimes.Clear();
                    foreach (var datespcs in luisResult.Entities.datetime)
                    {
                       switch (datespcs.Type)
                        {
                            case "date":
                                if (datespcs.Expressions.Count > 0)
                                {
                                    var forecast = new ForecastTime { StartTime = null, Type = ForecastType.Day };
                                    if (DateTime.TryParse(datespcs.Expressions[0], out DateTime time))
                                    {
                                        forecast.StartTime = time;
                                    }
                                    state.ForecastTimes.Add(forecast);
                                }
                                break;

                            case "datetime":
                                if (datespcs.Expressions.Count > 0)
                                {
                                    var forecast = new ForecastTime { StartTime = null, Type = ForecastType.Hour };
                                    if (DateTime.TryParse(datespcs.Expressions[0], out DateTime time))
                                    {
                                        forecast.StartTime = time;
                                    }
                                    state.ForecastTimes.Add(forecast);
                                }
                                break;
                        }                        
                    }
                }

            }
            catch
            {
                // put log here
            }
        }

        // This method is called by any waterfall step that throws an exception to ensure consistency
        public async Task<Exception> HandleDialogExceptions(WaterfallStepContext sc, Exception ex)
        {
            await sc.Context.SendActivityAsync(sc.Context.Activity.CreateReply(WeatherSkillSharedResponses.ErrorMessage));
            await sc.CancelAllDialogsAsync();
            return ex;
        }
    }
}
