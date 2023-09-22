using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RiotApi.invocador;

namespace RiotApi.invocador
{
    public class Ranked
    {   
        [JsonPropertyName("queueType")]
        public string? Queue { get; set; }
        [JsonPropertyName("tier")]
        public string? Elo { get; set; }
        [JsonPropertyName("rank")]
        public string? Rank { get; set; }
        [JsonPropertyName("wins")]
        public int Wins { get; set; }
        [JsonPropertyName("losses")]
        public int loss { get; set; }
        [JsonPropertyName("leaguePoints")]
        public int Pontos { get; set; }
        [JsonPropertyName("summonerName")]
        public string? Nome { get; set; }
        [JsonPropertyName("teamId")]
        public int IdDoTime { get; set; }
    }
}