using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using WeatherSkill.Dialogs.Shared.Resources;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Solutions.Extensions;
using Microsoft.Bot.Solutions.Resources;
using Microsoft.Bot.Solutions.Skills;

namespace WeatherSkill
{
    public class WeatherForecastDialog : WeatherSkillDialog
    {
        public WeatherForecastDialog(
            ISkillConfiguration services,
            IStatePropertyAccessor<WeatherSkillState> weatherStateAccessor,
            IStatePropertyAccessor<DialogState> dialogStateAccessor,
            IServiceManager serviceManager)
            : base(nameof(WeatherForecastDialog), services, weatherStateAccessor, dialogStateAccessor, serviceManager)
        {
            var sendEmail = new WaterfallStep[]
           {
                GetAuthToken,
                AfterGetAuthToken,
                CollectSubject
           };
        }

        public async Task<DialogTurnResult> CollectSubject(WaterfallStepContext sc, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
            }
            catch (Exception ex)
            {
                throw await HandleDialogExceptions(sc, ex);
            }

            return null;
        }
    }
}
