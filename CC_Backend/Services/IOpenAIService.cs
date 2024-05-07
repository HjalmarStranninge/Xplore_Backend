namespace CC_Backend.Services
{
    public interface IOpenAIService
    {
        Task<string> ReadImage(string prompt);
    }
   
}
