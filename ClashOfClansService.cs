using System;
using System.IO;
using System.Threading.Tasks;
using ClashOfClans;
using ClashOfClans.Models;
using Newtonsoft.Json;

namespace ClanManager
{
    public class ClashOfClansCredential
    {
        public string token;
    }

    public class ClashOfClansService
    {
        private readonly ClashOfClansClient _clashOfClansClient;

        public ClashOfClansService()
        {
            ClashOfClansCredential credential;

            using (var stream = new StreamReader("coc_credentials.json"))
            {
                string json = stream.ReadToEnd();
                credential = JsonConvert.DeserializeObject<ClashOfClansCredential>(json);
            }

            _clashOfClansClient = new ClashOfClansClient(credential.token);
        }

        public async Task<Clan> GetClanDataAsync(string clanTag)
        {
            return await _clashOfClansClient.Clans.GetClanAsync(clanTag).ConfigureAwait(false);
        }
    }
}