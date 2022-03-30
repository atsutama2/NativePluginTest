using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UniTaskDer
{
    public class UniTaskSampleCoroutine : MonoBehaviour
    {
        [SerializeField] private GameObject obj;
        
        private void Start()
        {
            var cancellationToken = this.GetCancellationTokenOnDestroy();

            // 特定のパターンでオブジェクトを移動させる
            MovePatternAsync(cancellationToken).Forget();
        }
        
        // 特定パターンで移動する
        private async UniTaskVoid MovePatternAsync(CancellationToken token)
        {
            await MoveAsync(Vector3.up * 2.0f, 1.0f, token);
            await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: token);
            
            await MoveAsync(Vector3.down * 1.0f, 3.0f, token);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: token);
            
            await MoveAsync(Vector3.right * 4.0f, 2.0f, token);
            await UniTask.Delay(TimeSpan.FromSeconds(2.0f), cancellationToken: token);
        }

        // 指定した速度で指定秒数移動する
        private async UniTask MoveAsync(Vector3 velocity, float seconds, CancellationToken token)
        {
            var startTime = Time.time;
            while (Time.time - startTime < seconds)
            {
                obj.transform.position += velocity * Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
    }
}
