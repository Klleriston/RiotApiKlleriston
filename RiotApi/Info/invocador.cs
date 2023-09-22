using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RiotApi.invocador
{
    public class Invocador
    {
        [JsonPropertyName("name")]
        public string? Nick { get; set; }
        [JsonPropertyName("id")]
        public string? id { get; set; }
        [JsonPropertyName("puuid")]
        public string? puuid { get; set; }
        [JsonPropertyName("accountId")]
        public string? accountID { get; set; }
    }
}