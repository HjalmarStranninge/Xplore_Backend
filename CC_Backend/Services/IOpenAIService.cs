using OpenAI_API;
using static OpenAI_API.Chat.ChatMessage;

namespace CC_Backend.Services
{
    public interface IOpenAIService
    {
        Task<string> ReadImage(string path, string prompt);
    }
    public class OpenAIService : IOpenAIService 
    {
        private HttpClient _httpClient;
        private string _apiKey; 

        public OpenAIService(string apiKey): this(new HttpClient(), apiKey)
        {

        }

        public OpenAIService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<string> ReadImage (string path, string prompt)
        {
            var api = new OpenAI_API.OpenAIAPI(_apiKey);
            var result = await api.Chat.CreateChatCompletionAsync(prompt, ImageInput.FromFile(path));
            return result.ToString();
        }

    } 


}
