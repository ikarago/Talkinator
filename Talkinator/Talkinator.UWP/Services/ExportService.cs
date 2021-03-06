﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Talkinator.UWP.Helpers;
using Windows.Media.MediaProperties;
using Windows.Media.SpeechSynthesis;
using Windows.Media.Transcoding;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Talkinator.UWP.Services
{
    public static class ExportService
    {
        /// <summary>
        /// Exports the entered text to a soundfile
        /// </summary>
        /// <param name="input">The text that's going to be converted into a soundfile</param>
        /// <param name="voice">The VoiceInformation containing the voice that's going to be used. If null system will use the default system voice</param>
        /// <returns></returns>
        public static async Task<bool> ExportTextToSpeechFile(string input, VoiceInformation voice = null)
        {
            bool success = false;

            // Add the spoken text
            var synth = new SpeechSynthesizer();
            if (voice != null)
            {
                synth.Voice = voice;
            }

            SpeechSynthesisStream synthStream = await synth.SynthesizeTextToStreamAsync(input);

            // Check devicefamily
            var device = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;
            bool isDesktop = (device.ContainsKey("DeviceFamily") && device["DeviceFamily"] == "Desktop");
            bool isXbox = (device.ContainsKey("DeviceFamily") && device["DeviceFamily"] == "Xbox");

            // Now get the export-file picker
            var exportPicker = new FileSavePicker();
            exportPicker.SuggestedStartLocation = PickerLocationId.Desktop;

            // Check whether the using is using Windows 10 Mobile, if true, only add .wav-export, because the codecs in Mobile are fucking retarded
            if (isDesktop == true)
            {
                exportPicker.FileTypeChoices.Add(".MP3", new List<string>() { ".mp3" });
                exportPicker.FileTypeChoices.Add(".WMA", new List<string>() { ".wma" });
            }
            else if (isXbox == true)
            {
                exportPicker.FileTypeChoices.Add(".WMA", new List<string>() { ".wma" });
            }
            exportPicker.FileTypeChoices.Add(".WAV", new List<string>() { ".wav" });
            exportPicker.SuggestedFileName = ResourceExtensions.GetLocalized("SpokenText");

            // Fill data of the fileTarget with selection from the Save-picker
            var fileTarget = await exportPicker.PickSaveFileAsync();
            if (fileTarget != null)
            {
                if (fileTarget.FileType == ".wma" || fileTarget.FileType == ".mp3" || fileTarget.FileType == ".m4a")
                {
                    success = await SaveAndEncodeFile(fileTarget, synthStream, synth.Voice);
                }
                else if (fileTarget.FileType == ".wav")
                {
                    try
                    {
                        using (var reader = new DataReader(synthStream))
                        {
                            await reader.LoadAsync((uint)synthStream.Size);
                            IBuffer buffer = reader.ReadBuffer((uint)synthStream.Size);
                            await FileIO.WriteBufferAsync(fileTarget, buffer);
                        }
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Couldn't export to wav");
                        Debug.WriteLine(ex);
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileTarget">File to write to</param>
        /// <param name="synthStream">The SpeechSynthesisStream with the actual sound</param>
        /// <param name="voice">The VoiceInformation for setting the correct voice in the artist</param>
        /// <returns></returns>
        private static async Task<bool> SaveAndEncodeFile(StorageFile fileTarget, SpeechSynthesisStream synthStream, VoiceInformation voice)
        {
            bool success = false;

            // Initialise some stuff
            MediaEncodingProfile _profile;
            MediaTranscoder _transcoder = new MediaTranscoder();
            CoreDispatcher _dispatcher = Window.Current.Dispatcher;
            CancellationTokenSource _cts = new CancellationTokenSource();


            Debug.WriteLine(fileTarget.FileType + " selected");

            // Set encoding profiles
            _profile = null;
            AudioEncodingQuality audioEncodingProfile = AudioEncodingQuality.High;
            if (fileTarget.FileType == ".wma")
            {
                _profile = MediaEncodingProfile.CreateWma(audioEncodingProfile);
            }
            else if (fileTarget.FileType == ".mp3")
            {
                _profile = MediaEncodingProfile.CreateMp3(audioEncodingProfile);
            }
            else if (fileTarget.FileType == ".m4a")
            {
                _profile = MediaEncodingProfile.CreateM4a(audioEncodingProfile);
            }
            else
            {
                Debug.WriteLine("Can't select a media encoding profile");
                return success;
            }

            // Write temporary Wav to Temp-storage
            ApplicationData appData = ApplicationData.Current;
            StorageFile source = await appData.TemporaryFolder.CreateFileAsync("temporary.wav", CreationCollisionOption.ReplaceExisting);
            try
            {
                using (var reader = new DataReader(synthStream))
                {
                    await reader.LoadAsync((uint)synthStream.Size);
                    IBuffer buffer = reader.ReadBuffer((uint)synthStream.Size);
                    await FileIO.WriteBufferAsync(source, buffer);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Couldn't prepare wav for transcoding");
                Debug.WriteLine(ex);
            }


            // Prepare transcoding files
            var preparedTranscoderResult = await _transcoder.PrepareFileTranscodeAsync(source, fileTarget, _profile);
            if (preparedTranscoderResult.CanTranscode)
            {
                // Set task for transcoding    
                await preparedTranscoderResult.TranscodeAsync().AsTask(_cts.Token);

                // Set Music-properties
                MusicProperties fileProperties = await fileTarget.Properties.GetMusicPropertiesAsync();
                fileProperties.Title = fileTarget.DisplayName;
                fileProperties.Artist = ("Talkinator " + ResourceExtensions.GetLocalized("VoicedBy") + " " + voice.DisplayName);
                await fileProperties.SavePropertiesAsync();

                // #TODO: Add the newly created file to the systems MRU?
                // Add the file to app MRU and possibly system MRU
                //RecentStorageItemVisibility visibility = SystemMRUCheckBox.IsChecked.Value ? RecentStorageItemVisibility.AppAndSystem : RecentStorageItemVisibility.AppOnly;
                //rootPage.mruToken = StorageApplicationPermissions.MostRecentlyUsedList.Add(file, file.Name, visibility);

                //RecentStorageItemVisibility visibility = RecentStorageItemVisibility.AppOnly;
                //StorageApplicationPermissions.FutureAccessList.Add(fileTarget, fileTarget.DisplayName);


                // Report completed
                success = true;
                Debug.WriteLine(fileTarget.FileType + " export completed");
            }
            else
            {
                Debug.WriteLine(preparedTranscoderResult.FailureReason);
            }

            return success;
        }
    }
}
