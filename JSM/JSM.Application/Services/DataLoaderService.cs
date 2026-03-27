using CsvHelper;
using JSM.Application.Dtos;
using JSM.Application.Interfaces;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace JSM.Application.Services
{
    public class DataLoaderService : IDataLoaderService
    {
        private readonly HttpClient _httpClient;
        private readonly UserTransformerService _transformer;
        private readonly MinhaConfiguracao _config;

        public DataLoaderService(HttpClient httpClient, UserTransformerService transformer, IOptions<MinhaConfiguracao> options)
        {
            _httpClient = httpClient;
            _transformer = transformer;
            _config = options.Value;
        }

        public async Task<List<UserDto>> LoadUsersAsync()
        {

            var users = new List<UserDto>();
            // Carrega CSV
            string csvUrl = _config.csvUrl;
            var csvContent = await _httpClient.GetStringAsync(csvUrl);
            users.AddRange(await LoadFromCsv(csvContent));

            // Carrega JSON
            string jsonUrl = _config.jsonUrl;
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
