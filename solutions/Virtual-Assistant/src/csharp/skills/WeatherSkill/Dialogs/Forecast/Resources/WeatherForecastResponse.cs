﻿// https://docs.microsoft.com/en-us/visualstudio/modeling/t4-include-directive?view=vs-2017
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Bot.Solutions.Dialogs;

namespace WeatherSkill.Dialogs.WeatherForecastSkill.Resources
{
    /// <summary>
    /// Contains bot responses.
    /// </summary>
    public static class WeatherForecastResponse
    {
        private static readonly ResponseManager _responseManager;

        static WeatherForecastResponse()
        {
            var dir = Path.GetDirectoryName(typeof(WeatherForecastResponse).Assembly.Location);
            var resDir = Path.Combine(dir, @"Dialogs\Forecast\Resources");
            _responseManager = new ResponseManager(resDir, "WeatherForecastResponse");
        }

        // Generated accessors

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