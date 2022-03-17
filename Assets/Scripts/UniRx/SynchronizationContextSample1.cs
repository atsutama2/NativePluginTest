using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UniRx
{
    public class SynchronizationContextSample1 : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log("MainThread ID: " + GetCurrentThreadId());
            
            DoAsync();
        }

        private async Task DoAsync()
        {
            Debug.Log("await前: " + GetCurrentThreadId());

            await Task.Run(() =>
            {
                Debug.Log("Task.Run: " + GetCurrentThreadId());
            });
            
            Debug.Log("await後: " + GetCurrentThreadId());
        }

        private int GetCurrentThreadId()
        {
            return Thread.CurrentThread.ManagedThreadId;
        }
    }
}
