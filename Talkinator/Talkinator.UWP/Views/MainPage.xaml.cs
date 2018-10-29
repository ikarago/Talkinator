using System;
using System.Diagnostics;
using Talkinator.UWP.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Talkinator.UWP.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();


        // Constructor
        public MainPage()
        {
            InitializeComponent();
            AdaptViewToDevice();
        }


        // Methods
        private void AdaptViewToDevice()
        {
            // Check devicefamily
            var device = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;
            bool isDesktop = (device.ContainsKey("DeviceFamily") && device["DeviceFamily"] == "Desktop");
            bool isXbox = (device.ContainsKey("DeviceFamily") && device["DeviceFamily"] == "Xbox");

            if (isDesktop == true)
            {
                // Hide classic Talkinator-button
                btnTitle.Visibility = Visibility.Collapsed;

                // Show custom titlebar
                gridTitlebar.Visibility = Visibility.Visible;
            }
            else
            {
                // Hide custom titlebar
                gridTitlebar.Visibility = Visibility.Collapsed;

                // Show classic Talkinator-button
                btnTitle.Visibility = Visibility.Visible;
            }
        }


        private async void txtTextToSay_Drop(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                try
                {
                    string copiedData = await e.DataView.GetTextAsync();
                    ViewModel.Text += copiedData;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        private void txtTextToSay_DragOver(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
        }
    }
}
