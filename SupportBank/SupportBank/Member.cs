using NLog;
using System.Collections.Generic;
using System.Linq;

namespace SupportBank
{
    internal class Member
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public string Name { get; }
        public double Total => Transactions.Sum(transaction => transaction.FromAccount == Name ? -transaction.Amount : transaction.Amount);
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public Member(string name)
        {
            Logger.Debug($"Creating Member: {name}");
            Name = name;
        }
    }
}
