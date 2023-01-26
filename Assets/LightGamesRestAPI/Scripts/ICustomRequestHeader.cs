using System.Collections.Generic;

namespace LightGames.RestAPI
{
    public interface ICustomRequestHeader
    {
        Dictionary<string, string> HeaderParamsKeyToValue { get; set; }
    }
}

