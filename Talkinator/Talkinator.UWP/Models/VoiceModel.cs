using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talkinator.UWP.Helpers;
using Windows.Media.SpeechSynthesis;

namespace Talkinator.UWP.Models
{
    public class VoiceModel
    {
        public VoiceInformation Voice { get; set; }
        public string VoiceId { get { return Voice.Id; } }
        public string VoiceName { get { return Voice.DisplayName; } }
        public string VoiceGender { get { return Voice.Gender.ToString(); } }
        public string VoiceLanguage { get { return Voice.Language; } }
        public string VoiceComboBoxName { get { return Voice.DisplayName + " (" + ResourceExtensions.GetLocalized("VoiceLanguage") + " " + Voice.Language + ", " + ResourceExtensions.GetLocalized("VoiceGender") + " " + Voice.Gender + ")"; } }

        // Contructor
        public VoiceModel(VoiceInformation voice)
        {
            Voice = voice;
        }
    }
}
