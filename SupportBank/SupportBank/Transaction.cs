using NLog;
using System;

namespace SupportBank
{
    internal class Transaction
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public string Date { get; private set; }
        public string From { get; private set; }
        public string To { get; private set; }
        public string Narrative { get; private set; }
        public double Amount { get; private set; }

        public Transaction(string[] values)
        {
            Date = values[0];
            From = values[1];
            To = values[2];
            Narrative = values[3];
            Amount = Convert.ToDouble(values[4]);
        }
    }
}
