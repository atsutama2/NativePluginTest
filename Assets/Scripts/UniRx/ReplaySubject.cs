using System;
using UnityEngine;

namespace UniRx
{
    public class ReplaySubject : MonoBehaviour
    {
        private void Start()
        {
            var subject = new ReplaySubject<int>(bufferSize: 5);

            for (var i = 0; i < 10; i++)
            {
                subject.OnNext(i);
            }
            
            subject.OnCompleted();

            subject.Subscribe(
                x => Debug.Log("OnNext: " + x),
                ex => Debug.LogError("OnError: " + ex),
                () => Debug.Log("OnCompleted"));
            
            subject.Dispose();
        }
    }
}
