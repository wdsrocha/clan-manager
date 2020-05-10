using System.Collections.Generic;
using J = Newtonsoft.Json.JsonPropertyAttribute;

namespace ClanManager
{
    public class Request
    {
        [J("spreadsheetId")] public string SpreadsheetId { get; set; }
        [J("clanTag")] public string ClanTag { get; set; }
        [J("page")] public string Page { get; set; }
        [J("columns")] public List<Column> Columns { get; set; }
    }

    public class Column
    {
        [J("name")] public string Name { get; set; }
        [J("label")] public string Label { get; set; }
    }
}
