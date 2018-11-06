using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

using Microsoft.Toolkit.Uwp.Notifications;
using Talkinator.UWP.Helpers;
using Windows.UI.Notifications;

namespace Talkinator.UWP.Services
{
    public static class MessageService
    {
        // Main Toastbuilder
        /// <summary>
        /// Show a Toast Notification
        /// </summary>
        /// <param name="message">Insert messages like this --> "nameOfTheResourceString"</param>
        /// <param name="title">Insert a title like this --> "nameOfTheResourceString"</param>
        /// <param name="exportPath">Insert the export path to be displayed when this is for notifying the user of an successful export</param>
        private static void ShowToast(string message, string title, string exportPath = null)
        {
            var toast = new ToastContent();
            toast.Visual = new ToastVisual();

            if (exportPath != null)
            {
                toast.Visual.BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                        {
                            new AdaptiveText()
                            {
                                Text = ResourceExtensions.GetLocalized(title),
                                HintStyle = AdaptiveTextStyle.Title
                            },

                            new AdaptiveText()
                            {
                                Text = (ResourceExtensions.GetLocalized(message) + " " + exportPath)
                            }
                        }
                };
            }
            else
            {
                toast.Visual.BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                        {
                            new AdaptiveText()
                            {
                                Text = ResourceExtensions.GetLocalized(title),
                                HintStyle = AdaptiveTextStyle.Title
                            },

                            new AdaptiveText()
                            {
                                Text = (ResourceExtensions.GetLocalized(message))
                            }
                        }
                };
            }

            var toastNotification = new ToastNotification(toast.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
        }

        // Export Messages
        public static void ShowExportSuccessfulMessage()
        {
            ShowToast("Message-ExportSuccessful-SavedTo", "Message-ExportSuccessful-Title");
        }
        public static void ShowExportFailedMessage()
        {
            ShowToast("Message-ExportFailed-Message", "Message-ExportFailed-Title");
        }
    }
}
