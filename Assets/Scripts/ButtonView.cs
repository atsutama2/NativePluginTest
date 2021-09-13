using UnityEngine;

public class ButtonView : MonoBehaviour
{
    public void ShowNativeDialog() {
        #if UNITY_ANDROID
        AndroidJavaClass nativeDialog = new AndroidJavaClass ("jp.co.test.mylibrary.AndroidNativeDialog");
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject context  = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        context.Call ("runOnUiThread", new AndroidJavaRunnable(() => {
            nativeDialog.CallStatic (
                "showNativeDialog",
                context,
                "タイトル",
                "本文テキスト"
            );
        }));
        #elif UNITY_EDITOR
            UnityEngine.Debug.Log("On Click!");
        #endif
    }
}
