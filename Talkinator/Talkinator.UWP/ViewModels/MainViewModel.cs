using System;
using System.Windows.Input;
using Talkinator.UWP.Helpers;
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

        // Speech Synth
        private SpeechSynthesizer _speechSynthesizer;
               
        // List of Voices

        // Media controls



        // Constructor
        public MainViewModel()
        {
            // Initialize this
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



        // Methods
        private void ClearText()
        {
            // #TODO Display a warning with a question if the user is sure
            Text = "";
        }

    }
}
