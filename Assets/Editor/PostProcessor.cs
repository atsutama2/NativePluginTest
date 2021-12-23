using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

namespace UnitySwift {
    public static class PostProcessor {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath) {
            if(buildTarget == BuildTarget.iOS) {

                var plistPath = buildPath + "/Info.plist";
				var plist = new PlistDocument();
				plist.ReadFromFile(plistPath);

                plist.root.values["NSMicrophoneUsageDescription"] = new PlistElementString("音読音声の認識にマイクを使用します");
				plist.root.values["NSSpeechRecognitionUsageDescription"] = new PlistElementString("音読音声の認識に音声認識を利用します");

                plist.WriteToFile(plistPath);

                var projPath = PBXProject.GetPBXProjectPath(buildPath);
                var proj = new PBXProject();
                proj.ReadFromString(File.ReadAllText(projPath));

                var targetGUID = proj.GetUnityFrameworkTargetGuid();
                proj.SetBuildProperty(targetGUID, "SWIFT_VERSION", "5.0");
                File.WriteAllText(projPath, proj.WriteToString());

                proj.WriteToFile(projPath);
            }
        }
    }
}
