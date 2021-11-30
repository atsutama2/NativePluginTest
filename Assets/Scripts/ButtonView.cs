using System.Runtime.InteropServices;
using UnityEngine;

public class ButtonView : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern void _showAlert();
#endif

    public void ShowNativeDialog() {
#if UNITY_ANDROID && !UNITY_EDITOR
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
#elif UNITY_IOS && !UNITY_EDITOR
        _showAlert();
#elif UNITY_EDITOR
        UnityEngine.Debug.Log("On Click!");
#endif
    }
}
