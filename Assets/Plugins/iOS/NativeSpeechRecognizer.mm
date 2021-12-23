#import <UnityFramework/UnityFramework-Swift.h>

extern "C"
{
    NativeSpeechRecognizer *sharedInstance = [[NativeSpeechRecognizer alloc] init];

    void _prepareRecording(const char* gameObjectName)
    {

        [sharedInstance prepareRecording:[NSString stringWithUTF8String:gameObjectName]];
    }

    void _recordButtonTapped()
    {
        [sharedInstance recordButtonTapped];
    }
}
