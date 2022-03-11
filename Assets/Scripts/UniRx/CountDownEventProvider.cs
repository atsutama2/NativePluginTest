using System;
using System.Collections;
using UnityEngine;


namespace UniRx
{
    // 指定秒数カウントイベント通知する
    public class CountDownEventProvider : MonoBehaviour
    {
        // カウントする秒数
        [SerializeField] private int _countSeconds = 10;

        // Subjectのインスタンス
        private Subject<int> _subject;
        
        // SubjectのIObservableインターフェイス部分のみ公開する
        public IObservable<int> CountDownObservable => _subject;

        private void Awake()
        {
            // Subject生成
            _subject = new Subject<int>();
            
            // カウントダウンするコルーチン起動
            StartCoroutine(CountCoroutine());
        }
        
        // 指定秒数カウントし、その都度メッセージ発行するコルーチン
        private IEnumerator CountCoroutine()
        {
            var current = _countSeconds;

            while (current > 0)
            {
                // 現在の値を発行する
                _subject.OnNext(current);
                current--;
                yield return new WaitForSeconds(1.0f);
            }
            
            // 最後に0とOnComplatedメッセージを発行する
            _subject.OnNext(0);
            _subject.OnCompleted();
        }

        private void OnDestroy()
        {
            // GameObjectが破棄されたらSubjectも開放する
            _subject.Dispose();
        }
    }
}
