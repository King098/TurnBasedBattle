using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace King.TurnBasedCombat
{
    [InitializeOnLoad]
    class CreateResourcesBundle
    {
        static CreateResourcesBundle()
        {
            EditorApplication.update += Update;
        }

        static void Update()
        {
            if (!EditorApplication.isCompiling)
            {
                OnUnityScripsCompilingCompleted();
                EditorApplication.update -= Update;
            }
        }

        private static void OnUnityScripsCompilingCompleted()
        {
            // Debug.Log("Unity Scrips Compiling completed.");
            if (!SystemSetting.UseResourcesPathOrStreamingAssetsPath)
            {
                if (!System.IO.File.Exists(Application.streamingAssetsPath + "/" + SystemSetting.AssetBundleName))
                {
                    int index = EditorUtility.DisplayDialogComplex("Tips", "You Need Create Resources AssetBundle To Run the Application!", "Windows", "Android", "IOS");
                    if (index == 0)
                    {
                        CreateBundleWindows();
                    }
                    else if (index == 1)
                    {
                        CreateBundleAndroid();
                    }
                    else if (index == 2)
                    {
                        CreateBundleIOS();
                    }
                }
            }
        }

        [MenuItem("Turn Based Combat/Create Resource Bundle (Windows)")]
        static void CreateBundleWindows()
        {
			List<string> f = new List<string>();
			GetDirs("Assets/TurnBasedCombat/Resources",ref f);
            string[] files = f.ToArray();
            for (int i = 0; i < files.Length; i++)
            {
                Debug.Log(files[i]);
                files[i] = files[i].Replace('\\', '/');
            }
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = SystemSetting.AssetBundleName;
            build.assetNames = files;
            if (!Directory.Exists("Assets/StreamingAssets"))
            {
                Directory.CreateDirectory("Assets/StreamingAssets");
            }
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", new AssetBundleBuild[] { build }, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
        }
        [MenuItem("Turn Based Combat/Create Resource Bundle (Android)")]
        static void CreateBundleAndroid()
        {
            List<string> f = new List<string>();
			GetDirs("Assets/TurnBasedCombat/Resources",ref f);
            string[] files = f.ToArray();
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = files[i].Replace('\\', '/');
            }
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = SystemSetting.AssetBundleName;
            build.assetNames = files;
            if (!Directory.Exists("Assets/StreamingAssets"))
            {
                Directory.CreateDirectory("Assets/StreamingAssets");
            }
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", new AssetBundleBuild[] { build }, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
        }
        [MenuItem("Turn Based Combat/Create Resource Bundle (IOS)")]
        static void CreateBundleIOS()
        {
            List<string> f = new List<string>();
			GetDirs("Assets/TurnBasedCombat/Resources",ref f);
            string[] files = f.ToArray();
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = files[i].Replace('\\', '/');
            }
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = SystemSetting.AssetBundleName;
            build.assetNames = files;
            if (!Directory.Exists("Assets/StreamingAssets"))
            {
                Directory.CreateDirectory("Assets/StreamingAssets");
            }
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets", new AssetBundleBuild[] { build }, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.iOS);
        }

        //参数1 为要查找的总路径， 参数2 保存路径  
        private static void GetDirs(string dirPath, ref List<string> dirs)
        {
            foreach (string path in Directory.GetFiles(dirPath, "*.*"))
            {
                dirs.Add(path.Substring(path.IndexOf("Assets")));
                Debug.Log(path.Substring(path.IndexOf("Assets")));
            }

            if (Directory.GetDirectories(dirPath).Length > 0)  //遍历所有文件夹  
            {
                foreach (string path in Directory.GetDirectories(dirPath))
                {
                    GetDirs(path, ref dirs);
                }
            }
        }
    }
}