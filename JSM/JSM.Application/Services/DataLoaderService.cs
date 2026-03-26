using CsvHelper;
using JSM.Application.Dtos;
using JSM.Application.Interfaces;
using System.Globalization;
using System.Text.Json;

namespace JSM.Application.Services
{
    public class DataLoaderService : IDataLoaderService
    {
        private readonly HttpClient _httpClient;
        private readonly UserTransformerService _transformer;

        public DataLoaderService(HttpClient httpClient, UserTransformerService transformer)
        {
            _httpClient = httpClient;
            _transformer = transformer;
        }

        public async Task<List<UserDto>> LoadUsersAsync()
        {
            var users = new List<UserDto>();
            // Carrega CSV
            string csvUrl = "https://storage.googleapis.com/juntossomosmais-code-challenge/input-backend.csv";
            var csvContent = await _httpClient.GetStringAsync(csvUrl);
            users.AddRange(await LoadFromCsv(csvContent));

            // Carrega JSON
            string jsonUrl = "https://storage.googleapis.com/juntossomosmais-code-challenge/input-backend.json";
            var jsonContent = await _httpClient.GetStringAsync(jsonUrl);
            users.AddRange(await LoadFromJson(jsonContent));

            return users;
        }
       
        private async Task<List<UserDto>> LoadFromCsv(string csvContent)
        {
            var users = new List<UserDto>();

            using var reader = new StringReader(csvContent);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csv.GetRecords<dynamic>().ToList();

            foreach (var record in records)
            {
                users.Add(_transformer.TransformCSV(record));
            }

            return users;
        }

        private async Task<List<UserDto>> LoadFromJson(string jsonContent)
        {
            var users = new List<UserDto>();

            RootObject root = JsonSerializer.Deserialize<RootObject>(jsonContent);

            // Acessar a lista de usuários
            List<User> users2 = root.Results;

            foreach (var doc in root.Results)
            {
                users.Add(_transformer.TransformJSON(doc));
            }

            return users;
        }
    }
}
