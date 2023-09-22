using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RiotApi.Invocador
{
    public class Maestrias
    {
        [JsonPropertyName("championId")]
        public string? championID { get; set; }
        [JsonPropertyName("championLevel")]
        public int nivelDeMaestria { get; set; }

        
    }
}