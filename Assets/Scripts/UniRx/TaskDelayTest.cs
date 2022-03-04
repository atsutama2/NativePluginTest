using System;
using System.Threading.Tasks;
using UnityEngine;

namespace UniRx
{
    public class TaskDelayTest : MonoBehaviour
    {
        private void Start()
        {
            //引数で指定されたミリ秒待機
            //AsyncTest();
            
            // 並列処理
            //AsyncSample1();
            //AsyncSample2();
            
            // 直接処理
            //AsyncSample1Task().ContinueWith(_ => AsyncSample2Task());
            
            // 指定された Task が全て完了してから Task を実行
            //TaskWhenAllComplate();
            
            // 戻り値が欲しい場合は Task<T> を使用
            ExecuteTaskString();
        }
        
        /// <summary>
        /// 引数で指定されたミリ秒待機
        /// </summary>
        private static async void AsyncTest()
        {
            Debug.Log("AsyncTest Start!");
            await Task.Delay(1000);
            Debug.Log("AsyncTest End!");
        }
        
        /// <summary>
        /// 並列処理の場合
        /// </summary>
        private static async void AsyncSample1()
        {
            Debug.Log("AsyncSample1 Start.");
            await Task.Delay(1000);
            Debug.Log("AsyncSample1 End.");
        }
        async void AsyncSample2()
        {
            Debug.Log("AsyncSample2 Start.");
            await Task.Delay(1000);
            Debug.Log("AsyncSample2 End.");
        }
        
        /// <summary>
        /// 直接処理の場合
        /// </summary>
        private static async Task AsyncSample1Task()
        {
            Debug.Log("AsyncSample1Task Start.");
            await Task.Delay(1000);
            Debug.Log("AsyncSample1Task End.");
        }
        async Task AsyncSample2Task()
        {
            Debug.Log("AsyncSample2Task Start.");
            await Task.Delay(1000);
            Debug.Log("AsyncSample2Task End.");
        }
        
        /// <summary>
        /// 指定された Task が全て完了してから Task を実行
        /// </summary>
        async void TaskWhenAllComplate()
        {
            Debug.Log("==========TaskWhenAllComplate Start==========");
            await Task.WhenAll(AsyncSample1Task(), AsyncSample2Task());
            Debug.Log("==========TaskWhenAllComplate End==========");
        }
        
        /// <summary>
        /// 戻り値が欲しい場合は Task<T> を使います。
        /// </summary>
        /// <returns></returns>
        async Task<String> AsyncSampleTaskString()
        {
            Debug.Log("AsyncSampleTaskString Start!");
            await Task.Delay(2000);
            return "AsyncSampleTaskString End!";
        }

        async void ExecuteTaskString()
        {
            var str = await AsyncSampleTaskString();
            Debug.Log(str);
        }
    }
}
