namespace OllamaChat.Factories
{
    public class ChatFactory : IChatFactory
    {
        public ChatFactory() { }

        public async Task<string> Chat(string prompt)
        {
            return "";
        }
    }
}
