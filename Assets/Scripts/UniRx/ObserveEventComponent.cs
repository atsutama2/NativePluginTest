using System;
using UnityEngine;

namespace UniRx
{
    public class ObserveEventComponent : MonoBehaviour
    {
        [SerializeField] private CountDownEventProvider _countDownEventProvider;
        
        // Observerのinstance
        private PrintLogObserver<int> _printLogObserver;

        private IDisposable _disposable;

        private void Start()
        {
            // PrintLogObserverインスタンスを作成
            _printLogObserver = new PrintLogObserver<int>();
            
            // SubjectのSubscribeを呼び出して、observerを登録する
            // _disposable = _countDownEventProvider.CountDownObservable.Subscribe(_printLogObserver);
            
            // 簡易版Subscribe()
            _disposable = _countDownEventProvider.CountDownObservable.Subscribe(
                x => Debug.Log(x), //OnNext
                Debug.LogError, //OnError
                () => Debug.Log("OnCompleted!"));
        }

        private void OnDestroy()
        {
            // GameObject破棄時にイベント購読を中断する
            _disposable?.Dispose();
        }
    }
}
