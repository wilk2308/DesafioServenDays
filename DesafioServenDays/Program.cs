using RestSharp;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TamagotchiPokemon
{
    public class PokemonListResponse
    {
        [JsonPropertyName("results")]
        public List<PokemonEntry> Results { get; set; }
    }

    public class PokemonEntry
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class PokemonDetails
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("weight")]
        public int Weight { get; set; }

        [JsonPropertyName("abilities")]
        public List<AbilityWrapper> Abilities { get; set; }
    }

    public class AbilityWrapper
    {
        [JsonPropertyName("ability")]
        public Ability Ability { get; set; }
    }

    public class Ability
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Bem-vindo ao Tamagotchi Pokémon!");
            Console.WriteLine("Buscando opções de Pokémons...\n");

            var client = new RestClient("https://pokeapi.co");
            var request = new RestRequest("/api/v2/pokemon?limit=10", Method.Get);

            RestResponse response = await client.ExecuteAsync(request);

            if (!response.IsSuccessful)
            {
                Console.WriteLine("Erro ao buscar Pokémons.");
                return;
            }

            var pokemonList = JsonSerializer.Deserialize<PokemonListResponse>(response.Content);

            if (pokemonList?.Results == null || pokemonList.Results.Count == 0)
            {
                Console.WriteLine("Nenhum Pokémon encontrado.");
                return;
            }

            for (int i = 0; i < pokemonList.Results.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {UpperFirst(pokemonList.Results[i].Name)}");
            }

            Console.Write("\n Digite o número do Pokémon que você quer conhecer melhor: ");
            if (!int.TryParse(Console.ReadLine(), out int escolha) || escolha < 1 || escolha > pokemonList.Results.Count)
            {
                Console.WriteLine("Escolha inválida.");
                return;
            }

            var selecionado = pokemonList.Results[escolha - 1];
            Console.WriteLine("\n🔍 Buscando informações sobre {UpperFirst(selecionado.Name)}...");

            var detailRequest = new RestRequest("/api/v2/pokemon/{selecionado.Name}", Method.Get);
            var detailResponse = await client.ExecuteAsync(detailRequest);

            if (!detailResponse.IsSuccessful)
            {
                Console.WriteLine(" Erro ao buscar detalhes.");
                return;
            }

            var detalhes = JsonSerializer.Deserialize<PokemonDetails>(detailResponse.Content);

            Console.WriteLine($"\n Detalhes de {UpperFirst(detalhes.Name)}:");
            Console.WriteLine($"- Altura: {detalhes.Height}");
            Console.WriteLine($"- Peso: {detalhes.Weight}");
            Console.WriteLine($"- Habilidades:");
            foreach (var hab in detalhes.Abilities)
            {
                Console.WriteLine($"  • {UpperFirst(hab.Ability.Name)}");
            }
        }

        static string UpperFirst(string text) =>
            string.IsNullOrEmpty(text) ? "" : char.ToUpper(text[0]) + text.Substring(1);
    }
}
