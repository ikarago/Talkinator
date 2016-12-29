using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationsExtensions.Toasts;
using Windows.UI.Notifications;
using NotificationsExtensions;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Talkinator.Helpers
{
    public static class MessageHelper
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
                                Text = ResourceHelper.GetString(title),
                                HintStyle = AdaptiveTextStyle.Title
                            },

                            new AdaptiveText()
                            {
                                Text = (ResourceHelper.GetString(message) + " " + exportPath)
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
                                Text = ResourceHelper.GetString(title),
                                HintStyle = AdaptiveTextStyle.Title
                            },

                            new AdaptiveText()
                            {
                                Text = (ResourceHelper.GetString(message))
                            }
                        }
                };
            }

            var toastNotification = new ToastNotification(toast.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
        }


        /// <summary>
        /// Show Error Dialog, with the standard "Close"-option.
        /// </summary>
        /// <param name="message">Insert messages like this --> "nameOfTheResourceString"</param>
        /// <param name="title">Insert a title for the message like this --> "nameOfTheResourceString"</param>
        /// <param name="button">Insert messages like this --> "nameOfTheResourceString"
        ///  -- When empty, it will show a default 'Close'-button.</param>
        public static async void ShowErrorDialog(string message, string title = null, string button = "Message-Close")
        {
            // Ask to show the changelog
            ContentDialog errorDialog = new ContentDialog()
            {
                Title = ResourceHelper.GetString(title),
                Content = ResourceHelper.GetString(message),
                PrimaryButtonText = ResourceHelper.GetString(button)
            };

            errorDialog.IsSecondaryButtonEnabled = false;
            ContentDialogResult result = await errorDialog.ShowAsync();
        }

        // Export Messages
        public static void ShowExportSuccessfulMessage(string exportPath)
        {
            ShowToast("Message-ExportSuccessful-SavedTo", "Message-ExportSuccessful-Title", exportPath);
        }
        public static void ShowExportFailedMessage()
        {
            ShowErrorDialog("Message-ExportFailed-Message", "Message-ExportFailed-Title");
        }

        // IAP Message
        public static void ShowIapAcquisitionSuccessfulMessage()
        {
            ShowToast("Message-AcquisitionSuccessful-Message", "Message-AcquisitionSuccessful-Title");
        }
        public static void ShowIapAcquisitionFailedMessage()
        {
            ShowErrorDialog("Message-AcquisitionFailed-Message", "Message-AcquisitionFailed-Title");
        }
        public static void ShowIapAlreadyBoughtMessage()
        {
            ShowErrorDialog("Message-AcquisitionDuplicate-Message", "Message-AcquisitionDuplicate-Title");
        }

    }
}
