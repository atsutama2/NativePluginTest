using UnityEngine;

namespace Zenject
{
    /// <summary>
    /// テスト用クラス
    /// </summary>
    public class TextDisplay : MonoBehaviour
    {
        [Inject] private ITest test;

        private void Update()
        {
            test.DebugTest();
        }
    }

    /// <summary>
    /// インターフェイス
    /// </summary>
    public interface ITest
    {
        void DebugTest();
    }

    public class Test : ITest
    {
        public void DebugTest()
        {
            Debug.Log("初期メッセージ");
        }
    }

    public class Test2 : ITest
    {
        public void DebugTest()
        {
            Debug.Log("機能変更メッセージ");
        }
    }
}