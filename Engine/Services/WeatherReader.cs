using System;
using System.Net;
using System.Xml;
using Engine.Models;
using Engine.Utilities;

namespace Engine.Services
{
    public static class WeatherReader
    {
        private const string OWM_CURRENT_WEATHER_URL =
            @"http://api.openweathermap.org/data/2.5/weather?zip={0},{1}&appid={2}&mode=xml";

        private const string OWM_FORECAST_WEATHER_URL =
            @"http://api.openweathermap.org/data/2.5/forecast?zip={0},{1}&appid={2}&mode=xml";

        private static DateTime _lastTimeRead;
        private static WeatherForecast _lastForecast;

        public static WeatherForecast GetCurrentForecast(
            string zipCode, string countryCode, TemperatureUnit unit)
        {
            if(_lastForecast != null)
            {
                TimeSpan span = DateTime.Now - _lastTimeRead;

                if(span.TotalMinutes < 5)
                {
                    return _lastForecast;
                }
            }

            string url = OWM_CURRENT_WEATHER_URL
                .Replace("{0}", zipCode)
                .Replace("{1}", countryCode)
                .Replace("{2}", Application.Default.OWMAPIKey);

            using(WebClient client = new WebClient())
            {
                XmlDocument weatherXML = new XmlDocument();
                weatherXML.LoadXml(client.DownloadString(url));

                decimal currentTemperatureKelvin = weatherXML.AttributeAsDecimal("/current/temperature/@value");
                decimal minTemperatureKelvin = weatherXML.AttributeAsDecimal("/current/temperature/@min");
                decimal maxTemperatureKelvin = weatherXML.AttributeAsDecimal("/current/temperature/@max");

                WeatherForecast currentWeatherForecast =
                    new WeatherForecast
                    {
                        TemperatureCurrent =
                            unit == TemperatureUnit.Farenheit
                                ? KelvinToFarenheit(currentTemperatureKelvin)
                                : KelvinToCelsius(currentTemperatureKelvin),
                        TemperatureMinimum =
                            unit == TemperatureUnit.Farenheit
                                ? KelvinToFarenheit(minTemperatureKelvin)
                                : KelvinToCelsius(minTemperatureKelvin),
                        TemperatureMaximum =
                            unit == TemperatureUnit.Farenheit
                                ? KelvinToFarenheit(maxTemperatureKelvin)
                                : KelvinToCelsius(maxTemperatureKelvin)
                    };

                _lastTimeRead = DateTime.Now;
                _lastForecast = currentWeatherForecast;

                return currentWeatherForecast;
            }
        }

        private static decimal KelvinToFarenheit(decimal kelvinTemperature)
        {
            return (kelvinTemperature - 273M) * 9M / 5M + 32M;
        }

        private static decimal KelvinToCelsius(decimal kelvinTemperature)
        {
            return kelvinTemperature - 273M;
        }
    }
}