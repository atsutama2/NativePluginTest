﻿using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class NativeSpeechRecognizerView : MonoBehaviour
{
    [SerializeField] private Text _text = null;
    bool isRequesting;

    private IEnumerator Start()
    {
        // マイクパーミッションが許可されているか調べる
        if (!Permission.HasUserAuthorizedPermission (Permission.Microphone)) {
            // 権限が無いので、マイクパーミッションのリクエストをする
            yield return RequestUserPermission (Permission.Microphone);
        }

        foreach (var device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
            _text.text = _text.text+"\n " + device;
        }
    }

    private IEnumerator RequestUserPermission(string permission)
    {
        isRequesting = true;
        Permission.RequestUserPermission(permission);
        // Androidでは「今後表示しない」をチェックされた状態だとダイアログは表示されないが、フォーカスイベントは通常通り発生する模様。
        // したがってタイムアウト処理は本来必要ないが、万が一の保険のために一応やっとく。

        // アプリフォーカスが戻るまで待機する
        float timeElapsed = 0;
        while (isRequesting)
        {
            if (timeElapsed > 0.5f){
                isRequesting = false;
                yield break;
            }
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        yield break;
    }

    public void StartRecognizer() {
        #if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass nativeRecognizer = new AndroidJavaClass ("jp.co.test.nativespeechrecognizer.NativeSpeechRecognizer");
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject context  = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            string gameObjectName = gameObject.name;
            context.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                nativeRecognizer.CallStatic(
                    "StartRecognizer",
                    context,
                    gameObjectName,
                    "CallbackMethod"
                );
            }));
		#elif UNITY_IOS && !UNITY_EDITOR

        #elif UNITY_EDITOR
            UnityEngine.Debug.Log("On Click!");
        #endif
    }

    private void CallbackMethod(string message)
    {
        _text.text = message;
        var messages = message.Split('\n');

        //ユーザーが話すのを開始した際のコールバック
        if (messages[0] == "onBeginningOfSpeech")
        {
            _text.text = "Start Voice";
            _text.enabled = true;
        }

        //認識した音量変化のコールバック
        if (messages[0] == "onRmsChanged")
        {
            _text.text = "認識中...";
        }

        //ユーザーが話すのを終了した際のコールバック
        if (messages[0] == "onEndOfSpeech")
        {
            _text.text = "Stop Voice";
            _text.enabled = false;
        }

        //認識した音量変化のコールバック
        if (messages[0] == "onError")
        {
            _text.text = "エラー";
        }

        //認識結果の準備が完了したコールバック
        if (messages[0] == "onResults")
        {
            _text.text = "onResults";
            var msg = "";
            for (var i = 1; i < messages.Length; i++)
            {
                msg += messages[i] + "\n";
            }

            Debug.Log(msg);
            _text.text = msg;
        }
    }

    public void ShowStaticFunction(){
        AndroidJavaClass nativeRecognizer = new AndroidJavaClass ("jp.co.test.nativespeechrecognizer.NativeSpeechRecognizer");
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        nativeRecognizer.CallStatic("staticFunction", gameObject.name, "onCallBackShowResult");
    }

    public void onCallBackShowResult(string resultText)
    {
        _text.text = resultText;
    }
}