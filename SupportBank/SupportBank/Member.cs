﻿using NLog;
using System.Collections.Generic;
using System.Linq;

namespace SupportBank
{
    internal class Member
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public string Name { get; private set; }
        public double Total => Transactions.Sum(transaction => transaction.From == Name ? -transaction.Amount : transaction.Amount);

        public List<Transaction> Transactions;

        public Member(string name)
        {
            Name = name;
            Transactions = new List<Transaction>();
        }
    }
}
