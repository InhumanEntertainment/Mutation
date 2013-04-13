using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Inhuman
{
    [System.Serializable]
    public class Build
    {
        public string Name = "Product Name";
        public string Version = "1.0";
        public string BundleId = "com.inhuman.default";

        public string Path = "Build/Folder/";
        public BuildTarget Target = BuildTarget.StandaloneWindows;
        public BuildOptions Options = BuildOptions.AutoRunPlayer | BuildOptions.ShowBuiltPlayer;
        public string Script;
        public string ScriptCommand;

        public AspectRatios Aspect = AspectRatios.Free;

        public string Icon;
        public string Splash;

        public static Dictionary<BuildTarget, string> TargetExtension = new Dictionary<BuildTarget, string>() 
        { 
            { BuildTarget.StandaloneWindows, ".exe" }, 
            { BuildTarget.StandaloneWindows64, ".exe" },
            { BuildTarget.Android, ".apk" },
            { BuildTarget.iPhone, ".xcode" },
            { BuildTarget.MetroPlayerX86, ".zip" },
            { BuildTarget.StandaloneOSXIntel, ".app" },
            { BuildTarget.WebPlayer, "" },
            { BuildTarget.FlashPlayer, "" },
        };

        public enum AspectRatios { None, iPhone, iPad, iPhone5, Nexus7, KindleFire, PC, Free }
        public static Dictionary<AspectRatios, Rect> TargetAspect = new Dictionary<AspectRatios, Rect>() 
        { 
            { AspectRatios.iPhone, new Rect(0, 0, 640, 960) }, 
            { AspectRatios.iPhone5, new Rect(0, 0, 640, 960) }, 
            { AspectRatios.iPad, new Rect(0, 0, 768, 1024) },       
            { AspectRatios.Free, new Rect(0, 0, 1000, 1000) },
            { AspectRatios.KindleFire, new Rect(0, 0, 600, 1004) },
            { AspectRatios.PC, new Rect(0, 0, 640, 480) },
        };

        //=====================================================================================================================================//
        public void Activate()
        {
            Debug.Log("BUILD MANAGER: " + Name);

            PlayerSettings.productName = Name;
            PlayerSettings.iOS.applicationDisplayName = Name;
            PlayerSettings.bundleVersion = Version;
            PlayerSettings.bundleIdentifier = BundleId;
            PlayerSettings.iPhoneBundleIdentifier = BundleId;

            EditorUserBuildSettings.SetBuildLocation(Target, BuildPath());

            if (TargetAspect.ContainsKey(Aspect))
            {
                PlayerSettings.defaultScreenWidth = (int)TargetAspect[Aspect].width;
                PlayerSettings.defaultScreenHeight = (int)TargetAspect[Aspect].height;
            }

            // Set Icons //
            if (Icon != "")
            {
                string path = AssetDatabase.GUIDToAssetPath(Icon);
                Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
                Texture2D[] icons = new Texture2D[] { texture };
                PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.iPhone, icons);
            }

            // Set Splash //
            if (Splash != "")
            {
                string splashPath = AssetDatabase.GUIDToAssetPath(Splash);
                Texture2D splashTexture = (Texture2D)AssetDatabase.LoadAssetAtPath(splashPath, typeof(Texture2D));
                PlayerSettings.resolutionDialogBanner = splashTexture;

                // Copy Te
                File.Copy(splashPath, "Assets/Textures/Splash.tga", true);
                AssetDatabase.Refresh();
            }

            // Call Script //
            GameObject scriptObject = GameObject.Find(Script);
            if (scriptObject)
            {
                scriptObject.SendMessage(ScriptCommand, SendMessageOptions.DontRequireReceiver);
            }
        }

        //=====================================================================================================================================//
        public void Run()
        {
            string buildPath = BuildPath();
            Debug.Log("BUILD MANAGER: Building - " + buildPath);

            string[] levels = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                levels[i] = EditorBuildSettings.scenes[i].path;
            }

            CreateFolder(System.IO.Path.GetDirectoryName(buildPath));

            if (this.Target == BuildTarget.Android)
            {
                Options = BuildOptions.None;
            }
            else
            {
                Options = BuildOptions.AutoRunPlayer | BuildOptions.ShowBuiltPlayer;
            }

            BuildPipeline.BuildPlayer(levels, buildPath, Target, Options);
        }

        //=====================================================================================================================================//
        public string BuildPath()
        {
            return Path + Name + TargetExtension[Target];
        }

        //=====================================================================================================================================//
        static void CreateFolder(string path)
        {
            if (!Directory.GetParent(path).Exists)
            {
                CreateFolder(Directory.GetParent(path).FullName);
            }
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
                Debug.Log("Created Folder: " + path);
            }
        }
    }
}