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

                var projPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
                var proj = new PBXProject();
                proj.ReadFromFile(projPath);

                string xcodeTarget = proj.TargetGuidByName("Unity-iPhone");

                proj.AddBuildProperty(xcodeTarget, "SWIFT_VERSION", "4.0");
                proj.SetBuildProperty(xcodeTarget, "ENABLE_BITCODE", "NO");
                proj.SetBuildProperty(xcodeTarget, "SWIFT_OBJC_BRIDGING_HEADER", "Libraries/Plugins/iOS/UnitySwift-Bridging-Header.h");
                proj.SetBuildProperty(xcodeTarget, "SWIFT_OBJC_INTERFACE_HEADER_NAME", "Unity-iPhone-Swift.h");
                proj.AddBuildProperty(xcodeTarget, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");

                proj.WriteToFile(projPath);
            }
        }
    }
}
