using System;
using System.Collections;
using UnityEngine;

namespace UniRx.ReactiveProperty
{
    public class ReactivePropertyTimer : MonoBehaviour
    {
        // 実体を定義
        [SerializeField] private IntReactiveProperty _current = new IntReactiveProperty(60);
        
        // 現在のタイマの値(読み取り専用)
        // ReactivePropertyをIReadOnlyReactivePropertyにアップキャスト
        public IReadOnlyReactiveProperty<int> CurrentTime => _current;

        private void Start()
        {
            StartCoroutine(CountDownCoroutine());
            _current.AddTo(this);
        }

        private IEnumerator CountDownCoroutine()
        {
            while (_current.Value > 0)
            {
                _current.Value--;
                yield return new WaitForSeconds(1);
            }
        }
    }
}
