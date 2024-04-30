using OpenAI_API;
using OpenAI_API.Models;
using static OpenAI_API.Chat.ChatMessage;

namespace CC_Backend.Services
{
    public interface IOpenAIService
    {
        Task<string> ReadImage(byte[] bytes, string prompt);
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
            var chat = api.Chat.CreateConversation();
            chat.Model = Model.GPT4_Vision;
            chat.AppendSystemMessage("Your job is to answer whether or not an object appears in the picture or not. Your answer should only be a numbers between 1-100, where 1 means the object is not in the picture and 100 means the object is definitely in the picture.");
            chat.AppendUserInput($"Does {prompt} appear in the image?", ImageInput.FromImageBytes(bytes));
            var response = await chat.GetResponseFromChatbotAsync();

            return response;


        }
    } 
}
