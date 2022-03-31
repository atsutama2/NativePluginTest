using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UniTaskDer
{
    // 結果を返すコルーチンの代替
    public class ReadTextureSample : MonoBehaviour
    {
        [SerializeField] private RawImage _rawImage;

        private const string ResourcePath = "https://reach-rh.com/wp-content/uploads/2017/08/7f707696e04dafe3359796ecc28196d3.jpg";
        
        private void Start()
        {
            var cancellationToken = this.GetCancellationTokenOnDestroy();
            InitializeAsync(ResourcePath, cancellationToken).Forget();
        }
        
        // テクスチャを非同期で読み込み、RowImageに設定する
        private async UniTaskVoid InitializeAsync(string uri, CancellationToken token)
        {
            try
            {
                var texture = await GetTextureAsync(uri, token);
                _rawImage.texture = texture;
            }
            catch (OperationCanceledException e)
            {
                // キャンセルの場合は何もしない
                Debug.LogError("キャンセルの場合は何もしない");
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        // テクスチャを取得する
        private static async UniTask<Texture> GetTextureAsync(string uri, CancellationToken token)
        {
            using var uwr = UnityWebRequestTexture.GetTexture(uri);
            await uwr.SendWebRequest().WithCancellation(token);
            return DownloadHandlerTexture.GetContent(uwr);
        }
    }
}
