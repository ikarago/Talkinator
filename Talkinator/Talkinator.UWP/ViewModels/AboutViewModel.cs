using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Talkinator.UWP.Helpers;
using Windows.ApplicationModel;
using Windows.UI.Xaml;

namespace Talkinator.UWP.ViewModels
{
    public class AboutViewModel : Observable
    {
        // Properties
        public Visibility FeedbackLinkVisibility => Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported() ? Visibility.Visible : Visibility.Collapsed;

        private string _appName;
        public string AppName
        {
            get { return _appName; }
            set { Set(ref _appName, value); }
        }

        private string _versionNumber;
        public string VersionNumber
        {
            get { return _versionNumber; }
            set { Set(ref _versionNumber, value); }
        }


        // Constructor
        public AboutViewModel()
        {
            Initialize();
        }

        // Initialize Stuff
        public void Initialize()
        {
            AppName = GetAppName();
            VersionNumber = GetVersionNumber();
        }


        // Commands
        private ICommand _launchFeedbackHubCommand;
        public ICommand LaunchFeedbackHubCommand
        {
            get
            {
                if (_launchFeedbackHubCommand == null)
                {
                    _launchFeedbackHubCommand = new RelayCommand(
                        async () =>
                        {
                            // This launcher is part of the Store Services SDK https://docs.microsoft.com/en-us/windows/uwp/monetize/microsoft-store-services-sdk
                            var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
                            await launcher.LaunchAsync();
                        });
                }

                return _launchFeedbackHubCommand;
            }
        }


        // Methods
        private string GetAppName()
        {
            var package = Package.Current;
            string appName = package.DisplayName;

            return appName;
        }

        private string GetVersionNumber()
        {
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
