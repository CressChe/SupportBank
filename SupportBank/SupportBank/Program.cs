using System;
using System.IO;
using System.Collections.Generic;

namespace SupportBank
{
    class Program
    {
        static void Main(string[] args)
        {
            var transactionHolder = GetTransactionsFromCSV();
            var memberHolder = GetAllMembers(transactionHolder);
            UpdateMemberTotals(memberHolder, transactionHolder);
            PrintAllMemberTotals(memberHolder);
            PrintMemberTotal(memberHolder, "Todd");

            List<Transaction> GetTransactionsFromCSV()
            {
                var transactionHolder = new List<Transaction>();
                using (var reader = new StreamReader(@"C:\Training\SupportBank\Transactions2014.csv"))
                {
                    reader.ReadLine();

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        transactionHolder.Add(new Transaction(values[0], values[1], values[2], values[3], Convert.ToDouble(values[4])));
                    }
                }
                return transactionHolder;
            }

            List<Member> GetAllMembers(List<Transaction> transactionHolder)
            {
                var names = GetMemberSet(transactionHolder);
                return GetMembersFromNames(names);
            }

            HashSet<string> GetMemberSet(List<Transaction> transactionHolder)
            {
                var names = new List<string>();
                foreach (Transaction transaction in transactionHolder)
                {
                    names.Add(transaction.From);
                    names.Add(transaction.To);
                }
                return new HashSet<string>(names);
            }

            List<Member> GetMembersFromNames(HashSet<string> names)
            {
                var memberHolder = new List<Member>();
                foreach (string name in names)
                {
                    memberHolder.Add(new Member(name));
                }
                return memberHolder;
            }

            void UpdateMemberTotals(List<Member> members, List<Transaction> transactions)
            {
                foreach (Transaction transaction in transactions)
                {
                    var memberFrom = members.Find(item => item.Name == transaction.From);
                    var memberTo = members.Find(item => item.Name == transaction.To);

                    memberFrom.Total -= transaction.Amount;
                    memberTo.Total += transaction.Amount;
                }
            }

            void PrintAllMemberTotals(List<Member> members)
            {
                foreach (Member member in members)
                {
                    Console.WriteLine(member.Name + ": " + member.Total);
                }
            }

            void PrintMemberTotal(List<Member> members, string name)
            {
                var member = members.Find(item => item.Name == name);
                Console.WriteLine(member.Name + ": " + member.Total);
            }
        }
    }
}
