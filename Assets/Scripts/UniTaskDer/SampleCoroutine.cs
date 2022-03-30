using System.Collections;
using UnityEngine;

namespace UniTaskDer
{
    public class SampleCoroutine : MonoBehaviour
    {
        [SerializeField] private GameObject obj;
        
        private void Start()
        {
            // 特定のパターンでオブジェクトを移動させる
            StartCoroutine(MovePatternCoroutine());
        }
        
        // 特定パターンで移動するコルーチン
        private IEnumerator MovePatternCoroutine()
        {
            yield return MoveCoroutine(Vector3.up * 2.0f, 1.0f);
            yield return new WaitForSeconds(1);
            
            yield return MoveCoroutine(Vector3.down * 2.0f, 3.0f);
            yield return new WaitForSeconds(0.5f);
            
            yield return MoveCoroutine(Vector3.right * 4.0f, 2.0f);
            yield return new WaitForSeconds(2);
        }

        // 指定した速度で指定秒数移動するコルーチン
        private IEnumerator MoveCoroutine(Vector3 velocity, float seconds)
        {
            var startTime = Time.time;
            while (Time.time - startTime < seconds)
            {
                obj.transform.position += velocity * Time.deltaTime;
                yield return null;
            }
        }
    }
}
