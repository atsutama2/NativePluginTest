using System;
using UnityEngine;

namespace UniRx
{
    public class BehaviorSubjectSample : MonoBehaviour
    {
        private void Start()
        {
            // 定義には初期値が必要
            var behaviorSubject = new BehaviorSubject<int>(0);
            
            behaviorSubject.OnNext(1);
            
            // Valueを参照すると最新の値を確認できる
            Debug.Log("Value: " + behaviorSubject.Value);

            behaviorSubject.Subscribe(
                x => Debug.Log("OnNext: " + x),
                ex => Debug.LogError("OnError: " + ex),
                () => Debug.Log("OnCompleted!"));
            
            behaviorSubject.OnNext(2);
            
            // Valueを参照すると最新の値を確認できる
            Debug.Log("Last Value: " + behaviorSubject.Value);
            
            behaviorSubject.OnNext(3);
            behaviorSubject.OnCompleted();
            
            // キャッシュを完全に削除
            behaviorSubject.Dispose();
        }
    }
}
