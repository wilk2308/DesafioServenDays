using RestSharp; // biblioteca externa usada para fazer requisições HTTP de forma simplificada
using System; // traz funcionalidades básicas, como Console.WriteLine
using System.Text.Json; // usada para serializar e desserializar JSON, ou seja, converter JSON para objetos C# e vice-versa
using System.Text.Json.Serialization; // traz os atributos como [JsonPropertyName], para mapear nomes do JSON em propriedades C#
using System.Threading.Tasks; // permite usar async/await, ou seja, programação assíncrona (esperar respostas sem travar o programa)
using System.Collections.Generic; // dá acesso a coleções genéricas, como List<T>

namespace TamagotchiPokemon // Define o namespace do programa, que agrupa classes relacionadas e evita conflitos de nomes
{
    public class PokemonListResponse // Classe que representa a resposta da lista de Pokémons
    {
        [JsonPropertyName("results")] // Atributo que mapeia a propriedade "results" do JSON para a lista de Pokémons
        public List<PokemonEntry> Results { get; set; } // Lista de Pokémons retornada pela API
    }

    public class PokemonEntry // Classe que representa cada Pokémon na lista
    {
        [JsonPropertyName("name")] // Atributo que mapeia a propriedade "name" do JSON para o nome do Pokémon
        public string Name { get; set; } // Nome do Pokémon

        [JsonPropertyName("url")] // Atributo que mapeia a propriedade "url" do JSON para a URL do Pokémon
        public string Url { get; set; } // URL que pode ser usada para buscar mais detalhes sobre o Pokémon
    }

    public class PokemonDetails // Classe que representa os detalhes de um Pokémon específico
    {
        [JsonPropertyName("name")] // Atributo que mapeia a propriedade "name" do JSON para o nome do Pokémon
        public string Name { get; set; } // Nome do Pokémon

        [JsonPropertyName("height")] // Atributo que mapeia a propriedade "height" do JSON para a altura do Pokémon
        public int Height { get; set; } // Altura do Pokémon em decímetros (10 cm)

        [JsonPropertyName("weight")] // Atributo que mapeia a propriedade "weight" do JSON para o peso do Pokémon
        public int Weight { get; set; } // Peso do Pokémon em hectogramas (100 g)

        [JsonPropertyName("abilities")] // Atributo que mapeia a propriedade "abilities" do JSON para a lista de habilidades do Pokémon
        public List<AbilityWrapper> Abilities { get; set; } // Lista de habilidades do Pokémon, cada uma representada por um AbilityWrapper
    }

    public class AbilityWrapper // Classe que encapsula a habilidade de um Pokémon
    {
        [JsonPropertyName("ability")] // Atributo que mapeia a propriedade "ability" do JSON para a habilidade do Pokémon
        public Ability Ability { get; set; } // Objeto que representa a habilidade do Pokémon, contendo o nome da habilidade
    }

    public class Ability // Classe que representa uma habilidade de Pokémon
    {
        [JsonPropertyName("name")] // Atributo que mapeia a propriedade "name" do JSON para o nome da habilidade
        public string Name { get; set; } // Nome da habilidade do Pokémon
    }

    class Program // Classe principal do programa, onde a execução começa
    {
        static async Task Main(string[] args) // Método principal que é executado ao iniciar o programa
        {
            Console.WriteLine("Bem-vindo ao Tamagotchi Pokémon!"); // Mensagem de boas-vindas ao usuário
            Console.WriteLine("Buscando opções de Pokémons...\n"); // Informa que o programa está buscando Pokémons

            var client = new RestClient("https://pokeapi.co"); // Cria um cliente RestSharp para fazer requisições à API do Pokémon
            var request = new RestRequest("/api/v2/pokemon?limit=10", Method.Get); // Cria uma requisição para buscar os primeiros 10 Pokémons da API

            RestResponse response = await client.ExecuteAsync(request); // Executa

            if (!response.IsSuccessful) // Verifica se a requisição foi bem-sucedida
            {
                Console.WriteLine("Erro ao buscar Pokémons."); // Se não foi, exibe uma mensagem de erro
                return;
            }

            var pokemonList = JsonSerializer.Deserialize<PokemonListResponse>(response.Content); // Desserializa o conteúdo da resposta JSON para um objeto PokemonListResponse

            if (pokemonList?.Results == null || pokemonList.Results.Count == 0) // Verifica se a lista de Pokémons está vazia ou nula
            {
                Console.WriteLine("Nenhum Pokémon encontrado."); // Se estiver, exibe uma mensagem informando que não foram encontrados Pokémons
                return;
            }

            for (int i = 0; i < pokemonList.Results.Count; i++) // Itera sobre a lista de Pokémons
            {
                Console.WriteLine($"{i + 1}. {UpperFirst(pokemonList.Results[i].Name)}"); // Exibe o número e o nome de cada Pokémon, formatando o nome para começar com letra maiúscula
            }

            Console.Write("\n Digite o número do Pokémon que você quer conhecer melhor: "); // Solicita ao usuário que digite o número do Pokémon que deseja conhecer melhor
            if (!int.TryParse(Console.ReadLine(), out int escolha) || escolha < 1 || escolha > pokemonList.Results.Count) // Tenta converter a entrada do usuário para um número inteiro e verifica se está dentro do intervalo válido
            {
                Console.WriteLine("Escolha inválida."); // Se a entrada não for válida, exibe uma mensagem de erro
                return;
            }

            var selecionado = pokemonList.Results[escolha - 1]; // Obtém o Pokémon selecionado pelo usuário, subtraindo 1 do número escolhido para ajustar ao índice da lista
            Console.WriteLine($"\n Buscando informações sobre {UpperFirst(selecionado.Name)}..."); // Informa que está buscando informações sobre o Pokémon selecionado

            var detailRequest = new RestRequest($"/api/v2/pokemon/{selecionado.Name}", Method.Get); // Cria uma nova requisição para buscar detalhes do Pokémon selecionado usando seu nome na URL
            var detailResponse = await client.ExecuteAsync(detailRequest); // Executa a requisição para buscar os detalhes do Pokémon selecionado

            if (!detailResponse.IsSuccessful) // Verifica se a requisição para buscar os detalhes foi bem-sucedida
            {
                Console.WriteLine(" Erro ao buscar detalhes."); // Se não foi, exibe uma mensagem de erro
                return;
            }

            var detalhes = JsonSerializer.Deserialize<PokemonDetails>(detailResponse.Content); // Desserializa o conteúdo da resposta JSON para um objeto PokemonDetails

            Console.WriteLine($"\n Detalhes de {UpperFirst(detalhes.Name)}:"); // Exibe os detalhes do Pokémon selecionado, formatando o nome para começar com letra maiúscula
            Console.WriteLine($"- Altura: {detalhes.Height}"); // Exibe a altura do Pokémon
            Console.WriteLine($"- Peso: {detalhes.Weight}"); // Exibe o peso do Pokémon
            Console.WriteLine($"- Habilidades:"); // Informa que serão listadas as habilidades do Pokémon
            foreach (var hab in detalhes.Abilities) // Itera sobre a lista de habilidades do Pokémon
            {
                Console.WriteLine($"  • {UpperFirst(hab.Ability.Name)}"); // Exibe cada habilidade, formatando o nome para começar com letra maiúscula
            }
        }

        static string UpperFirst(string text) => // Método auxiliar para formatar o texto, colocando a primeira letra em maiúscula
            string.IsNullOrEmpty(text) ? "" : char.ToUpper(text[0]) + text.Substring(1); // Verifica se o texto é nulo ou vazio; se não for, coloca a primeira letra em maiúscula e concatena com o restante do texto
    }
}
