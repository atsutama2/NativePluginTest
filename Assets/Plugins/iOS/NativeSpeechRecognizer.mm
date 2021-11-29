#import <Foundation/Foundation.h>
#import <Speech/Speech.h>
#import "Unity-iPhone-Swift.h"

extern "C"
{
    NativeSpeechRecognizer *instance = [NativeSpeechRecognizer sharedInstance];

    void _prepareRecording(const char* gameObjectName)
    {
        [instance prepareRecording:[NSString stringWithUTF8String:gameObjectName]];
    }

    void _recordButtonTapped()
    {
        [instance recordButtonTapped];
    }
}
