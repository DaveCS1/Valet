using System;
using System.Net;
using System.Xml;
using Engine.Models;
using Engine.Resources;
using Engine.Utilities;

namespace Engine.Services
{
    public static class WeatherReader
    {
        private const string OWM_CURRENT_WEATHER_URL =
            @"http://api.openweathermap.org/data/2.5/weather?zip={0},{1}&appid={2}&mode=xml";

        private const string OWM_FORECAST_WEATHER_URL =
            @"http://api.openweathermap.org/data/2.5/forecast?zip={0},{1}&appid={2}&mode=xml";

        private static DateTime _latestTimeRetrieved;
        private static WeatherForecast _latestCurrentWeatherForecast;

        public static string GetCurrentForecast(
            string postalCode, string countryCode, TemperatureUnit unit)
        {
            WeatherForecast weatherForecast =
                RetrieveCurrentWeatherForecast(postalCode, countryCode, unit);

            return string.Format(Literals.Weather_CurrentTemperature,
                                 (int)weatherForecast.TemperatureCurrent);
        }

        public static string GetTodaysLowAndHighTemperatures(
            string postalCode, string countryCode, TemperatureUnit unit)
        {
            WeatherForecast weatherForecast =
                RetrieveCurrentWeatherForecast(postalCode, countryCode, unit);

            return string.Format(Literals.Weather_LowHighTemperatures,
                                 (int)weatherForecast.TemperatureMinimum,
                                 (int)weatherForecast.TemperatureMaximum);
        }

        private static WeatherForecast RetrieveCurrentWeatherForecast(
            string postalCode, string countryCode, TemperatureUnit unit)
        {
            // If we retrieved this data in the last five minutes,
            // use the previsouly-retrieved values.
            // This is so we don't hit any request limits for the API.
            if(_latestCurrentWeatherForecast != null)
            {
                if((DateTime.Now - _latestTimeRetrieved).TotalMinutes < 5)
                {
                    return _latestCurrentWeatherForecast;
                }
            }

            string url = OWM_CURRENT_WEATHER_URL
                .Replace("{0}", postalCode)
                .Replace("{1}", countryCode)
                .Replace("{2}", Application.Default.OWMAPIKey);

            using(WebClient client = new WebClient())
            {
                XmlDocument weatherXML = new XmlDocument();
                weatherXML.LoadXml(client.DownloadString(url));

                decimal currentTemperatureKelvin = weatherXML.AttributeAsDecimal("/current/temperature/@value");
                decimal minTemperatureKelvin = weatherXML.AttributeAsDecimal("/current/temperature/@min");
                decimal maxTemperatureKelvin = weatherXML.AttributeAsDecimal("/current/temperature/@max");

                WeatherForecast weatherForecast =
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

                _latestTimeRetrieved = DateTime.Now;
                _latestCurrentWeatherForecast = weatherForecast;

                return weatherForecast;
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