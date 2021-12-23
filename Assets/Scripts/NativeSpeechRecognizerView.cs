using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class NativeSpeechRecognizerView : MonoBehaviour
{
    [SerializeField] private Text resultText = null;
    [SerializeField] private Text buttonText = null;
    private string _gameObjectName;
    bool isRequesting;

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void _prepareRecording(string _gameObjectName);
        [DllImport("__Internal")]
        private static extern void _recordButtonTapped();
        private static void PrepareRecording(string _gameObjectName)
        {
            _prepareRecording(_gameObjectName);
        }

        private static void RecordButtonTapped()
        {
            _recordButtonTapped();
        }
#endif

    private IEnumerator Start()
    {
        _gameObjectName = this.gameObject.name;
        foreach (var device in Microphone.devices)
        {
            resultText.text = resultText.text+"\n " + device;
        }

#if UNITY_IOS && !UNITY_EDITOR
        yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
        if (Application.HasUserAuthorization(UserAuthorization.Microphone)) {
            PrepareRecording(_gameObjectName);
        } else {
            Debug.Log("[iOS] Microphone not found");
        }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        // マイクパーミッションが許可されているか調べる
        if (!Permission.HasUserAuthorizedPermission (Permission.Microphone)) {
            // 権限が無いので、マイクパーミッションのリクエストをする
            yield return RequestUserPermission (Permission.Microphone);
        } else {
            Debug.Log("[Android] Microphone not found");
        }
#endif
        yield break;
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

#if UNITY_ANDROID && UNITY_IOS && !UNITY_EDITOR
    private IEnumerator OnApplicationFocus(bool hasFocus)
    {
        // iOSプラットフォームでは1フレーム待つ処理がないと意図通りに動かない。
        yield return null;

        if (isRequesting && hasFocus)
        {
            isRequesting = false;
        }
    }
#endif


    /// <summary>
    /// 音声認識開始
    /// </summary>
    public void StartRecognizer()
    {
        // 認識テキスト初期化
        resultText.text = "-";

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
                "Results",
                "ButtonResults"
            );
        }));
#elif UNITY_IOS && !UNITY_EDITOR
        RecordButtonTapped();
#endif

#if UNITY_EDITOR
        UnityEngine.Debug.Log("On Click!");
#endif
    }

    private void Results(string message)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        string[] messages = message.Split('\n');
        if (messages[0] == "onResults")
        {
            string msg = "";
            for (int i = 1; i < messages.Length; i++)
            {
                msg += messages[i] + "\n";
            }

            resultText.text = msg;
        } else {
            string msg = "-";
        }
#elif UNITY_IOS && !UNITY_EDITOR
        resultText.text = message;
#endif
    }

	public void ButtonResults(string message)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        string[] messages = message.Split('\n');
        if (message == "onReadyForSpeech") {
            this.GetComponent<Button>().interactable = false;
            buttonText.text = "認識を開始します";

        } else if (message == "onBeginningOfSpeech") {
            buttonText.text = "認識中...";

        } else if (message == "onEndOfSpeech") {
            buttonText.text = "終了...";
            StartCoroutine(SetStartTextCoroutine());
        } else {
            buttonText.text = message;
            this.GetComponent<Button>().interactable = true;

        }
#elif UNITY_IOS && !UNITY_EDITOR
		string[] messages = message.Split(':');
		if (messages.Length != 2) {
            return;

        } else if (messages[1] == "onReadyForSpeech") {
            buttonText.text = "認識を開始します";

        } else if (messages[1] == "onBeginningOfSpeech") {
            buttonText.text = "認識中...";

        } else if (messages[1] == "onEndOfSpeech") {
            buttonText.text = "終了...";
            StartCoroutine(SetStartTextCoroutine());

        } else {
            buttonText.text = message;
            this.GetComponent<Button>().interactable = true;

        }
#endif
	}

    private IEnumerator SetStartTextCoroutine()
    {
        buttonText.text = "認識を停止しました";
        yield return new WaitForSeconds(1.5f);
        buttonText.text = "認識を開始します";

        this.GetComponent<Button>().interactable = true;
    }

}
