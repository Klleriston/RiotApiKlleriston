using System;
using System.Text.Json.Serialization;

public class Champion
{
    [JsonPropertyName("id")]
    public int ChampionIDinvocador { get; set; }
    [JsonPropertyName("key")]
    public string? keyz { get; set; }

    [JsonPropertyName("championLevel")]
    public int NivelDeMaestria { get; set; }
}
