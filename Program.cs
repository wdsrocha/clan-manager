using System;
using System.Collections.Generic;
using ClashOfClans;
using System.Threading.Tasks;

namespace ClanManager
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var spreadsheetConnector = new Spreadsheet();

            var coc = new ClashOfClansService();
            var clan = await coc.GetClanDataAsync("#P28LL99R").ConfigureAwait(false);

            var data = new List<IList<object>>();
            foreach (var clanMember in clan.MemberList)
            {
                data.Add(new List<object>
                {
                    clanMember.Tag,
                    clanMember.Name,
                    clanMember.Donations.ToString()
                });
            }

            var response = await spreadsheetConnector.UpdateDataAsync(data).ConfigureAwait(false);
            Console.WriteLine(response);
        }
    }
}