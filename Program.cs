using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClashOfClans.Models;

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

            const string clanTag = "#P28LL99R";
            const string page = "AlphaGamers";

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

            var columns = new List<string>
            {
                "Name",
                "Town Hall",
                "Barbarian King",
                "Archer Queen",
                "Barbarian",
                "Archer",
                "Goblin",
                "Giant",
                "Wall Breaker",
                "Balloon",
                "Wizard",
                "Healer",
                "Dragon",
                "P.E.K.K.A",
                "Minion",
                "Hog Rider",
                "Valkyrie",
                "Golem",
                "Witch",
                "Lava Hound",
                "Baby Dragon",
                "Sneaky Goblin",
                "Lightning Spell",
                "Healing Spell",
                "Rage Spell",
                "Jump Spell",
                "Freeze Spell",
                "Poison Spell",
                "Earthquake Spell",
                "Haste Spell",
                "Skeleton Spell",
            };

            var sheet = new List<IList<object>> { columns.ToList<object>() };
            foreach (var clanMember in clan.MemberList)
            {
                var data = playerData[clanMember.Tag];
                var row = columns.Select(column => data.ContainsKey(column) ? data[column] : null);
                sheet.Add(row.ToList<object>());
            }

            var response = await spreadsheetConnector.UpdateDataAsync(sheet, page).ConfigureAwait(false);
            Console.WriteLine(response);
        }
    }
}