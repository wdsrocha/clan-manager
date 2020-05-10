using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClashOfClans.Models;
using Newtonsoft.Json;

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

            var requestFile = "request.json";
            try
            {
                requestFile = args[0];
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine($"Missing request file. Using default request file: {requestFile}");
            }

            var spreadsheetConnector = new Spreadsheet();

            Request request;
            using (var stream = new StreamReader(requestFile))
            {
                string json = stream.ReadToEnd();
                request = JsonConvert.DeserializeObject<Request>(json);
            }

            string clanTag = request.ClanTag;
            string page = request.Page;

            var coc = new ClashOfClansService();
            var clan = await coc.GetClanAsync(clanTag).ConfigureAwait(false);

            var playerData = new Dictionary<string, IDictionary<string, object>>();
            foreach (var clanMember in clan.MemberList)
            {
                var player = await coc.GetPlayerAsync(clanMember.Tag).ConfigureAwait(false);

                var items = player.Heroes
                    .Concat(player.Troops)
                    .Concat(player.Spells)
                    .Where(item => item.Village == Village.Home && !item.Name.Contains("Super"));

                playerData[player.Tag] = new Dictionary<string, object>
                {
                    ["Tag"] = player.Tag,
                    ["Name"] = player.Name,
                    ["Town Hall"] = player.TownHallLevel,
                };
                foreach (var item in items)
                {
                    playerData[player.Tag][item.Name] = item.Level;
                }
            }

            var columns = request.Columns;
            var sheet = new List<IList<object>> { columns.Select(column => column.Label).ToList<object>() };
            foreach (var clanMember in clan.MemberList)
            {
                var data = playerData[clanMember.Tag];
                var row = columns.Select(column =>
                {
                    var name = column.Name;
                    return data.ContainsKey(name) ? data[name] : "N/T";
                });
                sheet.Add(row.ToList<object>());
            }

            var response = await spreadsheetConnector.UpdateDataAsync(request.SpreadsheetId, sheet, page).ConfigureAwait(false);
            Console.WriteLine(response);
        }
    }
}