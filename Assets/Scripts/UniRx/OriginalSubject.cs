using System;
using System.Collections.Generic;

namespace UniRx
{
    // 独自でSubjectを実装
    public sealed class OriginalSubject<T> : ISubject<T>, IDisposable
    {
        public bool IsStopped { get; } = false;
        public bool IsDisposed { get; } = false;

        private readonly object _lockObject = new object();
        
        // 途中で発生した例外
        private Exception _error;
        
        // 自身を購読しているObserverリスト
        private List<IObserver<T>> _observers;

        public OriginalSubject()
        {
            _observers = new List<IObserver<T>>();
        }

        // IOserver.OnNextの実装
        public void OnNext(T value)
        {
            if (IsStopped) return;
            lock (_lockObject)
            {
                ThrowIfDisposed();
                
                // 自身を行動しているObserver全員へメッセージをばらまく
                foreach (var observer in _observers)
                {
                    observer.OnNext(value);
                }
            }
        }
        
        // IObserver.OnErrorの実装
        public void OnError(Exception error)
        {
            lock (_lockObject)
            {
                ThrowIfDisposed();
                if(IsStopped) return;
                this._error = error;

                try
                {
                    foreach (var observer in _observers)
                    {
                        observer.OnError(error);
                    }
                }
                finally
                {
                    Dispose();
                }
            }
        }
        
        // IObserver.OnCompletedの実装
        public void OnCompleted()
        {
            lock (_lockObject)
            {
                ThrowIfDisposed();
                if(IsStopped) return;

                try
                {
                    foreach (var observer in _observers)
                    {
                        observer.OnCompleted();
                    }
                }
                finally
                {
                    Dispose();
                }
            }
        }
        
        // IObservable.Subscribeの実装
        public IDisposable Subscribe(IObserver<T> observer)
        {
            lock (_lockObject)
            {
                if (IsStopped)
                {
                    // すでに動作を終了しているならOnErrorメッセージ
                    // またはOnCompletedメッセージを発行する
                    if (_error != null)
                    {
                        observer.OnError(_error);
                    }
                    else
                    {
                        observer.OnCompleted();
                    }

                    return Disposable.Empty;
                }
            }
            
            _observers.Add(observer); // リスト追加
            return new Subscription(this, observer);
        }

        private void ThrowIfDisposed()
        {
            if (IsDisposed) throw new ObjectDisposedException("OriginalSubject");
        }
        
        // SubscribeのDisposeを実現するために用いるinner class
        private sealed class Subscription : IDisposable
        {
            private readonly IObserver<T> _observer;
            private readonly OriginalSubject<T> _parent;

            public Subscription(OriginalSubject<T> parent, IObserver<T> observer)
            {
                this._parent = parent;
                this._observer = observer;
            }

            public void Dispose()
            {
                // DisposeされたらObserverリストから消去する
                _parent._observers.Remove(_observer);
            }
        }

        public void Dispose()
        {
            lock (_lockObject)
            {
                if (IsDisposed) return;
                _observers.Clear();
                _observers = null;
                _error = null;
            }
        }
    }
}
