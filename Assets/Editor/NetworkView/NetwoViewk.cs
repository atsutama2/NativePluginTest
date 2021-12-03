using UnityEditor;
using UnityEngine;

public sealed class NetworkView : EditorWindow
{
    [MenuItem("Tools/NetworkWindow", false)]
    private static void Open()
    {
        EditorWindow.GetWindow<NetworkView>("NetworkConfirmation");
    }
}
