namespace LightGames.RestAPI
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class HttpRequestDefinitionAttribute : Attribute
    {
        public string Route { get; }
        public int TimeOut { get; }

        public HttpRequestDefinitionAttribute(string route, int timeOut = -1)
        {
            this.Route = route;
            this.TimeOut = timeOut;
        }
    }
}

