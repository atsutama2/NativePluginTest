using System;
using UnityEngine;

namespace UniRx
{
    public class HotOperatorSample : MonoBehaviour
    {
        private void Start()
        {
            var originalSubject = new Subject<string>();
            
            // OnNextの内容をスペースで区切りで連結し、最後の1つだけを出力するObservable
            // IConnectableObservable<string>になっている
            // Publish()はHot変換するoperator
            IConnectableObservable<string> appendStringObservable =
                originalSubject.Scan((previous, current) => previous + " " + current).Last().Publish();
            
            // IConnectableObservable.Connect()を呼び出すと内部でSubscribeの実行が走る
            var disposable = appendStringObservable.Connect();
            
            originalSubject.OnNext("I");
            originalSubject.OnNext("have");
            
            // appendStringObservableを直接Subscribeすればよい
            appendStringObservable.Subscribe(Debug.Log);
            
            originalSubject.OnNext("a");
            originalSubject.OnNext("pen.");
            originalSubject.OnCompleted();
            
            // Hot変換解除
            disposable.Dispose();
            originalSubject.Dispose();
        }
    }
}
