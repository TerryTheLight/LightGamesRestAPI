public static class RestApiStatusType
{
    public static class LocalCustomStatus
    {
        public const int ResponseIsNull = 498;
        public const int JsonFormatInvalid = 499;
    }

    public const int Successful = 200;
    public const int GenericError = 400;
    public const int SessionNotFound = 402;
}
