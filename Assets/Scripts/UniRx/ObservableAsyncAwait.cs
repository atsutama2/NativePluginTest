using System;
using System.Threading.Tasks;
using UnityEngine;

namespace UniRx
{
    public class ObservableAsyncAwait : MonoBehaviour
    {
        private void Start()
        {
            // _ にTaskを代入するとawaitしていない時のコンパイラ警告を抑制できる
            _ = DoAsync();
        }

        private static async Task DoAsync()
        {
            // Taskが完了するまで待機
            await Task.Delay(TimeSpan.FromSeconds(1));

            // ObservableがOnCompletedするまで待機
            await Observable.Timer(TimeSpan.FromSeconds(1));

            // 無限に続くObservableを対象とする場合はoperatorを用いて終了条件を追加する必要がある
            await Observable.Interval(TimeSpan.FromSeconds(1)).Take(3);

            var result = await Observable.Return("Result!");
            Debug.Log(result);

            try
            {
                await Observable.Throw<Unit>(new Exception("Throw exception!"));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
    }
}
