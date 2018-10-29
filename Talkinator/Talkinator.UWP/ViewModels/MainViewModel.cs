﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Talkinator.UWP.Helpers;
using Talkinator.UWP.Models;
using Talkinator.UWP.Services;
using Talkinator.UWP.Views;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;

namespace Talkinator.UWP.ViewModels
{
    public class MainViewModel : NotificationBase
    {
        // Properties
        private string _text;
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set { SetProperty(ref _isPlaying, value); }
        }
        public bool IsNotPlaying
        {
            get { return !_isPlaying; }
        }

        private bool _isPreparing;
        public bool IsPreparing
        {
            get { return _isPreparing; }
            set { SetProperty(ref _isPreparing, value); }
        }

        private bool _hasPlaybackStopped;
        public bool HasPlaybackStopped
        {
            get { return _hasPlaybackStopped; }
            set { SetProperty(ref _hasPlaybackStopped, value); }
        }


        // Media stuff
        private MediaPlayer _mediaPlayer;
        public MediaPlayer MediaPlayer
        {
            get { return _mediaPlayer; }
            set { SetProperty(ref _mediaPlayer, value); }
        }
        private MediaPlaybackSession _mediaSession
        {
            get { return _mediaPlayer.PlaybackSession; }
        }

        private SystemMediaTransportControls _mediaControls;
        private SpeechSynthesizer _speechSynthesizer;

        // List of Voices
        private ObservableCollection<VoiceModel> _voices;
        public ObservableCollection<VoiceModel> Voices
        {
            get { return _voices; }
            set { SetProperty(ref _voices, value); }
        }

        private VoiceModel _selectedVoice;
        public VoiceModel SelectedVoice
        {
            get { return _selectedVoice; }
            set
            {
                SetProperty(ref _selectedVoice, value);
                _speechSynthesizer.Voice = _selectedVoice.Voice;
            }
        }


        // Constructor
        public MainViewModel()
        {
            Initialize();
        }


        // Commands
        private ICommand _playCommand;
        public ICommand PlayCommand
        {
            get
            {
                if (_playCommand == null)
                {
                    _playCommand = new RelayCommand(
                        () =>
                        {
                            Play();
                        });
                }
                return _playCommand;
            }
        }

        private ICommand _pauseCommand;
        public ICommand PauseCommand
        {
            get
            {
                if (_pauseCommand == null)
                {
                    _pauseCommand = new RelayCommand(
                        () =>
                        {
                            Pause();
                        });
                }
                return _pauseCommand;
            }
        }

        private ICommand _stopCommand;
        public ICommand StopCommand
        {
            get
            {
                if (_stopCommand == null)
                {
                    _stopCommand = new RelayCommand(
                        () =>
                        {
                            Stop();
                        });
                }
                return _stopCommand;
            }
        }

        private ICommand _clearTextCommand;
        public ICommand ClearTextCommand
        {
            get
            {
                if (_clearTextCommand == null)
                {
                    _clearTextCommand = new RelayCommand(
                        () =>
                        {
                            ClearText();
                        });
                }
                return _clearTextCommand;
            }
        }

        private ICommand _exportCommand;
        public ICommand ExportCommand
        {
            get
            {
                if (_exportCommand == null)
                {
                    _exportCommand = new RelayCommand(
                    () =>
                    {
                        Export();
                    });
                }
                return _exportCommand;
            }
        }

        private ICommand _aboutCommand;
        public ICommand AboutCommand
        {
            get
            {
                if (_aboutCommand == null)
                {
                    _aboutCommand = new RelayCommand(
                        () =>
                        {
                            ShowAboutDialog();
                        });
                }
                return _aboutCommand;
            }
        }

        private ICommand _settingsCommand;
        public ICommand SettingsCommand
        {
            get
            {
                if (_settingsCommand == null)
                {
                    _settingsCommand = new RelayCommand(
                        () =>
                        {
                            ShowSettingsDialog();
                        });
                }
                return _settingsCommand;
            }
        }

        private ICommand _voiceSettingsCommand;
        public ICommand VoiceSettingsCommand
        {
            get
            {
                if (_voiceSettingsCommand == null)
                {
                    _voiceSettingsCommand = new RelayCommand(
                        () =>
                        {
                            ToVoiceSettings();
                        });
                }
                return _voiceSettingsCommand;
            }
        }



        // Methods
        private void Initialize()
        {
            // #TODO: Make this more foolproof for when there aren't any voices available
            _speechSynthesizer = new SpeechSynthesizer();
            _mediaPlayer = new MediaPlayer();
            _voices = new ObservableCollection<VoiceModel>();

            GetVoices();

            // Set stuff for the Media Player
            IsPlaying = false;
            HasPlaybackStopped = true;
            _mediaPlayer.AutoPlay = false;
            _mediaPlayer.AudioCategory = MediaPlayerAudioCategory.Speech;
            _mediaControls = _mediaPlayer.SystemMediaTransportControls;
            _mediaControls.AutoRepeatMode = MediaPlaybackAutoRepeatMode.None;

            // Set the Text so it isn't null
            Text = "";
        }

        private void GetVoices()
        {
            var voices = SpeechSynthesizer.AllVoices;
            var defaultVoice = SpeechSynthesizer.DefaultVoice;

            // Put the VoiceInformation into an VoiceModel
            foreach (VoiceInformation voice in voices)
            {
                var voiceModel = new VoiceModel(voice);
                Voices.Add(voiceModel);

                // Check for the default voice of the system and if true set it as the currently selected voice
                if (voiceModel.VoiceId == defaultVoice.Id)
                {
                    SelectedVoice = voiceModel;
                }
            }
        }

        private async Task<bool> PreparePlayback()
        {
            IsPreparing = true;
            
            // Create an audio stream of the text
            SpeechSynthesisStream synthStream = await _speechSynthesizer.SynthesizeTextToStreamAsync(_text);
            MediaSource mediaSource = MediaSource.CreateFromStream(synthStream, "audio");
            // Now make a PlaybackItem from this stream
            MediaPlaybackItem playbackItem = new MediaPlaybackItem(mediaSource);

            // Now set the properties of this PlaybackItem
            MediaItemDisplayProperties mediaProperties = playbackItem.GetDisplayProperties();

            mediaProperties.Type = MediaPlaybackType.Music;
            mediaProperties.MusicProperties.Artist = ("Talkinator voiced by " + SelectedVoice.VoiceName);
            mediaProperties.MusicProperties.Title = "Spoken text";

            playbackItem.ApplyDisplayProperties(mediaProperties);

            // Set this to enabled to make sure the info entered above will be shown in the System UI
            _mediaPlayer.SystemMediaTransportControls.IsEnabled = true;

            // Set the created item as a Source into the MediaPlayer
            _mediaPlayer.Source = playbackItem;

            // #TODO: Implement AutoPlay as well? Need to test their importance first
            _mediaPlayer.MediaEnded += _mediaPlayer_MediaEnded;

            // #TODO: Reimplement easter eggs
            IsPreparing = false;
            return true;
        }

        private async void _mediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                IsPlaying = false;
                HasPlaybackStopped = true;
            });
        }

        /// <summary>
        /// Start playback
        /// </summary>
        private async void Play()
        {
            // If paused; continue the playback session
            if (_mediaSession.PlaybackState == MediaPlaybackState.Paused && _hasPlaybackStopped == false)
            {
                IsPlaying = true;
                _mediaPlayer.Play();
            }
            else // otherwise start a new playback session
            {
                if (Text != "")
                {
                    await PreparePlayback();
                    HasPlaybackStopped = false;
                    IsPlaying = true;
                    _mediaPlayer.Play();
                }
            }
        }

        /// <summary>
        /// Pause playback
        /// </summary>
        private void Pause()
        {
            _mediaPlayer.Pause();
            IsPlaying = false;
        }

        /// <summary>
        /// Stop the current playback entirely
        /// </summary>
        private void Stop()
        {
            _mediaPlayer.Pause();
            IsPlaying = false;
            HasPlaybackStopped = true;
        }

        private void ClearText()
        {
            // #TODO Display a warning with a question if the user is sure
            Text = "";
        }

        private async void Export()
        {
            // #TODO: Make this a proper service, not the crap that it used to be
            try
            {
                var exportService = new ExportService();
                await exportService.ExportSpeechToFile(Text, SelectedVoice.Voice);
            }
            catch (Exception ex)
            {
                // #TODO: Reflect this error back at the user
                Debug.WriteLine("MainViewModel - Export - Failed - " + ex);
            }

        }

        private async void ShowAboutDialog()
        {
            var dialog = new AboutDialog();
            await dialog.ShowAsync();
        }

        private async void ShowSettingsDialog()
        {
            var dialog = new SettingsDialog();
            await dialog.ShowAsync();
        }

        private async void ToVoiceSettings()
        {
            var uri = new Uri(@"ms-settings:speech");
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
