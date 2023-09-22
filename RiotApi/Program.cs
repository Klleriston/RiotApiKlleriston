using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using RiotApi.invocador;
using RiotApi.Invocador;
using System.Text.Json.Serialization;

class Program
{
    static async Task Main(string[] args)
    {
        string apiKey = "?api_key=YOUR-API-KEY";
        string baseUrl = "https://br1.api.riotgames.com/";

        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUrl);

            Invocador? invocador = null;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("BEM VINDO AO LOL STATS\n");
                Console.WriteLine("Escolha uma opção:");
                Console.WriteLine("1. Qual invocador deseja ver os stats?");
                Console.WriteLine("2. Buscar rank do invocador");
                Console.WriteLine("3. Exibir Challengers");
                Console.WriteLine("4. Exibe jogo atual do player");
                Console.WriteLine("5. Sair");

                Console.Write($"\nDigite uma opção:");
                string escolha = Console.ReadLine()!;

                switch (escolha)
                {
                    case "1":
                        invocador = await BuscarInvocador(client, apiKey);
                        break;
                    case "2":
                        await BuscarPerfilInvocador(client, invocador, apiKey);
                        break;
                    case "3":
                        await BuscaChallengers(client, apiKey, invocador);
                        break;
                    case "4":
                        await JogoAtual(client, apiKey, invocador);
                        break;
                    case "5":
                        Console.WriteLine("tchau tchau !! :P");
                        Console.WriteLine("Aperte um botão para sair");
                        Console.ReadKey();
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Opção inválida. Tente novamente.");
                        break;
                }
            }
        }
    }

    static async Task<Invocador> BuscarInvocador(HttpClient client, string apiKey)
    {
        try
        {
            Console.Write("Digite o nome do invocador: ");
            string nick = Console.ReadLine()!;

            string dadosDoInvocador = await client.GetStringAsync($"lol/summoner/v4/summoners/by-name/{nick.Replace(" ", "%20").ToLower()}{apiKey}");

            var invocador = JsonSerializer.Deserialize<Invocador>(dadosDoInvocador);

            Console.WriteLine("\nDigite uma tecla para sair");
            Console.ReadKey();
            Console.Clear();

            return invocador;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Erro ao buscar informações do invocador: {ex.Message}");
            return null;
        }
    }

    static async Task BuscarPerfilInvocador(HttpClient client, Invocador? invocador, string apiKey)
    {
        Console.Clear();
        if (invocador == null)
        {
            Console.WriteLine("Primeiro você deve buscar algum invocador");
            return;
        }

        try
        {
            string dadosRankedInvocador = await client.GetStringAsync($"lol/league/v4/entries/by-summoner/{invocador.id}{apiKey}");

            List<Ranked>? rankedInfo = JsonSerializer.Deserialize<List<Ranked>>(dadosRankedInvocador) ?? new List<Ranked>();

            Console.WriteLine("League wiki:\n");
            Console.WriteLine($"{invocador.Nick}");
            foreach (var leagueRank in rankedInfo)
            {
                Console.WriteLine($"\n{leagueRank.Queue?.Replace("_", " ")}\nElo: {leagueRank.Elo}, {leagueRank.Pontos} pdl ({leagueRank.Wins} wins, {leagueRank.loss} losses)\n");
            }

            Console.WriteLine("\nDigite uma tecla para sair");
            Console.ReadKey();
            Console.Clear();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Erro ao buscar informações de ranked: {ex.Message}");
        }
    }
    static async Task BuscaChallengers(HttpClient client, string apiKey, Invocador? invocador)
    {
        Console.Clear();
        Console.WriteLine("Esses são os challengers do servidor:\n");

        try
        {
            string dadosRankedTOP = await client.GetStringAsync($"https://br1.api.riotgames.com/lol/league/v4/challengerleagues/by-queue/RANKED_SOLO_5x5{apiKey}");

            var response = JsonDocument.Parse(dadosRankedTOP);

            if (response.RootElement.TryGetProperty("entries", out var entries))
            {
                var desafiantes = new List<Ranked>();
                foreach (var entry in entries.EnumerateArray())
                {
                    var playerName = entry.GetProperty("summonerName").GetString();
                    var playerRank = entry.GetProperty("rank").GetString();
                    var playerLeaguePoints = entry.GetProperty("leaguePoints").GetInt32();

                    desafiantes.Add(new Ranked
                    {
                        Nome = playerName,
                        Elo = playerRank,
                        Pontos = playerLeaguePoints
                    });
                }
                desafiantes = desafiantes.OrderByDescending(desafiantes => desafiantes.Pontos).ToList();
                foreach (var desafiante in desafiantes)
                {
                    await Console.Out.WriteLineAsync($"{desafiante.Nome} {desafiante.Pontos}pdl");
                    
                }
                Console.WriteLine("\nDigite uma tecla para sair");
                Console.ReadKey();
                Console.Clear();
            }
            else
            {
                Console.WriteLine("Não há challengers.");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Erro ao buscar informações de ranked: {ex.Message}");
        }
    }

    static async Task JogoAtual(HttpClient client, string apiKey, Invocador? invocador)
    {
        Console.Clear();
        if (invocador == null)
        {
            await Console.Out.WriteLineAsync("Por favor pesquise um invocador antes !");
            return;
        }

        try
        {
            string dadosDaPartida = await client.GetStringAsync($"https://br1.api.riotgames.com/lol/spectator/v4/active-games/by-summoner/{invocador.id}{apiKey}");

            var response = JsonDocument.Parse(dadosDaPartida);

            if (response.RootElement.TryGetProperty("participants", out var participant))
            {
                var invocadoresNaPartida = new List<Ranked>();
                foreach (var partc in participant.EnumerateArray())
                {
                    var PlayerNICK = partc.GetProperty("summonerName").GetString();
                    var PlayerTIME = partc.GetProperty("teamId").GetInt32();

                    invocadoresNaPartida.Add(new Ranked
                    {
                        Nome = PlayerNICK,
                        IdDoTime = PlayerTIME,
                    });
                }

                var formacaoTime = invocadoresNaPartida.GroupBy(invocadorTIME => invocadorTIME.IdDoTime);

                
                await Console.Out.WriteLineAsync($"Jogadores na partida:\n");
                int contagemDeTiME = formacaoTime.Count();
                int atualTimeIndex = 0; // 
                foreach (var teamGroup in formacaoTime)
                {
                    var teamInvocadores = teamGroup.Select(player => $"{player.Nome}").ToList();
                    var SeparadorTeam = string.Join("\n", teamInvocadores);

                    await Console.Out.WriteLineAsync(SeparadorTeam);

                    if (atualTimeIndex < contagemDeTiME - 1)
                    {
                        await Console.Out.WriteLineAsync($"vs");
                    }

                    atualTimeIndex++;
                }
                Console.WriteLine("\nDigite uma tecla para sair");
                Console.ReadKey();
                Console.Clear();
            }
            else
            {
                await Console.Out.WriteLineAsync("O invocador não está em um partida no momento.");
            }
        }
        catch (HttpRequestException ex)
        {
            await Console.Out.WriteLineAsync($"Erro ao buscar informações da partida: {ex.Message}");
        }
    }
}

