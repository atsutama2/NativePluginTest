using UnityEngine;

namespace UniRx
{
    public class OnCompletedSample : MonoBehaviour
    {
        private void Start()
        {
            // データベース作成
            var subject = new Subject<string>();
            
            // Observaleの購読
            // string型をint型に変換、失敗したら例外
            subject.Select(int.Parse)
                .Subscribe(
                    // OnNextメッセージのハンドリング
                    x => Debug.Log(x),
                    // OnErrorメッセージのハンドリング
                    ex => Debug.Log("例外が発生しました: " + ex.Message),
                    // OnCompletedメッセージのハンドリング
                    () => Debug.Log("OnCompleted!")
                );
            
            subject.OnNext("1");
            subject.OnNext("2");
            
            // int.Parseに失敗して例外が発生する
            // OnErrorメッセージがSelectオペレータから発行される
            // そのためメッセージ処理後に購読が終了する
            subject.OnNext("Three");
            
            // OnErrorメッセージ発生により購読は終了済み
            // 有効なObserverがいないため通知されない
            subject.OnNext("4");
            
            // ただしsubject自体が発行したわけではないのでsubjectはまだ正常稼働中
            // そのため再購読すればまた利用できる
            subject.Subscribe(
                Debug.Log,
                () => Debug.Log("Completed!"));
            
            subject.OnNext("Hello!");
            subject.OnCompleted();
            
            // 破棄
            subject.Dispose();
        }
    }
}
