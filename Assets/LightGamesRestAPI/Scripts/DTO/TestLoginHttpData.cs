using LightGames.RestAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLoginHttpRequest : HttpRequestBase
{
    public const string Endpoint = "INSERT_YOUR_ENDPOINT_HERE"; //Eg: hello/world

    public string email;
    public string password;
}

[HttpRequestDefinitionAttribute(TestLoginHttpRequest.Endpoint)]
public class TestLoginHttpResponse : HttpResponseBase
{
    public AccessToken AccessToken;
    public string Email;
}

public class AccessToken
{
    public string access_token;
    public string token_type;
    public long expires_in;
}
