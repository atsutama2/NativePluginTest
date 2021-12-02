import Foundation
import Speech

public class NativeSpeechRecognizer : NSObject {

    @objc static let sharedInstance = NativeSpeechRecognizer()

    private var speechRecognizer: SFSpeechRecognizer?
    private var recognitionRequest: SFSpeechAudioBufferRecognitionRequest?
    private var recognitionTask: SFSpeechRecognitionTask?
    private var audioEngine: AVAudioEngine?

    private var gameObjectName: String?
    var queue: AudioQueueRef!
    var timer: Timer!
    var volume = 0

    override init() {
        super.init()
        self.speechRecognizer = SFSpeechRecognizer(locale: Locale(identifier: "ja-JP"))!
        self.audioEngine = AVAudioEngine()
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

        // オンライン認識
        if #available(iOS 13, *) {
            recognitionRequest.requiresOnDeviceRecognition = true
        }

        self.recognitionTask = self.speechRecognizer?.recognitionTask(with: recognitionRequest) { result, error in
            var isFinal = false

            if let result = result {
                let resultString = result.bestTranscription.formattedString
                print("Now: " + resultString)

                self.updateField(resultString)

                isFinal = result.isFinal
                print("isFinal" + String(isFinal))
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

        //音量測定
        // self.SettingVolume()

        self.audioEngine?.prepare()

        try self.audioEngine?.start()

        self.updateField("")
    }

    public func speechRecognizer(_ speechRecognizer: SFSpeechRecognizer, availabilityDidChange available: Bool) {
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
            // self.StopUpdatingVolume()
        } else {
            do {
                try startRecording()
                self.updateButton(true, title: "録音を停止します")
            } catch {
                self.updateButton(true, title: "録音ができません")
            }
        }
    }

    fileprivate func updateButton(_ isEnabled: Bool, title: String) {
        UnitySendMessage(self.gameObjectName, "ButtonResults", "\(isEnabled):\(title)")
    }

    fileprivate func updateField(_ text: String) {
        UnitySendMessage(self.gameObjectName, "Results", text)
    }

    // fileprivate func updateVolume(_ text: String) {
    //     UnitySendMessage(self.gameObjectName, "OnCallbackVolume", text)
    // }

    //音量測定セッティング
    // func SettingVolume(){
    //     //データフォーマット設定
    //     var dataFormat = AudioStreamBasicDescription(
    //         mSampleRate: 44100.0,
    //         mFormatID: kAudioFormatLinearPCM,
    //         mFormatFlags: AudioFormatFlags(kLinearPCMFormatFlagIsBigEndian | kLinearPCMFormatFlagIsSignedInteger | kLinearPCMFormatFlagIsPacked),
    //         mBytesPerPacket: 2,
    //         mFramesPerPacket: 1,
    //         mBytesPerFrame: 2,
    //         mChannelsPerFrame: 1,
    //         mBitsPerChannel: 16,
    //         mReserved: 0)

    //     //インプットレベルの設定
    //     var audioQueue: AudioQueueRef? = nil
    //     var error = noErr
    //     error = AudioQueueNewInput(
    //         &dataFormat,
    //         AudioQueueInputCallback,
    //         .none,
    //         .none,
    //         .none,
    //         0,
    //         &audioQueue)

    //     if error == noErr {
    //         self.queue = audioQueue
    //     }

    //     AudioQueueStart(self.queue, nil)

    //     //音量を取得の設定
    //     var enabledLevelMeter: UInt32 = 1
    //     AudioQueueSetProperty(self.queue,
    //                           kAudioQueueProperty_EnableLevelMetering,
    //                           &enabledLevelMeter,
    //                           UInt32(MemoryLayout<UInt32>.size))

    //     self.timer = Timer.scheduledTimer(timeInterval: 1.0,
    //                                         target: self,
    //                                         selector: #selector(DetectVolume(_:)),
    //                                         userInfo: nil,
    //                                         repeats: true)
    //     self.timer.fire()

    // }

    //音量測定
//     @objc func DetectVolume(_ timer: Timer) {
//         //音量取得
//         var levelMeter = AudioQueueLevelMeterState()
//         var propertySize = UInt32(MemoryLayout<AudioQueueLevelMeterState>.size)

//         AudioQueueGetProperty(
//             self.queue,
//             kAudioQueueProperty_CurrentLevelMeterDB,
//             &levelMeter,
//             &propertySize)

//         self.volume = (Int)((levelMeter.mPeakPower + 144.0) * (100.0/144.0))

//         print("volume: " + String(self.volume))
//         self.updateVolume(String(self.volume))

// //        let mPeakPower = levelMeter.mPeakPower
// //        let mAveragePower = levelMeter.mAveragePower
// //        print("mPeakPower: " + String(mPeakPower))
// //        print("mAveragePower: " + String(mAveragePower))

//     }

    // 測定停止
    // private func StopUpdatingVolume()
    // {
    //     self.timer.invalidate()
    //     self.timer = nil
    //     AudioQueueFlush(self.queue)
    //     AudioQueueStop(self.queue, false)
    //     AudioQueueDispose(self.queue, true)
    // }
}

// private func AudioQueueInputCallback(
//     _ inUserData: UnsafeMutableRawPointer?,
//     inAQ: AudioQueueRef,
//     inBuffer: AudioQueueBufferRef,
//     inStartTime: UnsafePointer<AudioTimeStamp>,
//     inNumberPacketDescriptions: UInt32,
//     inPacketDescs: UnsafePointer<AudioStreamPacketDescription>?)
// {
//     // Do nothing, because not recoding.
// }
