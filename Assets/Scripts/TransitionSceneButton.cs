using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionSceneButton : MonoBehaviour
{
    [SerializeField] private AppDefine.SceneType sceneName;

    public void OnTransitionSceneStart()
    {
        if (string.IsNullOrEmpty(sceneName.ToString())) {
            return;
        }

        SceneManager.LoadScene(sceneName.ToString());
    }
}
