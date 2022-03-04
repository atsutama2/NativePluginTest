using System;
using System.Threading;
using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UniRx
{
    public class DownloadTextureUniTask : MonoBehaviour
    {
        // uGUIのRowImage
        [FormerlySerializedAs("_rawImage")] [SerializeField]
        private RawImage rawImage;

        private void Start()
        {
            // このGameObjectに紐付いたCansellationTokenを取得
            var token = this.GetCancellationTokenOnDestroy();
            // テクスチャのセットアップ実行
            SetupTextureAsync(token).Forget();
        }

        private async UniTaskVoid SetupTextureAsync(CancellationToken token)
        {
            try
            {
                const string uri = "https://www.sandabiyori.com/wp-content/uploads/2021/06/sanda-neko-002.jpg";
                // UniRxのRetryを使いたいので、UniTaskからObservableへ変換する
                var observable = Observable.Defer(() => GetTextureAsync(uri, token).ToObservable()).Retry(3);
                
                // Observbaleもawaitで待ち受けが可能
                var texture = await observable;
                rawImage.texture = texture;
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Debug.LogError(e);
            }
        }

        // コルーチンの代わりにasync/awaitを利用する
        private async UniTask<Texture> GetTextureAsync(string uri, CancellationToken token)
        {
            using var uwr = UnityWebRequestTexture.GetTexture(uri);
            await uwr.SendWebRequest().WithCancellation(token);
            return ((DownloadHandlerTexture) uwr.downloadHandler).texture;
        }
    }
}