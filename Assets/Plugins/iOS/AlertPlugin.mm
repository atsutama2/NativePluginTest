#import "Unity-iPhone-Swift.h"

extern "C"
{
    void _showAlert()
    {
        [AlertPlugin showAlert];
    }
}
