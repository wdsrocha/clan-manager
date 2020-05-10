// Based on https://gist.github.com/willchis/9e14a49df26034d60141586308084d65#file-gistfile1-txt
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;

namespace ClanManager
{
    public class Spreadsheet
    {
        private readonly string[] _scopes = { SheetsService.Scope.Spreadsheets };
        private readonly string _applicationName = "Clan Manager";
        private readonly string _spreadsheetId = "1jRMzCIXji371Et_NPowCPZibqwkdVR-XXau35S92Or8"; // wesleysr1997@gmail.com
        private readonly SheetsService _sheetsService;

        public Spreadsheet()
        {
            GoogleCredential credential;

            using (var stream = new FileStream("google_credentials.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(_scopes);
            }

            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName
            });
        }

        public async Task<IList<IList<object>>> GetDataAsync(string range)
        {
            var request = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, range);
            var response = await request.ExecuteAsync().ConfigureAwait(false);
            return response.Values;
        }

        public async Task<string> UpdateDataAsync(IList<IList<object>> data, string range)
        {
            const string valueInputOption = "USER_ENTERED";

            var updateData = new List<ValueRange>();
            var dataValueRange = new ValueRange
            {
                Range = range,
                Values = data
            };
            updateData.Add(dataValueRange);

            var requestBody = new BatchUpdateValuesRequest
            {
                ValueInputOption = valueInputOption,
                Data = updateData
            };

            var request = _sheetsService.Spreadsheets.Values.BatchUpdate(requestBody, _spreadsheetId);

            var response = await request.ExecuteAsync().ConfigureAwait(false);

            return JsonConvert.SerializeObject(response);
        }
    }
}