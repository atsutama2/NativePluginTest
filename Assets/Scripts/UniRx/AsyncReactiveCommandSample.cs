using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UniRx
{
    public class AsyncReactiveCommandSample : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Text _resultText;

        private void Start()
        {
            // AsyncReactiveCommand<Unit>型と同義
            var asyncReactiveCommand = new AsyncReactiveCommand();

            // ボタンを押したらサーバと通信
            // 通信中はボタンを押せない(通信中はButtonのInsteractiveがfalseになる)
            asyncReactiveCommand.BindToOnClick(_button, _ =>
            {
                return FetchAsync("https://zipcloud.ibsnet.co.jp/api/search?zipcode=7830060")
                    .ToObservable()
                    .ForEachAsync(x =>
                    {
                        _resultText.text = x;
                    });
            });
        }
        
        // UniTaskを使ってサーバ通信
        private async UniTask<string> FetchAsync(string uri)
        {
            using (var uwr = UnityWebRequest.Get(uri))
            {
                await uwr.SendWebRequest();
                return uwr.downloadHandler.text;
            }
        }
    }
}
