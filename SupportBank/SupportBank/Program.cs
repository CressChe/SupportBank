using Newtonsoft.Json;
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
            SetUpLogging();

            var transactions = GetTransactionsFromJson();
            var members = GetAllMembers(transactions);
            UpdateMemberTotals(members, transactions);

            PromptUserForInput(members);
        }

        private static void SetUpLogging()
        {
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"..\..\..\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;
        }

        private static List<Transaction> GetTransactionsFromCSV()
        {
            Logger.Info("Parsing CSV");
            //var lines = File.ReadAllLines(@"..\..\..\Transactions2014.csv").ToList();
            var lines = File.ReadAllLines(@"..\..\..\DodgyTransactions2015.csv").ToList();
            lines = lines.Skip(1).ToList();
            var transactions = lines
                .Where(line => IsValidTransaction(line.Split(',')))
                .Select(line => new Transaction(line.Split(',')))
                .ToList();
            return transactions;
        }

        private static List<Transaction> GetTransactionsFromJson()
        {
            Logger.Info("Parsing Json");
            string input = File.ReadAllText(@"..\..\..\Transactions2013.json");
            return JsonConvert.DeserializeObject<List<Transaction>>(input);
        }

        private static bool IsValidTransaction(string[] values)
        {
            var hasValidAmount = double.TryParse(values[4], out _);
            var hasValidDate = DateTime.TryParse(values[0], out _);

            var error = "";
            if (!hasValidAmount)
            {
                error += $"Amount given invalid. ";
            }
            if (!hasValidDate)
            {
                error += $"Date given invalid. ";
            }
            if (!string.IsNullOrEmpty(error))
            {
                Logger.Error(error + $"Date: {values[0]}, From: {values[1]}, "
                    + $"To: {values[2]}, Narrative: {values[3]}, Amount: {values[4]}");
                Console.WriteLine("The following transaction could not be processed: \n" 
                    + $"Date: {values[0]}, From: {values[1]}, To: {values[2]}, Narrative: {values[3]}, Amount: {values[4]}");
            }

            return hasValidAmount && hasValidDate;
        }

        private static List<Member> GetAllMembers(List<Transaction> transactions)
        {
            Logger.Info("Getting all members from transactions");
            var toNames = transactions.Select(t => t.ToAccount);
            var fromNames = transactions.Select(t => t.FromAccount);
            var members = toNames.Concat(fromNames).Distinct().Select(name => new Member(name)).ToList();
            return members;
        }

        private static void UpdateMemberTotals(List<Member> members, List<Transaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                var memberFrom = members.Find(member => member.Name == transaction.FromAccount);
                var memberTo = members.Find(member => member.Name == transaction.ToAccount);

                memberFrom.Transactions.Add(transaction);
                memberTo.Transactions.Add(transaction);
                Logger.Debug($"Transaction from {memberFrom.Name} to {memberTo.Name} of amount {transaction.Amount} processed");
            }
        }

        private static void PromptUserForInput(List<Member> members)
        {
            Console.WriteLine("\nWelcome to SupportBank!");
            Console.WriteLine("Would you like to: \n1) Check all account balances? \n2) Check transaction history for an account?");
            var userOption = Console.ReadLine();
            Logger.Debug($"User Input: '{userOption}'");
            if (userOption == "1")
            {
                PrintAllMemberTotals(members);
            }
            else if (userOption == "2")
            {
                Console.WriteLine("Which member would you like to print the transaction history for?");
                var printMember = Console.ReadLine();
                Logger.Debug($"Account Name Input: '{printMember}'");
                PrintMemberTotal(members, printMember);
            }
            else
            {
                Console.WriteLine("Invalid option given. Please try again.");
                Logger.Debug($"User entered an invalid Option - programme rerun.");
                PromptUserForInput(members);
            }
        }


        private static void PrintAllMemberTotals(List<Member> members)
        {
            Logger.Debug("Printing totals for all members");
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
                Logger.Warn($"User input '{name}' did not match any members.");
                Console.WriteLine($"Member {name} not found. Nothing to return.");
                return;
            }

            Logger.Debug($"Printing account details for {member.Name}");
            Console.WriteLine(member.Name + " Total: " + member.Total);
            Console.WriteLine("Date \t\t To \t\t From \t\t Amount \t\t Narrative");
            foreach (var transaction in member.Transactions)
            {
                Console.WriteLine($"{transaction.Date} \t {transaction.ToAccount} \t\t {transaction.FromAccount} \t\t {transaction.Amount} \t\t {transaction.Narrative}");
            }
        }

    }
}
