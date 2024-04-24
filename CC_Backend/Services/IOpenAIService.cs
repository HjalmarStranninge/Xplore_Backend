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
        private string _apiKey; 

        public OpenAIService(string apiKey)
        {
            _apiKey = apiKey;
        }

        // Read an image with a prompt and get a response from GPT4-Vision
        public async Task<string> ReadImage (byte[] bytes, string prompt)
        {
            var api = new OpenAI_API.OpenAIAPI(_apiKey);
            var result = await api.Chat.CreateChatCompletionAsync(prompt, ImageInput.FromImageBytes(bytes));
            return result.ToString();
        }
    } 
}
