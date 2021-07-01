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
            var target = new FileTarget { FileName = @"..\..\..\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;

            var transactions = GetTransactionsFromCSV();
            var members = GetAllMembers(transactions);
            UpdateMemberTotals(members, transactions);

            PromptUserForInput(members);
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
            Logger.Info("Getting all members from transactions");
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
                Logger.Debug($"Transaction from {memberFrom.Name} to {memberTo.Name} of amount {transaction.Amount} processed");
            }
        }

        private static void PromptUserForInput(List<Member> members)
        {
            Console.WriteLine("Welcome to SupportBank!");
            Console.WriteLine("Would you like to: \n1) Check all account balances? \n2) Check transaction history for an account?");
            var userOption = Console.ReadLine();
            if (userOption == "1")
            {
                PrintAllMemberTotals(members);
            }
            else if (userOption == "2")
            {
                Console.WriteLine("Which member would you like to print the transaction history for?");
                var printMember = Console.ReadLine();
                PrintMemberTotal(members, printMember);
            }
            else
            {
                Console.WriteLine("Invalid option given. Please try again.");
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
