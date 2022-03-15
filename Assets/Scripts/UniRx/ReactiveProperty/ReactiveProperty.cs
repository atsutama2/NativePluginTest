using UnityEngine;

namespace UniRx.ReactiveProperty
{
    public class ReactiveProperty : MonoBehaviour
    {
        [SerializeField] private CustomReactiveProperty customReactiveProperty;
        
        private void Start()
        {
            // int型を扱う、初期値100設定
            var health = new ReactiveProperty<int>(100);
            
            // Valueにアクセスすれば現在設定されている値を読み取れる
            Debug.Log("現在の値: " + health.Value);
            
            // 直接Subscribe出来る
            // Subscribeしたタイミングで現在の値が自動的に発行される
            health.Subscribe(
                x => Debug.Log("通知された値: " + x),
                () => Debug.Log("OnCompleted"));
            
            // Valueに値を設定すると、同時にOnNextが発行される
            health.Value = 50;
            
            Debug.Log("現在の値: " + health.Value);
            
            // Dispose()するとOnCompletedメッセージが発行される
            health.Dispose();
        }
    }
}
