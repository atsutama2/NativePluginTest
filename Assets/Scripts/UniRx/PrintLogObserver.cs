using System;
using UnityEngine;


// 受信したメッセージをログに出力するObserver
namespace UniRx
{
    public class PrintLogObserver<T> : IObserver<T>
    {
        public void OnCompleted()
        {
            Debug.Log("OnCompleted!");
        }

        public void OnError(Exception error)
        {
            Debug.Log(error);
        }

        public void OnNext(T value)
        {
            Debug.Log(value);
        }
    }
}
