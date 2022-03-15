using System;
using UnityEngine;

namespace UniRx
{
    public class AddToSample : MonoBehaviour
    {
        private void Start()
        {
            // 5フレームごとにメッセージを発行するObservable
            // このGameObjectのOnDestroyに連動して自動でDispose()させる
            Observable.IntervalFrame(5).Subscribe(_ => Debug.Log("Do!")).AddTo(this);
        }
    }
}
