namespace OllamaChat.Factories
{
    public interface IChatFactory
    {
        public Task<string> Chat(string prompt);
    }
}