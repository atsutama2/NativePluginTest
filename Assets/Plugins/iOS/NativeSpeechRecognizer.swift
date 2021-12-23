import Foundation
import Speech

@objc public class NativeSpeechRecognizer : NSObject {

    @objc static let sharedInstance = NativeSpeechRecognizer()

    private let speechRecognizer = SFSpeechRecognizer(locale: Locale(identifier: "ja-JP"))!
    private var recognitionRequest: SFSpeechAudioBufferRecognitionRequest?
    private var recognitionTask: SFSpeechRecognitionTask?
    private let audioEngine = AVAudioEngine()

    private var gameObjectName: String?

    @objc public func prepareRecording(_ gameObjectName: String) {

        self.gameObjectName = gameObjectName

        // Asynchronously make the authorization request.
        SFSpeechRecognizer.requestAuthorization { authStatus in

            // Divert to the app's main thread so that the UI
            // can be updated.
            OperationQueue.main.addOperation {
                switch authStatus {
                case .authorized:
                    self.updateState(false, title: "authorized")
                case .denied:
                    self.updateState(false, title: "Error denied")
                case .restricted:
                    self.updateState(false, title: "Error restricted")
                case .notDetermined:
                    self.updateState(false, title: "Error notDetermined")
                default:
                    self.updateState(false, title: "Other Error")
                }
            }
        }
    }

    private func startRecording() throws {

        // Cancel the previous task if it's running.
        recognitionTask?.cancel()
        self.recognitionTask = nil

        // Configure the audio session for the app.
        let audioSession = AVAudioSession.sharedInstance()
        try audioSession.setCategory(.record, mode: .measurement, options: .duckOthers)
        try audioSession.setActive(true, options: .notifyOthersOnDeactivation)
        let inputNode = audioEngine.inputNode

        // Create and configure the speech recognition request.
        recognitionRequest = SFSpeechAudioBufferRecognitionRequest()
        guard let recognitionRequest = recognitionRequest else {
            fatalError("Unable to created a SFSpeechAudioBufferRecognitionRequest object")
        }
        recognitionRequest.shouldReportPartialResults = true

        // Keep speech recognition data on device
        if #available(iOS 13, *) {
            recognitionRequest.requiresOnDeviceRecognition = false
        }

        // Create a recognition task for the speech recognition session.
        // Keep a reference to the task so that it can be canceled.
        recognitionTask = speechRecognizer.recognitionTask(with: recognitionRequest) { result, error in
            var isFinal = false

            if let result = result {
                // Update the text view with the results.
                self.updateField(result.bestTranscription.formattedString)
                isFinal = result.isFinal
                print("Text \(result.bestTranscription.formattedString)")
            }

            if error != nil || isFinal {
                // Stop recognizing speech if there is a problem.
                self.audioEngine.stop()
                inputNode.removeTap(onBus: 0)

                self.recognitionRequest = nil
                self.recognitionTask = nil

                self.updateState(true, title: "onReadyForSpeech")
            }
        }

        // Configure the microphone input.
        let recordingFormat = inputNode.outputFormat(forBus: 0)
        inputNode.installTap(onBus: 0, bufferSize: 1024, format: recordingFormat) { (buffer: AVAudioPCMBuffer, when: AVAudioTime) in
            self.recognitionRequest?.append(buffer)
        }

        self.audioEngine.prepare()
        try self.audioEngine.start()

        self.updateField("")
    }

    // MARK: SFSpeechRecognizerDelegate

    @objc public func speechRecognizer(_ speechRecognizer: SFSpeechRecognizer, availabilityDidChange available: Bool) {
        if available {
            self.updateState(true, title: "onReadyForSpeech")
        } else {
            self.updateState(false, title: "Recognition not available")
        }
    }

    // MARK: Interface Builder actions

    @objc public func recordButtonTapped() {
        if audioEngine.isRunning {
            audioEngine.stop()
            recognitionRequest?.endAudio()
            self.updateState(false, title: "onEndOfSpeech")
        } else {
            do {
                try startRecording()
                self.updateState(true, title: "onBeginningOfSpeech")
            } catch {
                self.updateState(true, title: "Can not record")
            }
        }
    }

    private func updateState(_ isEnabled: Bool, title: String) {
        UnityFramework.getInstance().sendMessageToGO(
            withName: self.gameObjectName!,
            functionName: "OnCallbackStateResults",
            message: "\(isEnabled):\(title)")
    }

    private func updateField(_ text: String) {
        UnityFramework.getInstance().sendMessageToGO(
            withName: self.gameObjectName!,
            functionName: "OnCallbackResults",
            message: text)
    }
}
