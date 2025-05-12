using System;
using System.IO;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace UtilitareBanking
{
    public class CursValutar
    {
        private static string numeFisier;
        private static Dictionary<string, decimal> conversionRates;
        private static string monedaBaza;
        private static long timeNextUpdateUnix;

        private static readonly string ApiKey = "2392ce523f2a1b58e005bda0";

        private static bool initializat = false;

        // Clasă internă pentru Deserialize JSON
        private class ExchangeResponse
        {
            [JsonProperty("result")]
            public string Result { get; set; }

            [JsonProperty("base_code")]
            public string BaseCode { get; set; }

            [JsonProperty("time_next_update_unix")]
            public long TimeNextUpdateUnix { get; set; }

            [JsonProperty("time_next_update_utc")]
            public string TimeNextUpdateUtc { get; set; }

            [JsonProperty("conversion_rates")]
            public Dictionary<string, decimal> ConversionRates { get; set; }
        }

        public CursValutar(string numeFisierInput)
        {
            Stream streamFisierText = File.Open(numeFisierInput, FileMode.OpenOrCreate);
            streamFisierText.Close();

            numeFisier = numeFisierInput;
            conversionRates = null;
            initializat = true;
        }

        private static void IncarcaDacaEsteNecesar()
        {
            if (!initializat)
            {
                Console.WriteLine("Clasa CursValutar nu a fost initializata corect!");
                return;
            }

            if (conversionRates == null || DateTimeOffset.UtcNow.ToUnixTimeSeconds() > timeNextUpdateUnix)
            {
                if (File.Exists(numeFisier))
                {
                    string json = File.ReadAllText(numeFisier);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        var data = JsonConvert.DeserializeObject<ExchangeResponse>(json);
                        if (data != null && data.ConversionRates != null)
                        {
                            monedaBaza = data.BaseCode;
                            timeNextUpdateUnix = data.TimeNextUpdateUnix;

                            conversionRates = new Dictionary<string, decimal>();
                            foreach (var rate in data.ConversionRates)
                            {
                                conversionRates[rate.Key] = rate.Value;
                            }

                            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > timeNextUpdateUnix)
                            {
                                ActualizeazaDate();
                            }

                            return;
                        }
                    }
                }

                ActualizeazaDate();
            }
        }

        private static void ActualizeazaDate()
        {
            using (HttpClient client = new HttpClient())
            {
                string response = client.GetStringAsync($"https://v6.exchangerate-api.com/v6/{ApiKey}/latest/RON").Result;
                var data = JsonConvert.DeserializeObject<ExchangeResponse>(response);

                if (data == null || data.ConversionRates == null)
                {
                    Console.WriteLine("Nu am putut obtine cursurile valutare.");
                    return;
                }

                monedaBaza = data.BaseCode;
                timeNextUpdateUnix = data.TimeNextUpdateUnix;

                conversionRates = new Dictionary<string, decimal>();
                foreach (var rate in data.ConversionRates)
                {
                    conversionRates[rate.Key] = rate.Value;
                }

                SaveRatesToFile(data);
            }
        }

        private static void SaveRatesToFile(ExchangeResponse rates)
        {
            string json = JsonConvert.SerializeObject(rates, Formatting.Indented);
            File.WriteAllText(numeFisier, json);
        }

        public static decimal SchimbValutar(string monedaDeLa, string monedaLa)
        {
            IncarcaDacaEsteNecesar();

            Console.WriteLine($"Time Update : {timeNextUpdateUnix}, MonedaBaza: {monedaBaza}");

            if (!conversionRates.ContainsKey(monedaDeLa) || !conversionRates.ContainsKey(monedaLa))
            {
                Console.WriteLine($"Una dintre monede ({monedaDeLa} sau {monedaLa}) nu exista in cursurile disponibile.");
                return -1;
            }

            decimal rataDeLa = conversionRates[monedaDeLa];
            decimal rataLa = conversionRates[monedaLa];

            decimal rataConversie = rataLa / rataDeLa;
            return Math.Round(rataConversie, 4);
        }
    }
}
