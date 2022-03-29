using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UniTask
{
    public class CoroutineAwaiterSample : MonoBehaviour
    {
        [SerializeField] private GameObject cube;
        
        private async UniTaskVoid Start()
        // private async void Start()
        {
            // コルーチンにawaitを付けるだけで自動的にコルーチンが起動して待ち受けができる
            // 前→右→後と順番に移動する
            await MoveCoroutine(Vector3.forward * 1.0f, 2);
            await MoveCoroutine(Vector3.right * 2.0f, 1);
            await MoveCoroutine(Vector3.back * 2.0f, 1);

            var cancellationToken = this.GetCancellationTokenOnDestroy();

            // cancellationTokenを指定する場合
            await MoveCoroutine(Vector3.down * 3.0f, 1).WithCancellation(cancellationToken);
        }

        // 指定した速度で、指定した秒数移動するコルーチン
        private IEnumerator MoveCoroutine(Vector3 velocity, float seconds)
        {
            var start = Time.time;
            while ((Time.time - start) < seconds)
            {
                cube.transform.position += velocity * Time.deltaTime;
                yield return null;
            }
        }
    }
}
