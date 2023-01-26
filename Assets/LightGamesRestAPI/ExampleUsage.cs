using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleUsage : MonoBehaviour
{
    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            var loginRequestData = new TestLoginHttpRequest()
            {
                email = "helloworld@gmail.com",
                password = "12345",
            };

            var result = await TestAPIManager.instance.ExecutePostAPI<TestLoginHttpRequest, TestLoginHttpResponse>(loginRequestData, (success) => { }, (fail) => { });
            Debug.Log($"Result = {result.AccessToken.access_token}");
        }
    }
}
