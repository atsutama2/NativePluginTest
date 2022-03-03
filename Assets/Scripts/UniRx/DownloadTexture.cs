using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UniRx
{
    public class DownloadTexture : MonoBehaviour
    {
        // uGUIのRowImage
        [FormerlySerializedAs("_rawImage")] 
        [SerializeField] private RawImage rawImage;

        private void Start()
        {
            const string uri = "https://president.ismcdn.jp/mwimgs/f/f/1420wm/img_ffc84426d80a248cfab019a6edf492891364594.jpg";

            // テクスチャを取得する
            // ただし例外発生時は計3回まで施行する
            GetTextureAsync(uri).OnErrorRetry(
                onError: (Exception _) => { },
                retryCount: 3
            ).Subscribe(
                result => { rawImage.texture = result; },
                Debug.LogError
            ).AddTo(this);
        }
        
        // コルーチンを起動して、その結果をObservableで返す
        private IObservable<Texture> GetTextureAsync(string uri)
        {
            return Observable.FromCoroutine<Texture>(observer => GetTextureCoroutine(observer, uri));
        }
        
        // コルーチンでテクスチャをDownloadする
        private IEnumerator GetTextureCoroutine(IObserver<Texture> observer, string uri)
        {
            using var uwr = UnityWebRequestTexture.GetTexture(uri);
            yield return uwr.SendWebRequest();
#pragma warning disable CS0618
            if (uwr.isNetworkError || uwr.isHttpError)
#pragma warning restore CS0618
            {
                // Errorが起きたらOnErrorメッセージを発行する
                observer.OnError(new Exception(uwr.error));
                yield break;
            }

            var result = ((DownloadHandlerTexture) uwr.downloadHandler).texture;
            // 成功したらOnNext/OnComplatedメッセージを発行する
            observer.OnNext(result);
            observer.OnCompleted();
        }
    }
}
