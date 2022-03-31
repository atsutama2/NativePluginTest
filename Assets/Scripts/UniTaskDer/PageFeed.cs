using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UniTaskDer
{
    // イベントをasync/awaitで扱う
    // ページ送り
    // 複数枚設定したテクスチャをボタンがクリックされるたびに順番にRawImageに表示していく
    public class PageFeed : MonoBehaviour
    {
        [SerializeField] private Texture[] _textures;
        [SerializeField] private Button    _button;
        [SerializeField] private RawImage _rawImage;

        private void Start()
        {
            var token = this.GetCancellationTokenOnDestroy();
            InitializeAsync(token).Forget();
            Debug.Log("Start完了");
        }

        private async UniTaskVoid InitializeAsync(CancellationToken token)
        {
            // 5秒待つ
            await HogeAsync();
            // 8秒待つ
            await HogeHogeAsync();
            await PageFeedAsync(token);
            Destroy(gameObject);
        }

        private async UniTask PageFeedAsync(CancellationToken token)
        {
            // uGUI ButtonのクリックイベントのAsyncHandlerを取得
            using (var clickEventHandler = _button.GetAsyncClickEventHandler(token))
            {
                foreach (var t in _textures)
                {
                    _rawImage.texture = t;
                    await clickEventHandler.OnClickAsync(); // クリック待ち
                }
            }
        }

        static async Task HogeAsync()
        {
            Debug.Log("①待つ！");
            await Task.Delay(5000);
            Debug.Log("5秒経った!");
        }
        
        static async Task HogeHogeAsync()
        {
            Debug.Log("②待つ！");
            await Task.Delay(8000);
            Debug.Log("8秒経った!");
        }
    }
}
