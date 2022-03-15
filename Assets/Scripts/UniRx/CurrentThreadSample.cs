using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UniRx
{
    public class CurrentThreadSample : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("Unity Main Thread ID: " + Thread.CurrentThread.ManagedThreadId);

            var subject = new Subject<Unit>();
            subject.AddTo(this);

            subject.ObserveOn(Scheduler.Immediate).Subscribe(_ =>
            {
                Debug.Log("Thread Id: " + Thread.CurrentThread.ManagedThreadId);
            });
            
            // メインスレッドにてOnNext発行
            subject.OnNext(Unit.Default);
            
            // 別スレッドからOnNextを発行
            Task.Run(() => subject.OnNext(Unit.Default));
            subject.OnCompleted();
        }
    }
}
