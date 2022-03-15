using System;
using UnityEngine;

namespace UniRx
{
    public class MessageSample : MonoBehaviour
    {
        // 残り時間
        [SerializeField] private float _countTimeSeconds = 30f;
        
        // 時間切れを通知するObservable
        public IObservable<Unit> OnTimeUpAsyncSubject => _onTimeUpAsyncSubject;
        
        // AsyncSubject(メッセージを一度だけ発行できるSubject)
        private readonly AsyncSubject<Unit> _onTimeUpAsyncSubject = new AsyncSubject<Unit>();
        private IDisposable _disposable;

        private void Start()
        {
            // 指定時間経過したらメッセージを通知する
            _disposable = Observable.Timer(TimeSpan.FromSeconds(_countTimeSeconds)).Subscribe(_ =>
            {
                // Timerが発火したら、Unit型のメッセージを発行する
                _onTimeUpAsyncSubject.OnNext(Unit.Default);
                _onTimeUpAsyncSubject.OnCompleted();
                Debug.Log(_);
            });
        }

        private void OnDestroy()
        {
            // Observableがまだ動いてたら止める
            _disposable?.Dispose();
            
            // AysncSubjectを破棄
            _onTimeUpAsyncSubject.Dispose();
        }
    }
}
