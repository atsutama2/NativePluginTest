using System;
using UnityEngine;
using UnityEngine.UI;

namespace UniRx.AsyncSubject
{
    // プレイヤのテクスチャ変更
    public class PlayerTextureChanger : MonoBehaviour
    {
        [SerializeField] private RawImage _rawImage;
        [SerializeField] private GameResourceProvider _gameResourceProvider;

        private void Start()
        {
            // プレイヤのテクスチャの読み込みが完了次第テクスチャを変更する
            _gameResourceProvider.PlayerTextureAsync.Subscribe(SetTexture).AddTo(this);
        }

        private void SetTexture(Texture newTexture)
        {
            var r = GetComponent<RawImage>();
            _rawImage.texture = newTexture;
        }
    }
}
