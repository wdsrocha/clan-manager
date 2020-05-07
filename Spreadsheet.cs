// Based on https://gist.github.com/willchis/9e14a49df26034d60141586308084d65#file-gistfile1-txt
using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;

namespace ClanManager
{
    public class Spreadsheet
    {
        private readonly string[] _scopes = { SheetsService.Scope.Spreadsheets }; // Change this if you're accessing Drive or Docs
        private readonly string _applicationName = "Clan Manager";
        private readonly string _spreadsheetId = "1jRMzCIXji371Et_NPowCPZibqwkdVR-XXau35S92Or8";
        private readonly SheetsService _sheetsService;

        public Spreadsheet()
        {
            GoogleCredential credential;

            // Put your credentials json file in the root of the solution and make sure copy to output dir property is set to always copy 
            using (var stream = new FileStream("google_credentials.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(_scopes);
            }

            // Create Google Sheets API service.
            _sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName
            });
        }

        // Pass in your data as a list of a list (2-D lists are equivalent to the 2-D spreadsheet structure)
        public async System.Threading.Tasks.Task<string> UpdateDataAsync(List<IList<object>> data)
        {
            const String range = "AlphaGamers";
            const string valueInputOption = "USER_ENTERED";

            // The new values to apply to the spreadsheet.
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