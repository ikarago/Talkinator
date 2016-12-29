using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace Talkinator.Helpers
{
    public static class UpdateHelper
    {
        // Constraints for the settings
        const string previousVersionMajor = "previousVersionMajor";
        const string previousVersionMinor = "previousVersionMinor";
        const string previousVersionBuild = "previousVersionBuild";
        const string previousVersionRevision = "previousVersionRevision";

        /// <summary>
        /// Gets the version number of the app-package
        /// </summary>
        /// <returns>Returns a string with the number</returns>
        public static string GetVersion()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;
            string versionNumber = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            return versionNumber;
        }


        // Methods
        public static async Task<bool> CheckIfAppIsUpdated()
        {
            bool isAppUpdated = false;
            bool toChangelog = false;

            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

            // First, check if there was a previous version. If not, just fill in the current version as the previous version.
            if (localSettings.Values[previousVersionMajor] == null)
            {
                localSettings.Values[previousVersionMajor] = version.Major;
                localSettings.Values[previousVersionMinor] = version.Minor;
                localSettings.Values[previousVersionBuild] = version.Build;
                localSettings.Values[previousVersionRevision] = version.Revision;
            }


            // Second, check if the app has been updated by comparing the old agains the new version
            if (localSettings.Values[previousVersionMajor] != null)
            {
                // Get the values for the old version
                ushort oldVerMajor = Convert.ToUInt16(localSettings.Values[previousVersionMajor]);
                ushort oldVerMinor = Convert.ToUInt16(localSettings.Values[previousVersionMinor]);
                ushort oldVerBuild = Convert.ToUInt16(localSettings.Values[previousVersionBuild]);
                ushort oldVerRevision = Convert.ToUInt16(localSettings.Values[previousVersionRevision]);

                if (version.Major > oldVerMajor)    // Check if it's a major version-update
                {
                    isAppUpdated = true;
                    localSettings.Values[previousVersionMajor] = version.Major;
                    localSettings.Values[previousVersionMinor] = version.Minor;
                    localSettings.Values[previousVersionBuild] = version.Build;
                    localSettings.Values[previousVersionRevision] = version.Revision;
                }
                else if (version.Minor > oldVerMinor)   // check if it's an minor version-update
                {
                    isAppUpdated = true;
                    localSettings.Values[previousVersionMajor] = version.Major;
                    localSettings.Values[previousVersionMinor] = version.Minor;
                    localSettings.Values[previousVersionBuild] = version.Build;
                    localSettings.Values[previousVersionRevision] = version.Revision;
                }
                else if (version.Build > oldVerBuild)   // check if the build has been updated
                {
                    isAppUpdated = true;
                    localSettings.Values[previousVersionMajor] = version.Major;
                    localSettings.Values[previousVersionMinor] = version.Minor;
                    localSettings.Values[previousVersionBuild] = version.Build;
                    localSettings.Values[previousVersionRevision] = version.Revision;
                }
                else if (version.Revision > oldVerRevision) // This probably won't be used, but is here anyway; check if the revision has been updated
                {
                    isAppUpdated = true;
                    localSettings.Values[previousVersionMajor] = version.Major;
                    localSettings.Values[previousVersionMinor] = version.Minor;
                    localSettings.Values[previousVersionBuild] = version.Build;
                    localSettings.Values[previousVersionRevision] = version.Revision;
                }
            }

            if (isAppUpdated == true)
            {
                // Ask to show the changelog
                ContentDialog openChangelogDialog = new ContentDialog()
                {
                    Title = ResourceHelper.GetString("Error-AppUpdated-Title"),
                    Content = ResourceHelper.GetString("Error-AppUpdated-Text"),
                    PrimaryButtonText = ResourceHelper.GetString("Error-Close"),
                    SecondaryButtonText = ResourceHelper.GetString("Error-GotoChangelog")
                };

                ContentDialogResult result = await openChangelogDialog.ShowAsync();

                if (result == ContentDialogResult.Secondary)
                {
                    toChangelog = true;
                }

                Debug.WriteLine("UpdateHelper - To Changelog = " + toChangelog);
            }

            return toChangelog;
        }
    }
}
