﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.TemplateManager;
using Microsoft.Bot.Schema;
using VirtualAssistant.Dialogs.Cancel.Resources;

namespace VirtualAssistant
{
    public class CancelResponses : TemplateManager
    {
        // Constants
        public const string _confirmPrompt = "Cancel.ConfirmCancelPrompt";
        public const string _cancelConfirmed = "Cancel.CancelConfirmed";
        public const string _cancelDenied = "Cancel.CancelDenied";

        // Fields
        private static LanguageTemplateDictionary _responseTemplates = new LanguageTemplateDictionary
        {
            ["default"] = new TemplateIdMap
            {
                { _confirmPrompt, (context, data) => CancelStrings.CANCEL_PROMPT },
                { _cancelConfirmed, (context, data) => SendAcceptingInputReply(context, CancelStrings.CANCEL_CONFIRMED) },
                { _cancelDenied, (context, data) => SendAcceptingInputReply(context, CancelStrings.CANCEL_DENIED) },
            },
            ["en"] = new TemplateIdMap { },
            ["fr"] = new TemplateIdMap { },
        };

        public CancelResponses()
        {
            Register(new DictionaryRenderer(_responseTemplates));
        }

        public static IMessageActivity SendAcceptingInputReply(ITurnContext turnContext, string text)
        {
            var reply = turnContext.Activity.CreateReply();
            reply.InputHint = InputHints.AcceptingInput;
            reply.Text = text;

            return reply;
        }
    }
}
