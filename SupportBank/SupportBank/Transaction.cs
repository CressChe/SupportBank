﻿using NLog;
using System;

namespace SupportBank
{
    internal class Transaction
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public DateTime Date { get; }
        public string From { get; }
        public string To { get; }
        public string Narrative { get; }
        public double Amount { get; }

        public Transaction(string[] values)
        {
            Logger.Debug($"Creating Transaction with values - Date: {values[0]}, From: {values[1]}, To: {values[2]}, Narrative: {values[3]}, Amount: {values[4]}");
            Date = values[0];
            From = values[1];
            To = values[2];
            Narrative = values[3];
            Amount = Convert.ToDouble(values[4]);
        }
    }
}
