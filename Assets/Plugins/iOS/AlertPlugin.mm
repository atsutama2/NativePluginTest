#import "unityswift-Swift.h"

extern "C"
{
    void _showAlert()
    {
        [AlertPlugin showAlert];
    }
}
