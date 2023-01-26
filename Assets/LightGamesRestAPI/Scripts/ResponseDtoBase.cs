namespace LightGames.RestAPI
{
    public class ResponseDtoBase
    {
        public int HttpStatusCode;
        public string Message;
        public string UserMessage;

        public const int SuccessStatus = 200;
    }
}


