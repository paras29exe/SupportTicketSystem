using Microsoft.Extensions.Configuration;
using SupportTicketSystem.Core.Exceptions;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SupportTicketSystem.Infrastructure.ExternalApi
{
    public class StarWarsApi(IConfiguration configuration)
    {
        private readonly string fileName = configuration["StarWarsApi:FileName"] ?? throw new AppException(404,  "StarWarsApi:FileName is not configured in appsettings.json");
        private readonly string StorageDirectoryPath = configuration["StarWarsApi:StorageDirectoryPath"] ?? throw new AppException(404, "StarWarsApi:StorageDirectoryPath is not configured in appsettings.json");

        private readonly string apiUrl = "https://swapi.py4e.com/api/people/";

        public async Task<string> CallExternalApiAsync()
        {
            string filePath = Path.Combine(StorageDirectoryPath, fileName);

            string? directory = Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(directory))
            {
                throw new AppException(500, $"Failed to determine directory: '{directory}' for StarWarsApi file path.", fileName);
            }

            if (!Directory.Exists(directory))
            {
                throw new AppException(404, $"Directory '{directory}' does not exist. Please ensure the directory is created and accessible.");
            }

            if(Path.GetExtension(filePath) != ".json")
            {
                throw new AppException(400, $"File '{filePath}' must have a .json extension.");
            }

            using HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                string failure = $"Request failed with Status : {response.StatusCode} due to {response.ReasonPhrase}";
                await File.WriteAllTextAsync(filePath, failure);
                throw new AppException((int)response.StatusCode, $"Error calling external API: {response.ReasonPhrase}");
            }

            string data = await response.Content.ReadAsStringAsync();

            try
            {
                using JsonDocument doc = JsonDocument.Parse(data);

                var people = doc.RootElement
                                .GetProperty("results")
                                .EnumerateArray()
                                .Take(5)
                                .Select(person => new
                                {
                                    Name = person.GetProperty("name").GetString(),
                                    Height = person.GetProperty("height").GetString()
                                })
                                .ToList();

                string formatted = JsonSerializer.Serialize(people, new JsonSerializerOptions { WriteIndented = true });
    
                // write only the name and height for first 5 people from the api
                await File.WriteAllTextAsync(filePath, formatted);

                return filePath;
            }
            catch (JsonException)
            {
                await File.WriteAllTextAsync(filePath, data);
                throw new AppException(500, "Failed to parse JSON response from external API.");
            }
        }
    }
}
