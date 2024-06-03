using OpenAI_API;
using OpenAI_API.Models;
using static OpenAI_API.Chat.ChatMessage;

namespace CC_Backend.Services
{
    public class OpenAIService : IOpenAIService
    {
        private string _apiKey;

        public OpenAIService(string apiKey)
        {
            _apiKey = apiKey;
        }

        // Read an image with a prompt and get a response from GPT4-Vision
        public async Task<string> ReadImage(string prompt, byte[] picture)
        {
            var api = new OpenAIAPI(_apiKey);
            var chat = api.Chat.CreateConversation();
            chat.Model = Model.GPT4_Vision;
            //chat.AppendSystemMessage("Your job is to answer whether or not an object appears in the picture or not. Your answer should only be a numbers between 1-100, where 1 means the object is not in the picture and 100 means the object is definitely in the picture.");
            chat.AppendSystemMessage("Analyze the provided image. Determine whether the specified object is present in the image. Respond with a number between 1 and 100: 1 means the object is definitely not in the picture, 100 means the object is definitely in the picture. Use numbers between 1 and 100 to indicate your level of confidence.Example: If you are 50% confident the object is present, respond with 50, If you are 90% confident the object is present, respond with 90. The response can ONLY be numbers, no text is allowed.");

            chat.AppendUserInput($"Does {prompt} appear in the image?", ImageInput.FromImageBytes(picture));
            var response = await chat.GetResponseFromChatbotAsync();

            return response;
        }
    }
}
