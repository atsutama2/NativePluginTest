using System;
using System.Collections;
using UnityEngine;

namespace UniRx.AsyncSubject
{
    public class GameResourceProvider : MonoBehaviour
    {
        // プレイヤーのテクスチャ情報を扱うAsyncSubject
        private readonly AsyncSubject<Texture> _playerTextureAsyncSubject = new AsyncSubject<Texture>();
        
        // プレイヤのテクスチャ情報
        public IObservable<Texture> PlayerTextureAsync => _playerTextureAsyncSubject;

        private void Start()
        {
            // 起動時にテクスチャをロードする
            StartCoroutine(LoadTexture());
        }

        private IEnumerator LoadTexture()
        {
            // プレイヤのテクスチャ情報を非同期で読み込み
            var resource = Resources.LoadAsync<Texture>("Textures/sampleTexture");
            yield return resource;
            
            // 読み込みが完了したらAsyncSubjectで結果を通知する
            _playerTextureAsyncSubject.OnNext(resource.asset as Texture);
            _playerTextureAsyncSubject.OnCompleted();
        }
    }
}
