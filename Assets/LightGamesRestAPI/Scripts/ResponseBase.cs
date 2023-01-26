namespace LightGames.RestAPI
{
    // PS: Doesn't need System.Serializable, as both Unity's JsonUtility
    // and Newtonsoft's JsonConvert works without it
    public class HttpResponseBase
    {
        public int HttpStatusCode;
        public string Message;
        public string UserMessage;

        public const int SuccessStatus = 200;
        public const int FailureStatus = 400;

        public bool IsSuccess => HttpStatusCode == SuccessStatus;
    }

}

