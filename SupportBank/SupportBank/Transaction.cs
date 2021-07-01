using Newtonsoft.Json;
using NLog;
using System;

namespace SupportBank
{
    internal class Transaction
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public DateTime Date { get; set; }
        [JsonProperty("FromAccount")]
        public string From { get; set; }
        [JsonProperty("ToAccount")]
        public string To { get; set; }
        public string Narrative { get; set; }
        public double Amount { get; set; }

        public Transaction() { }

        public Transaction(string[] values)
        {
            Logger.Debug($"Creating Transaction with values - Date: {values[0]}, From: {values[1]}, To: {values[2]}, Narrative: {values[3]}, Amount: {values[4]}");
            Date = Convert.ToDateTime(values[0]);
            From = values[1];
            To = values[2];
            Narrative = values[3];
            Amount = Convert.ToDouble(values[4]);
        }
    }
}
