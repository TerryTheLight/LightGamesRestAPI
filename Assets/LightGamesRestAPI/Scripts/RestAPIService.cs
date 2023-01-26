namespace LightGames.RestAPI
{
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json;
    using System;
    using UnityEngine;
    using UnityEngine.Networking;
    using LightGames.Utility;

    public class RestAPIService
    {
        private const string _defaultErrorMessage = "Something went wrong. Please try again!";
        private const string _contentType = "Content-Type";
        private const string _applicationJson = "application/json";
        private const int _defaultTimeout = 30;

        private readonly string _baseURL = "";     //The base URL that is required to be replaced here

        private readonly HttpResponseBase _jsonInvalidResponse = new HttpResponseBase()
        {
            HttpStatusCode = 400,
            Message = "Invalid JSON format",
            UserMessage = $"{_defaultErrorMessage} (Json Format Invalid)"
        };

        public RestAPIService(string baseURL)
        {
            _baseURL = baseURL;
        }

        public async UniTask<TResponseDto> SendPostRequestAsync<TResponseDto, TRequest>(TRequest requestData)
            where TRequest : HttpRequestBase, new()
            where TResponseDto : HttpResponseBase, new()
        {
            var httpRequestDefinition = GetHttpRequestDefinitionAttribute<TRequest, TResponseDto>();

            if (string.IsNullOrEmpty(_baseURL))
            {
                Debug.LogError("Base URL for API Call cannot be null/empty");
                return default;
            }

            string endpoint = httpRequestDefinition.Route;

            if (string.IsNullOrEmpty(endpoint))
            {
                Debug.LogError("Endpoint for API Call cannot be null/empty");
                return default;
            }

            string fullPath = GetFullApiPath(_baseURL, endpoint);

            Debug.Log($"[{nameof(RestAPIService)}] SendWebRequest | requestMethod: POST | string: {fullPath} | param: {JsonConvert.SerializeObject(requestData)}");

            using (UnityWebRequest webRequest = UnityWebRequest.Post(fullPath, new WWWForm()))
            {
                string paramjson = requestData.AsJson();
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(paramjson);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader(_contentType, _applicationJson);

                webRequest.timeout = _defaultTimeout;

                await webRequest.SendWebRequest();

                return ProcessWebRequest<TResponseDto>(webRequest);
            }
        }

        private TResponseDto ProcessWebRequest<TResponseDto>(UnityWebRequest webRequest)
            where TResponseDto : HttpResponseBase, new()
        {
            string downloadedText = webRequest.downloadHandler.text;

            // If web request indicating failed..
            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                return GetFailedResponse<TResponseDto>(webRequest, downloadedText);
            }

            // If response is not in json format..
            if (!JsonUtilities.IsValidJson(downloadedText))
            {
                return ((TResponseDto)_jsonInvalidResponse);
            }

            Debug.Log($"[{nameof(RestAPIService)}] ProcessWebRequest | RequestPath: {webRequest.uri.AbsolutePath} | RawResponse: {downloadedText}");

            return ProcessSuccessResponse<TResponseDto>(downloadedText);
        }

        private TResponseDto GetFailedResponse<TResponseDto>(UnityWebRequest webRequest, string downloadedText)
            where TResponseDto : HttpResponseBase, new()
        {
            Debug.LogError($"[{nameof(RestAPIService)}] ProcessFailedResponse | RequestPath: {webRequest.uri.AbsolutePath}");

            // If received nothing from response..
            if (downloadedText.Length == 0)
            {
                return new TResponseDto()
                {
                    HttpStatusCode = (int)webRequest.responseCode,
                    Message = webRequest.error,
                    UserMessage = $"{_defaultErrorMessage} ({webRequest.responseCode})",
                };
            }

            Debug.LogError($"[{nameof(RestAPIService)}] ProcessFailedResponse | Response: {downloadedText}");

            // If the response is not in json format..
            if (!JsonUtilities.IsValidJson(downloadedText))
            {
                return (TResponseDto)_jsonInvalidResponse;
            }

            HttpResponseBase baseErrorResponse = DeSerializeApiJsonResponse(downloadedText);
            return new TResponseDto()
            {
                HttpStatusCode = baseErrorResponse.HttpStatusCode,
                Message = baseErrorResponse.Message,
                UserMessage = baseErrorResponse.UserMessage
            };
        }

        private TResponseDto ProcessSuccessResponse<TResponseDto>(string jsonText)
            where TResponseDto : HttpResponseBase, new()
        {
            TResponseDto response = DeSerializeApiJsonResponse<TResponseDto>(jsonText);

            // the callback invocation has to be done outside of try catch, or else
            // the error caused by scripts outside this method (by onApiCalledAction)
            // will be caught back at this try catch block, making it harder to follow
            // the stack trace.
            return ((TResponseDto)response);
        }

        private HttpResponseBase DeSerializeApiJsonResponse(string jsonText)
        {
            HttpResponseBase response;

            try
            {
                // Able to use JsonUtility because the based ResponseBase class does not have
                // fields that are not serializable without the use of reflection
                response = JsonConvert.DeserializeObject<HttpResponseBase>(jsonText);
            }
            catch (JsonException e)
            {
                Debug.LogException(e);

                response = new HttpResponseBase()
                {
                    HttpStatusCode = 400,
                    Message = "Invalid JSON format",
                    UserMessage = $"{_defaultErrorMessage} (Invalid JSON format)"
                };
            }
            catch (Exception e)
            {
                Debug.LogException(e);

                response = new HttpResponseBase()
                {
                    HttpStatusCode = RestApiStatusType.GenericError,
                    Message = e.Message,
                    UserMessage = $"{_defaultErrorMessage} ({RestApiStatusType.GenericError})"
                };
            }

            return response;
        }

        private TResponseDto DeSerializeApiJsonResponse<TResponseDto>(string jsonText)
            where TResponseDto : HttpResponseBase, new()
        {
            TResponseDto response;

            try
            {
                // Able to use JsonUtility because the based ResponseBase class does not have
                // fields that are not serializable without the use of reflection
                response = JsonConvert.DeserializeObject<TResponseDto>(jsonText);
            }
            catch (JsonException e)
            {
                Debug.LogException(e);

                response = new TResponseDto()
                {
                    HttpStatusCode = RestApiStatusType.LocalCustomStatus.JsonFormatInvalid,
                    Message = "Invalid JSON format",
                    UserMessage = $"{_defaultErrorMessage} ({RestApiStatusType.LocalCustomStatus.JsonFormatInvalid})"
                };
            }
            catch (Exception e)
            {
                Debug.LogException(e);

                response = new TResponseDto()
                {
                    HttpStatusCode = RestApiStatusType.GenericError,
                    Message = e.Message,
                    UserMessage = $"{_defaultErrorMessage} ({RestApiStatusType.GenericError})"
                };
            }

            return response;
        }

        private string GetFullApiPath(string baseUrl, string endpoint)
        {
            string apiUriPath;
            if (baseUrl[^1] == '/')
            {
                if (endpoint[0] != '/')
                    apiUriPath = baseUrl + endpoint;
                else
                    apiUriPath = baseUrl.Remove(baseUrl.Length - 1) + endpoint;
            }
            else
            {
                if (endpoint[0] == '/')
                    apiUriPath = baseUrl + endpoint;
                else
                    apiUriPath = baseUrl + '/' + endpoint;
            }

            return apiUriPath;
        }

        private HttpRequestDefinitionAttribute GetHttpRequestDefinitionAttribute<TPostRequest, TResponseDto>()
        {
            if (Attribute.GetCustomAttribute(typeof(TResponseDto), typeof(HttpRequestDefinitionAttribute)) is not HttpRequestDefinitionAttribute httpRequestDefinition)
            {
                throw new Exception($"request {typeof(TResponseDto)} wasn't defined yet!!! Please add HttpRequestDefinitionAttribute for it!!!!");
            }

            return httpRequestDefinition;
        }
    }
}

