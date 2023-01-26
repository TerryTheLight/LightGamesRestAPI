using Cysharp.Threading.Tasks;
using LightGames.RestAPI;
using UnityEngine.Events;
using LightGames.Utility;

public class TestAPIManager : Singleton<TestAPIManager>
{
    private const string _baseApiURL = "INSERT_YOUR_OWN_BASE_API_URL_HERE"; //Eg: https://www.google.com/
    private RestAPIService _restAPIService;

    protected override void Awake()
    {
        base.Awake();
        _restAPIService = new RestAPIService(_baseApiURL);
    }

    public async UniTask<TResponseDto> ExecutePostAPI<TRequestData, TResponseDto>(TRequestData request,
        UnityAction<TResponseDto> onSuccess, UnityAction<TResponseDto> onFail)
        where TResponseDto : HttpResponseBase, new()
        where TRequestData : HttpRequestBase, new()
    {
        var result = await _restAPIService.SendPostRequestAsync<TResponseDto, TRequestData>(request);

        if (result.IsSuccess)
        {
            onSuccess?.Invoke(result);
        }
        else
        {
            onFail?.Invoke(result);
        }

        return (TResponseDto)result;
    }
}
