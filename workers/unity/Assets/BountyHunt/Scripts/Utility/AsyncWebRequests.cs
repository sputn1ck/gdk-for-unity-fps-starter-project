using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class AwaitRequestText
{
    RequestTextResult result;
    private int maxTries;
    public AwaitRequestText(MonoBehaviour mb, string url, int maxTries = 50)
    {
        this.maxTries = maxTries;
        mb.StartCoroutine(GetRequest(url));
    }

    public async Task<RequestTextResult> GetResult()
    {
        // die tries sind da damit das programm zur not stoppt
        int tries = 0;
        while (result == null && tries < maxTries)
        {
            tries++;
            await Task.Delay(TimeSpan.FromSeconds(0.1));
        }
        if (tries >= maxTries)
        {
            return new RequestTextResult()
            {
                response = null,
                hasError = true,
                error = "exceeded max tries",
            };
        }

        return result;
    }

    IEnumerator GetRequest(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                result = new RequestTextResult
                {
                    response = null,
                    hasError = true,
                    error = webRequest.error
                };
            }
            else
            {
                result = new RequestTextResult
                {
                    hasError = false,
                    response = webRequest.downloadHandler.text
                };
            }
        }

    }
    public static async Task<string> SendAsyncWebRequest(MonoBehaviour mb, string url, int maxTries = 20)
    {
        AwaitRequestText ar = new AwaitRequestText(mb, url, maxTries);
        RequestTextResult rr = await ar.GetResult();
        if (rr.hasError)
        {
            Debug.LogError(rr.error);
        }
        return rr.response;
    }
}

public class AwaitRequestTexture
{
    RequestTextureResult result;
    private int maxTries;
    public AwaitRequestTexture(MonoBehaviour mb, string url, int maxTries = 50)
    {
        mb.StartCoroutine(GetRequest(url));
        this.maxTries = maxTries;
    }

    public async Task<RequestTextureResult> GetResult()
    {
        // die tries sind da damit das programm zur not stoppt
        int tries = 0;
        while (result == null && tries < maxTries)
        {
            tries++;
            await Task.Delay(TimeSpan.FromSeconds(0.1));
        }
        if (tries >= maxTries)
        {
            return new RequestTextureResult()
            {
                response = null,
                hasError = true,
                error = "exceeded max tries",
            };
        }

        return result;
    }

    IEnumerator GetRequest(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                result = new RequestTextureResult
                {
                    response = null,
                    hasError = true,
                    error = webRequest.error
                };
            }
            else
            {
                result = new RequestTextureResult
                {
                    hasError = false,
                    response = DownloadHandlerTexture.GetContent(webRequest)
            };
            }
        }

    }
    public static async Task<Texture2D> SendAsyncWebRequest(MonoBehaviour mb, string url, int maxTries = 20)
    {
        AwaitRequestTexture ar = new AwaitRequestTexture(mb, url, maxTries);
        RequestTextureResult rr = await ar.GetResult();
        if (rr.hasError)
        {
            Debug.LogError(rr.error);
        }
        return rr.response;
    }
}

public class RequestTextResult
{
    public string response;
    public string error;
    public bool hasError;
}

public class RequestTextureResult
{
    public Texture2D response;
    public string error;
    public bool hasError;
}
