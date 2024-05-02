using OpenAI_API;
using OpenAI_API.Models;
using static OpenAI_API.Chat.ChatMessage;

namespace CC_Backend.Services
{
    public interface IOpenAIService
    {
        Task<string> ReadImage(string prompt);
    }
   
}
