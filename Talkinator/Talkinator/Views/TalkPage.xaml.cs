﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Talkinator.Helpers;
using Talkinator.Services;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace Talkinator.Views
{
    public sealed partial class TalkPage : Page
    {
        MediaPlayer _player = new MediaPlayer();
        SystemMediaTransportControls _mediaControls;
        private bool _pauseStatus;
        private bool _loopEnabled;
        private bool updateDialogShown = false;
        private SpeechSynthesizer synth;

        public TalkPage()
        {
            this.InitializeComponent();
            synth = new SpeechSynthesizer();

            appVersion.Text = UpdateHelper.GetVersion();
            CheckUpdateStatus();
            GetVoices();
            CheckPurchaseStatus();

            _loopEnabled = false;

            _player.AutoPlay = false;
            _player.AudioCategory = MediaPlayerAudioCategory.Speech;
            _mediaControls = _player.SystemMediaTransportControls;
            _mediaControls.AutoRepeatMode = MediaPlaybackAutoRepeatMode.None;
        }

        private async void CheckUpdateStatus()
        {
            if (updateDialogShown == false)
            {
                bool toChangelog = await UpdateHelper.CheckIfAppIsUpdated();
                if (toChangelog == true)
                {
                    var uri = new Uri(@"https://ikarago.tumblr.com/talkinator-changelog");
                    var success = await Windows.System.Launcher.LaunchUriAsync(uri);
                }

                updateDialogShown = true;
            }
        }

        private async void CheckPurchaseStatus()
        {
            try
            {
                if (await LicenseHelper.CheckPurchaseStatus() == true)
                {
                    // Do something cool
                }
                else
                {

                }
            }
            catch
            {

            }

        }

        private void GetVoices()
        {
            var voices = SpeechSynthesizer.AllVoices;
            var currentVoice = synth.Voice;

            foreach (VoiceInformation voice in voices.OrderBy(v => v.Language))
            {
                ComboBoxItem cbItem = new ComboBoxItem();
                cbItem.Name = voice.DisplayName;
                cbItem.Tag = voice;
                cbItem.Content = (voice.DisplayName + " (Language: " + voice.Language + ", Gender: " + voice.Gender + ")");
                Debug.WriteLine("Id = " + voice.Id + " " + voice.DisplayName + " (Language: " + voice.Language + ", Gender: " + voice.Gender + ")");
                cboxVoices.Items.Add(cbItem);

                // Now check if the current voice is the default one...
                if (voice.Id == currentVoice.Id)
                {
                    cbItem.IsSelected = true;
                    cboxVoices.SelectedItem = cbItem;
                }
            }

        }



        private async void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (_pauseStatus == true)
            {
                _pauseStatus = false;
                _player.Play();
            }
            else
            {
                if (txtTextToSay.Text != "")
                {
                    pgrLoading.IsActive = true;
                    SpeechSynthesisStream synthStream = await synth.SynthesizeTextToStreamAsync(txtTextToSay.Text);
                    MediaSource mediaSource = MediaSource.CreateFromStream(synthStream, "audio");
                    MediaPlaybackItem playbackItem = new MediaPlaybackItem(mediaSource);

                    MediaItemDisplayProperties mediaProps = playbackItem.GetDisplayProperties();

                    mediaProps.Type = MediaPlaybackType.Music;
                    mediaProps.MusicProperties.Artist = "Talkinator"; // TODO Get name of selected Voice and paste it as Talkinator 'Bob'
                    mediaProps.MusicProperties.Title = "Spoken text";
                    //mediaProps.MusicProperties.Genres.Add("Speech");
                    playbackItem.ApplyDisplayProperties(mediaProps);

                    _player.SystemMediaTransportControls.IsEnabled = true;

                    _player.Source = playbackItem;

                    pgrLoading.IsActive = false;

                    if (_loopEnabled == true)
                    {
                        _player.IsLoopingEnabled = true;
                    }

                    _player.AutoPlay = true;
                    _player.MediaEnded += _player_MediaEnded;

                    _player.Play();



                }
                else
                {
                    pgrLoading.IsActive = true;

                    // Say random sentence
                    Random random = new Random();
                    List<string> randomQuotes = new List<string>();
                    randomQuotes.Add("Hi there!");
                    randomQuotes.Add("It's dangerous to go alone, say this!");
                    randomQuotes.Add("I used to be an useful computer, but then I took an Talkinator to the knee.");
                    randomQuotes.Add("Insert random quote here.");
                    randomQuotes.Add("You know, you could also fill in the textbox above.");
                    randomQuotes.Add("Did you know that you can export your own quotes as audio files?");
                    randomQuotes.Add("Yabba-dabba-rediculously-doo!");
                    randomQuotes.Add("Did you know that the programmer couldn't come up with funny lines, so he tried those old forced ones?");
                    randomQuotes.Add("Thank god for Jim Sterling");
                    randomQuotes.Add("What are your favourite videogames?");
                    randomQuotes.Add("Premium Quality, serve loud!");
                    randomQuotes.Add("Did you know that the programmer is still really fond of the videogame Burnout 2: Point of Impact? It's old, but still holds up pretty well. Give it a shot!");
                    randomQuotes.Add("Quote number 13. I still haven't been spotted yet.");
                    randomQuotes.Add("RIP AND TEAR");
                    randomQuotes.Add("Get to the choppa");
                    randomQuotes.Add("Have you tried using Snips? I've heared it's pretty neat and from the same developer.");
                    randomQuotes.Add("Ohaio gozaimasu! Man, learning Japanese is hard!");
                    randomQuotes.Add("Agents are go!");
                    randomQuotes.Add("Gotta catch them all!");
                    randomQuotes.Add("Why? Because I'm a soul man");
                    randomQuotes.Add("You may guess three times who I am");
                    randomQuotes.Add("On World of Warcraft, I've a character of level 40. How high is yours?");
                    randomQuotes.Add("Wrong number!");
                    randomQuotes.Add("Why are you pressing the play-button? You didn't fill anything in for me to say!");
                    randomQuotes.Add("Never gonna give you up, never gonna let you down.");
                    randomQuotes.Add("Did you know that I was originally going to be called the Talkulator? The programmer changed it after he would repeatedly call me the Talkinator when talking about it to friends.");


                    SpeechSynthesisStream synthStream = await synth.SynthesizeTextToStreamAsync(randomQuotes[random.Next(randomQuotes.Count)]);
                    MediaSource mediaSource = MediaSource.CreateFromStream(synthStream, "audio");
                    MediaPlaybackItem playbackItem = new MediaPlaybackItem(mediaSource);

                    MediaItemDisplayProperties mediaProps = playbackItem.GetDisplayProperties();

                    mediaProps.Type = MediaPlaybackType.Music;
                    mediaProps.MusicProperties.Artist = "Talkinator"; // TODO Get name of selected Voice and paste it as Talkinator 'Bob'
                    mediaProps.MusicProperties.Title = "Spoken text";
                    //mediaProps.MusicProperties.Genres.Add("Speech");
                    playbackItem.ApplyDisplayProperties(mediaProps);

                    _player.SystemMediaTransportControls.IsEnabled = true;

                    _player.Source = playbackItem;

                    pgrLoading.IsActive = false;

                    _player.AutoPlay = true;
                    _player.Play();
                }
            }
            
            btnPlay.Visibility = Visibility.Collapsed;
            btnPause.Visibility = Visibility.Visible;
            txtTextToSay.XYFocusDown = btnPause;
            btnGoToPatreon.XYFocusDown = btnPause;
            btnToSpeechSettings.XYFocusUp = btnPause;
        }

        private async void _player_MediaEnded(MediaPlayer sender, object args)
        {
            //throw new NotImplementedException();
            //var dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                btnPause.Visibility = Visibility.Collapsed;
                btnPlay.Visibility = Visibility.Visible;
                txtTextToSay.XYFocusDown = btnPlay;
                btnGoToPatreon.XYFocusDown = btnPlay;
                btnToSpeechSettings.XYFocusUp = btnPlay;
            });

            //mediaEnded();
        }

        private async void btnTitle_Click(object sender, RoutedEventArgs e)
        {
            SpeechSynthesisStream synthStream = await synth.SynthesizeTextToStreamAsync("I'll be back");

            // Set the source of the mediaelement to the synth
            MediaSource mediaSource = MediaSource.CreateFromStream(synthStream, "audio");
            MediaPlaybackItem playbackItem = new MediaPlaybackItem(mediaSource);

            MediaItemDisplayProperties mediaProps = playbackItem.GetDisplayProperties();

            mediaProps.Type = MediaPlaybackType.Music;
            mediaProps.MusicProperties.Artist = "Talkinator"; // TODO Get name of selected Voice and paste it as Talkinator 'Bob'
            mediaProps.MusicProperties.Title = "Spoken text";
            //mediaProps.MusicProperties.Genres.Add("Speech");
            playbackItem.ApplyDisplayProperties(mediaProps);

            _player.SystemMediaTransportControls.IsEnabled = true;

            _player.Source = playbackItem;

            pgrLoading.IsActive = false;

            _player.AutoPlay = true;
            _player.Play();
        }

        private void cbtnAbout_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnOpenChangelog_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri(@"https://ikarago.tumblr.com/talkinator-changelog");
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private async void btnSendFeedback_Click(object sender, RoutedEventArgs e)
        {
            // Launch an URI-link
            var uri = new Uri(@"mailto:ikarago@outlook.com");
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtTextToSay.Text = "";
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            _player.Pause();
            btnPause.Visibility = Visibility.Collapsed;
            btnPlay.Visibility = Visibility.Visible;
            txtTextToSay.XYFocusDown = btnPlay;
            btnGoToPatreon.XYFocusDown = btnPlay;
            btnToSpeechSettings.XYFocusUp = btnPlay;
        }

        private async void cbtnExport_Click(object sender, RoutedEventArgs e)
        {
            ExportService exportHelper = new ExportService();
            var cbItem = (ComboBoxItem)cboxVoices.SelectedItem;
            var voice = (VoiceInformation)cbItem.Tag;

            await exportHelper.ExportSpeechToFile(txtTextToSay.Text, voice);
        }

        private void txtTextToSay_DragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
        }

        private async void txtTextToSay_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.Text))
            {
                try
                {
                    string copiedData = await e.DataView.GetTextAsync();
                    txtTextToSay.Text = copiedData;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

            }
        }

        private void cboxVoices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cbItem = (ComboBoxItem)cboxVoices.SelectedItem;
            var voice = (VoiceInformation)cbItem.Tag;
            synth.Voice = voice;
            Debug.WriteLine("Selected Id = " + voice.Id + " " + voice.DisplayName + " (Language: " + voice.Language + ", Gender: " + voice.Gender + ")");

        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            _player.Pause();
            _pauseStatus = true;
            btnPause.Visibility = Visibility.Collapsed;
            btnPlay.Visibility = Visibility.Visible;
            txtTextToSay.XYFocusDown = btnPlay;
            btnGoToPatreon.XYFocusDown = btnPlay;
            btnToSpeechSettings.XYFocusUp = btnPlay;
        }

        private async void mediaEnded()
        {
            btnPause.Visibility = Visibility.Collapsed;
            btnPlay.Visibility = Visibility.Visible;
            txtTextToSay.XYFocusDown = btnPlay;
            btnGoToPatreon.XYFocusDown = btnPlay;
            btnToSpeechSettings.XYFocusUp = btnPlay;
        }

        private void tbtnRepeat_Checked(object sender, RoutedEventArgs e)
        {
            _loopEnabled = tbtnRepeat.IsChecked.Value;

            if (_loopEnabled == true)
            {
                _player.IsLoopingEnabled = true;
            }
            else
            {
                _player.IsLoopingEnabled = false;
            }
        }

        private async void cbtnSupportUs_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri(@"http://www.patreon.com/ikarago");
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private void btnClosePatreonAd_Click(object sender, RoutedEventArgs e)
        {
            gridPatreonBanner.Visibility = Visibility.Collapsed;
            btnPlay.XYFocusUp = txtTextToSay;
            btnPause.XYFocusUp = txtTextToSay;
        }

        private async void btnToSpeechSettings_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri(@"ms-settings:speech");
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}