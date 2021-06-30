using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportBank
{
    class Transaction
    {
        public string Date { get; private set; }
        public string From { get; private set; }
        public string To { get; private set; }
        public string Narrative { get; private set; }
        public double Amount { get; private set; }

        public Transaction(string date, string from, string to, string narrative, double amount)
        {
            Date = date;
            From = from;
            To = to;
            Narrative = narrative;
            Amount = amount;
        }
    }
}
