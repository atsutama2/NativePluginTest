using System;
using UniRx.Triggers;
using UnityEngine;

namespace UniRx
{
    // 連打防止 UniRxでやった場合
    public class ThrottleButton : MonoBehaviour
    {
        private void Start()
        {
            // Update()のタイミングでAttackボタンが押されているか判定
            // Attackボタンが押されたらSubscribe()の処理を実行
            // そのあと30フレームの間ボタン入力を無視
            this.UpdateAsObservable().Where(_ => Input.GetButtonDown("Attack")).ThrottleFirstFrame(600).Subscribe(_ =>
            {
                Debug.Log("Attack!");
            });
        }
    }
}