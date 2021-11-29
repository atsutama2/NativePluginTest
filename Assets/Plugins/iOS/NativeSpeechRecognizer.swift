import Foundation
import Speech

public class NativeSpeechRecognizer : NSObject {

    @objc static let sharedInstance = NativeSpeechRecognizer()

    private var speechRecognizer: SFSpeechRecognizer?
    private var recognitionRequest: SFSpeechAudioBufferRecognitionRequest?
    private var recognitionTask: SFSpeechRecognitionTask?
    private var audioEngine: AVAudioEngine?

    private var gameObjectName: String?

    override init() {
        super.init()
        self.speechRecognizer = SFSpeechRecognizer(locale: Locale(identifier: "ja-JP"))!
        self.audioEngine = AVAudioEngine()

        // オンライン認識
        if #available(iOS 13, *) {
            recognitionRequest?.requiresOnDeviceRecognition = false
        }
    }

    @objc func prepareRecording(_ gameObjectName: String) {

        self.gameObjectName = gameObjectName

        SFSpeechRecognizer.requestAuthorization { authStatus in
            OperationQueue.main.addOperation {
                switch authStatus {
                case .authorized:
                    self.updateButton(true, title: "録音を開始します")

                case .denied:
                    self.updateButton(false, title: "ユーザーが音声認識へのアクセスを拒否されました")

                case .restricted:
                    self.updateButton(false, title: "この装置では音声認識が制限されています")

                case .notDetermined:
                    self.updateButton(false, title: "スピーチ認識はまだ認証されていません")
                }
            }
        }
    }

    fileprivate func startRecording() throws {
        if let recognitionTask = self.recognitionTask {
            recognitionTask.cancel()
            self.recognitionTask = nil
        }

        let audioSession = AVAudioSession.sharedInstance()
        try audioSession.setCategory(AVAudioSessionCategoryRecord)
        try audioSession.setMode(AVAudioSessionModeMeasurement)
        try audioSession.setActive(true, with: .notifyOthersOnDeactivation)

        self.recognitionRequest = SFSpeechAudioBufferRecognitionRequest()

        guard let inputNode = self.audioEngine?.inputNode else {
            fatalError("オーディオエンジンには入力ノードがありません")
        }
        guard let recognitionRequest = self.recognitionRequest else {
            fatalError("SFSpeechAudioBufferRecognitionRequestオブジェクトを作成できません")
        }

        self.recognitionRequest?.shouldReportPartialResults = true

        self.recognitionTask = self.speechRecognizer?.recognitionTask(with: recognitionRequest) { result, error in
            var isFinal = false

            if let result = result {
                self.updateField(result.bestTranscription.formattedString)
                isFinal = result.isFinal
            }

            if error != nil || isFinal {
                self.audioEngine?.stop()
                inputNode.removeTap(onBus: 0)

                self.recognitionRequest = nil
                self.recognitionTask = nil

                self.updateButton(true, title: "録音を開始します")
            }
        }

        let recordingFormat = inputNode.outputFormat(forBus: 0)
        inputNode.installTap(onBus: 0, bufferSize: 1024, format: recordingFormat) { (buffer: AVAudioPCMBuffer, when: AVAudioTime) in
            self.recognitionRequest?.append(buffer)
        }

        self.audioEngine?.prepare()

        try self.audioEngine?.start()

        self.updateField("")
    }

    @nonobjc internal func speechRecognizer(_ speechRecognizer: SFSpeechRecognizer, availabilityDidChange available: Bool) {
        if available {
            self.updateButton(true, title: "録音を開始します")
        } else {
            self.updateButton(false, title: "認識がありません")
        }
    }

    // MARK: Interface Builder actions
    @objc func recordButtonTapped() {
        if (self.audioEngine?.isRunning)! {
            self.audioEngine?.stop()
            self.recognitionRequest?.endAudio()
            self.updateButton(false, title: "停止...")
        } else {
            try! self.startRecording()
            self.updateButton(true, title: "録音を停止します")
        }
    }

    fileprivate func updateButton(_ isEnabled: Bool, title: String) {
        UnitySendMessage(self.gameObjectName, "ButtonResults", "\(isEnabled):\(title)")
    }

    fileprivate func updateField(_ text: String) {
        UnitySendMessage(self.gameObjectName, "Results", text)
    }
}
