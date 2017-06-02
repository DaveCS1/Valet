using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Speech.Synthesis;
using System.Windows;
using Engine;
using Engine.Models;
using Engine.Services;
using Engine.Utilities;

namespace Valet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SpeechSynthesizer _speechSynthesizer = new SpeechSynthesizer();

        public MainWindow()
        {
            InitializeComponent();

            InitializeVoice();
        }

        private void InitializeVoice()
        {
            ReadOnlyCollection<InstalledVoice> voices =
                _speechSynthesizer.GetInstalledVoices();

            if(!string.IsNullOrWhiteSpace(Settings.Default.VoiceName) &&
               voices.Any(v => v.VoiceInfo.Name == Settings.Default.VoiceName))
            {
                _speechSynthesizer.SelectVoice(Settings.Default.VoiceName);
            }
            else
            {
                if(Settings.Default.VoiceGender.Equals("Male",
                                                       StringComparison.CurrentCultureIgnoreCase))
                {
                    _speechSynthesizer.SelectVoiceByHints(VoiceGender.Male);
                }
                else if(Settings.Default.VoiceGender.Equals("Female",
                                                            StringComparison.CurrentCultureIgnoreCase))
                {
                    _speechSynthesizer.SelectVoiceByHints(VoiceGender.Female);
                }
                else
                {
                    _speechSynthesizer.SelectVoiceByHints(VoiceGender.Neutral);
                }
            }

            _speechSynthesizer.Rate = Settings.Default.VoiceRate;
            _speechSynthesizer.Volume = Settings.Default.VoiceVolume;
        }

        private void Speak_OnClick(object sender, RoutedEventArgs e)
        {
            WeatherForecast weatherForecast =
                WeatherReader.GetCurrentForecast(Settings.Default.PostalCode,
                                                 Settings.Default.CountryCode,
                                                 (TemperatureUnit)Enum.Parse(typeof(TemperatureUnit),
                                                                             Settings.Default.TemperatureUnit));

            _speechSynthesizer.SpeakAsync(GreetingBuilder.GetCurrentGreetingFor(Settings.Default.UserName));

            _speechSynthesizer.SpeakAsync(string.Format("The current temperature is {0} degrees.",
                                                        (int)weatherForecast.TemperatureCurrent));
            _speechSynthesizer.SpeakAsync(string.Format("Today's low will be {0}, and the high will be {1}.",
                                                        (int)weatherForecast.TemperatureMinimum,
                                                        (int)weatherForecast.TemperatureMaximum));
        }
    }
}