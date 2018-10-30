using System;
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
using Windows.UI.Xaml.Controls;

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

        private string _secretText;
        public string SecretText
        {
            get { return _secretText; }
            set { SetProperty(ref _secretText, value); }
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
            SpeechSynthesisStream synthStream;
            // Check if any text has been entered, otherwise play a secret message
            if (Text == "")
            {
                // Randomize Text
                SetSecretText();
                synthStream = await _speechSynthesizer.SynthesizeTextToStreamAsync(SecretText);
            }
            else
            {
                synthStream = await _speechSynthesizer.SynthesizeTextToStreamAsync(Text);
            }
            MediaSource mediaSource = MediaSource.CreateFromStream(synthStream, "audio");
            // Now make a PlaybackItem from this stream
            MediaPlaybackItem playbackItem = new MediaPlaybackItem(mediaSource);

            // Now set the properties of this PlaybackItem
            MediaItemDisplayProperties mediaProperties = playbackItem.GetDisplayProperties();

            mediaProperties.Type = MediaPlaybackType.Music;
            mediaProperties.MusicProperties.Artist = ("Talkinator " + ResourceExtensions.GetLocalized("VoicedBy") + " " + SelectedVoice.VoiceName);
            mediaProperties.MusicProperties.Title = ResourceExtensions.GetLocalized("SpokenText");

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

        private void SetSecretText()
        {
            // Set a random secret sentence
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
            randomQuotes.Add("Wubbalubbadubdub!");
            randomQuotes.Add("We bring the FUN in to NO REFUNDS!");
            randomQuotes.Add("He's gonna take you back to the past...");
            randomQuotes.Add("Daisy, Daisy, give me your answer, do, I'm half crazy all for the love of you. It won't be a stylish marriage, I can't afford a carriage, But you'd look sweet upon the seat Of a bicycle made for two.");
            randomQuotes.Add("You gotta get Swifty!");
            randomQuotes.Add("Have you heard about Temida? This young scientist will define the boundraries of time. Maybe you could ask Ikarago about it! ;)");

            // In memory of mr. Erik 'Haakieeees!' Oltmans, my high school math teacher. Rest In Peace - 16-01-2017
            randomQuotes.Add("HAAKIEEEEEEEEES!");
            randomQuotes.Add("Pannekoek!");

            SecretText = randomQuotes[random.Next(randomQuotes.Count)];
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
                await PreparePlayback();
                HasPlaybackStopped = false;
                IsPlaying = true;
                _mediaPlayer.Play();
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
                bool success = await ExportService.ExportTextToSpeechFile(Text, SelectedVoice.Voice);
                if (success == true)
                {
                    // Show UX message telling exporting was successful
                }
                else
                {
                    // Show UX message telling exporting failed
                }
            }
            catch (Exception ex)
            {
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
