using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UniRx
{
    public class SelectMany : MonoBehaviour
    {
        [SerializeField] private Button _downloadButton;
        [SerializeField] private InputField _uriInputField;

        private void Start()
        {
            _downloadButton.OnClickAsObservable()
                .Select(_ => _uriInputField.text)
                .SelectMany(uri => FetchAsync(uri).ToObservable())
                .Subscribe(Debug.Log);
        }

        private static async UniTask<string> FetchAsync(string uri)
        {
            using (var uwr = UnityWebRequest.Get(uri))
            {
                await uwr.SendWebRequest();
                return uwr.downloadHandler.text;
            }
        }
    }
}
