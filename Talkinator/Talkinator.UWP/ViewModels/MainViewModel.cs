using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Talkinator.UWP.Helpers;
using Talkinator.UWP.Models;
using Windows.Media;
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

        private bool _isLoopOn;
        public bool IsLoopOn
        {
            get { return _isLoopOn; }
            set { SetProperty(ref _isLoopOn, value); }
        }

        // Media stuff
        private MediaPlayer _mediaPlayer;
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
            set { SetProperty(ref _selectedVoice, value); }
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
                            // #TODO
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
                            // #TODO
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
                            // # TODO
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
                        // #TODO
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
                            // #TODO
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
                            // #TODO
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
                            // #TODO
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
            IsLoopOn = false;
            _mediaPlayer.AutoPlay = false;
            _mediaPlayer.AudioCategory = MediaPlayerAudioCategory.Speech;
            _mediaControls = _mediaPlayer.SystemMediaTransportControls;
            _mediaControls.AutoRepeatMode = MediaPlaybackAutoRepeatMode.None;
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

        private void ClearText()
        {
            // #TODO Display a warning with a question if the user is sure
            Text = "";
        }

    }
}
