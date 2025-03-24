using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

class ApiService
{
    private readonly string apiToken;
    private readonly string url = "https://wdapi2.com.br/consulta";
    private string _placa;
    private string _modelo;
    private string _ano;
    private string _marca;

    public ApiService()
    {
        // Carrega a variável de ambiente do arquivo .env
        DotEnv.Load();
        apiToken = Environment.GetEnvironmentVariable("API_TOKEN") ?? throw new Exception("API_TOKEN não encontrado. Verifique seu arquivo .env");
    }

    public async Task GetVehicleInfoAsync(string placa)
    {
        string link = $"{url}/{placa}/{apiToken}";

        try
        {
            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<VehicleData>(responseBody);

            if (data == null || data.Erro != null)
                throw new Exception($"Erro na API: {data?.Erro}");

            _placa = placa;
            _modelo = data.Modelo;
            _ano = data.Ano;
            _marca = data.Marca;
        }
        catch (HttpRequestException e)
        {
            throw new Exception($"Erro na requisição: {e.Message}");
        }
    }

    public override string ToString()
    {
        return $"Placa: {_placa}, Modelo: {_modelo}, Ano: {_ano}, Marca: {_marca}";
    }

    // Propriedades somente leitura
    public string Placa => _placa;
    public string Modelo => _modelo;
    public string Ano => _ano;
    public string Marca => _marca;
}

// Classe para deserializar os dados da API
class VehicleData
{
    public string Modelo { get; set; }
    public string Ano { get; set; }
    public string Marca { get; set; }
    public string Erro { get; set; }
}

// Classe utilitária para carregar variáveis do arquivo .env
static class DotEnv
{
    public static void Load()
    {
        if (File.Exists(".env"))
        {
            foreach (var line in File.ReadAllLines(".env"))
            {
                var parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2)
                {
                    Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
                }
            }
        }
    }
}

// Exemplo de uso
class Program
{
    static async Task Main()
    {
        try
        {
            var apiService = new ApiService();
            await apiService.GetVehicleInfoAsync("ABC1234");
            Console.WriteLine(apiService);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
}
