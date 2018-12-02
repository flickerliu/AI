// https://docs.microsoft.com/en-us/visualstudio/modeling/t4-include-directive?view=vs-2017
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Bot.Solutions.Dialogs;

namespace PointOfInterestSkill.Dialogs.Main.Resources
{
    /// <summary>
    /// Contains bot responses.
    /// </summary>
    public static class POIMainResponses
    {
        private static readonly ResponseManager _responseManager;

        static POIMainResponses()
        {
            var dir = Path.GetDirectoryName(typeof(POIMainResponses).Assembly.Location);
            var resDir = Path.Combine(dir, @"Dialogs\Main\Resources");
            _responseManager = new ResponseManager(resDir, "POIMainResponses");
        }

        // Generated accessors
        public static BotResponse PointOfInterestWelcomeMessage => GetBotResponse();

        public static BotResponse HelpMessage => GetBotResponse();

        public static BotResponse GreetingMessage => GetBotResponse();

        public static BotResponse GoodbyeMessage => GetBotResponse();

        public static BotResponse LogOut => GetBotResponse();

        public static BotResponse FeatureNotAvailable => GetBotResponse();

        public static BotResponse ShowRecognizedUserIntent(object intent) => GetBotResponseWithParam(intent);

        private static BotResponse GetBotResponse([CallerMemberName] string propertyName = null)
        {
            return _responseManager.GetBotResponse(propertyName);
        }

        private static BotResponse GetBotResponseWithParam(object param, [CallerMemberName] string propertyName = null)
        {
            var def = _responseManager.GetBotResponse(propertyName);
            var response = new BotResponse(
                                           string.Format(def.Reply.Text, param),
                                           string.Format(def.Reply.Speak, param),
                                           def.InputHint);

            return response;
        }
    }
}