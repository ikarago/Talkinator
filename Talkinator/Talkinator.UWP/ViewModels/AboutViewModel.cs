using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talkinator.UWP.Helpers;
using Windows.ApplicationModel;

namespace Talkinator.UWP.ViewModels
{
    public class AboutViewModel : Observable
    {
        // Properties
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
            VersionNumber = GetVersionNumber();
        }


        // Methods
        private string GetVersionNumber()
        {
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
