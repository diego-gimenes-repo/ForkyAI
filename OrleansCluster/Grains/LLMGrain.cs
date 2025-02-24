using Orleans;
using OrleansCluster.Abstractions.Grains;
using OrleansCluster.Grains.Interfaces;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ILLMGrain = OrleansCluster.Abstractions.Grains.ILLMGrain;

namespace OrleansCluster.Grains
{
    public class LLMGrain : Grain, ILLMGrain
    {
        private readonly HttpClient _httpClient;

        public LLMGrain(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> ProcessMessageAsync(string message, string model)
        {
            var requestBody = new
            {
                model = model,
                prompt = message
            };

            var jsonRequest = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:5050/api/semantickernel/execute", content);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }
    }
}
