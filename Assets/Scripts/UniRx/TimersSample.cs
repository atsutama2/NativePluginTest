using System;
using System.Threading;
using UnityEngine;

namespace UniRx
{
    public class TimersSample : MonoBehaviour
    {
        private void Start()
        {
            // MainThreadを指定
            // 「1秒経過直後に実行されたUpdate()」と同じタイミングに実行される
            Observable.Timer(TimeSpan.FromSeconds(1), Scheduler.MainThread).Subscribe(x => Debug.Log("a: 1秒経過しました")).AddTo(this);
            
            // 未指定の場合はMainThreadScheduler指定と同じになる
            Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(x => Debug.Log("b: 1秒経過しました")).AddTo(this);
            
            // MainThreadEndOfFrame指定とき
            // 「1秒経過直後のフレームのレンダリング後」に実行される
            Observable.Timer(TimeSpan.FromSeconds(1), Scheduler.MainThreadEndOfFrame)
                .Subscribe(x => Debug.Log("c: 1秒経過しました")).AddTo(this);
            
            // CurrentThreadを指定するとそのまま同じスレッドで処理を実行する
            // このコードの場合は新しく作ったスレッド上でタイマのカウントを実行する
            new Thread(() =>
            {
                Observable.Timer(TimeSpan.FromSeconds(1), Scheduler.CurrentThread)
                    .Subscribe(x => Debug.Log("d: 1秒経過しました")).AddTo(this);
            }).Start();
        }
    }
}
