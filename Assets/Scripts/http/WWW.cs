using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class WWW : MonoBehaviour
{
    private UnityWebRequest uwr;

    public WWW(string url)
    {
        this.uwr = UnityWebRequest.Get(url);
    }

    void Start()
    {
        WWW www = new WWW("https://www.jma.go.jp/bosai/forecast/data/overview_forecast/130000.json");
        www.GET();
    }

    public async void GET()
    {
        string str = await GetTextAsync(this.uwr);
    }

    async UniTask<string> GetTextAsync(UnityWebRequest req)
    {
        var op = await req.SendWebRequest();
        if (req.isNetworkError || req.isHttpError) {
            Debug.Log(string.Format("[Error]: {0}", req.error));
            return req.error;
        } else {
            Debug.Log(string.Format("[Success]: {0}", req.downloadHandler.text));
            return op.downloadHandler.text;
        }
    }
}
