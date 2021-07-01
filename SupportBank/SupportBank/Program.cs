using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SupportBank
{
    internal class Program
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"C:\Training\SupportBank\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;

            var transactions = GetTransactionsFromCSV();
            var members = GetAllMembers(transactions);
            UpdateMemberTotals(members, transactions);

            Console.WriteLine("To print All return 'Y'");
            var printAll = Console.ReadLine();
            if (printAll == "Y" || printAll == "y")
            {
                PrintAllMemberTotals(members);
            }
            else
            {
                Console.WriteLine("Which member would you like to print?");
                var printMember = Console.ReadLine();
                PrintMemberTotal(members, printMember);
            }
        }

        private static List<Transaction> GetTransactionsFromCSV()
        { 
            Logger.Info("Parsing CSV");
            var lines = File.ReadAllLines(@"..\..\..\Transactions2014.csv").ToList();
            lines = lines.Skip(1).ToList();
            var transactions = lines.Select(line => new Transaction(line.Split(','))).ToList();
            return transactions;
        }

        private static List<Member> GetAllMembers(List<Transaction> transactions)
        {
            var toNames = transactions.Select(t => t.To);
            var fromNames = transactions.Select(t => t.From);
            var members = toNames.Concat(fromNames).Distinct().Select(name => new Member(name)).ToList();
            return members;
        }

        private static void UpdateMemberTotals(List<Member> members, List<Transaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                var memberFrom = members.Find(member => member.Name == transaction.From);
                var memberTo = members.Find(member => member.Name == transaction.To);

                memberFrom.Transactions.Add(transaction);
                memberTo.Transactions.Add(transaction);
            }
        }

        private static void PrintAllMemberTotals(List<Member> members)
        {
            foreach (var member in members)
            {
                Console.WriteLine(member.Name + ": " + member.Total);
            }
        }

        private static void PrintMemberTotal(List<Member> members, string name)
        {
            var member = members.Find(member => member.Name.ToLower() == name.ToLower());
            if (member == null)
            {
                Console.WriteLine($"Member {name} not found. Nothing to return.");
            }
            else
            {
                Console.WriteLine(member.Name + " Total: " + member.Total);
                Console.WriteLine("Date \t\t To \t\t From \t\t Amount \t\t Narrative");
                foreach (var transaction in member.Transactions)
                {
                    Console.WriteLine($"{transaction.Date} \t {transaction.To} \t\t {transaction.From} \t\t {transaction.Amount} \t\t {transaction.Narrative}");
                }
            }
        }

    }
}
