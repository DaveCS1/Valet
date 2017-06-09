using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Speech.Synthesis;
using System.Windows;
using Engine;
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

        private void Greeting_OnClick(object sender, RoutedEventArgs e)
        {
            SpeakGreeting();
        }

        private void CurrentTemperature_OnClick(object sender, RoutedEventArgs e)
        {
            SpeakCurrentTemperature();
        }

        private void TodaysLowAndHighTemperatures_OnClick(object sender, RoutedEventArgs e)
        {
            SpeakTodaysLowAndHighTemperatures();
        }

        private void CompleteStatus_OnClick(object sender, RoutedEventArgs e)
        {
            SpeakGreeting();
            SpeakCurrentTemperature();
            SpeakTodaysLowAndHighTemperatures();
        }

        private void SpeakGreeting()
        {
            Speak(GreetingBuilder.GetCurrentGreetingFor(Settings.Default.UserName));
        }

        private void SpeakCurrentTemperature()
        {
            Speak(WeatherReader.GetCurrentForecast(Settings.Default.PostalCode,
                                                   Settings.Default.CountryCode,
                                                   (TemperatureUnit)
                                                   Enum.Parse(typeof(TemperatureUnit),
                                                              Settings.Default.TemperatureUnit)));
        }

        private void SpeakTodaysLowAndHighTemperatures()
        {
            Speak(WeatherReader.GetTodaysLowAndHighTemperatures(Settings.Default.PostalCode,
                                                                Settings.Default.CountryCode,
                                                                (TemperatureUnit)
                                                                Enum.Parse(typeof(TemperatureUnit),
                                                                           Settings.Default.TemperatureUnit)));
        }

        private void Speak(string message)
        {
            _speechSynthesizer.SpeakAsync(message);
        }
    }
}