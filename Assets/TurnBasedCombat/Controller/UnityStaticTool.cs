using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace King.Tools
{
    /// <summary>
    /// 静态类，用于实现各种Unity里面的工具方法，和自己的方法
    /// </summary>
    public class UnityStaticTool
    {
        /// <summary>
        /// 清除一个GameObject的所有子物体
        /// </summary>
        /// <param name="parent">GameObject对象</param>
        /// <param name="include_inactive">是否清除未激活的对象,默认清除</param>
        public static void DestoryChilds(GameObject parent, bool include_inactive = true)
        {
            Transform[] trans = parent.GetComponentsInChildren<Transform>(include_inactive);
            for (int i = 1; i < trans.Length; i++)
            {
                GameObject.Destroy(trans[i].gameObject);
            }
        }


        /// <summary>
        /// 立即清除一个GameObject的所有子物体
        /// </summary>
        /// <param name="parent">GameObject对象</param>
        /// <param name="include_inactive">是否清除未激活的对象,默认清除</param>
        public static void DestoryChildsImmdiate(GameObject parent, bool include_inactive = true)
        {
            Transform[] trans = parent.GetComponentsInChildren<Transform>(include_inactive);
            for (int i = 1; i < trans.Length; i++)
            {
                GameObject.DestroyImmediate(trans[i].gameObject);
            }
        }

        /// <summary>
        /// 创建一个对象在
        /// </summary>
        /// <typeparam name="T">继承自Component的对象类型</typeparam>
        /// <param name="prefab">创建的预制体</param>
        /// <param name="parent">创建的物体的父级</param>
        /// <param name="pos">创建的物体相对于父级的坐标</param>
        /// <param name="scale">创建的物体相对于父级的缩放</param>
        /// <param name="rota">创建的物体相对于父级的旋转</param>
        /// <param name="result">创建的物体的存储</param>
        /// <returns>如果创建成功，则返回true，否则返回false</returns>
        public static bool CreateObjectReletiveTo<T>(GameObject prefab, Transform parent, Vector3 pos, Vector3 scale, Vector3 rota, out T result) where T : Component
        {
            if (prefab != null)
            {
                GameObject obj = GameObject.Instantiate<GameObject>(prefab);
                if (obj != null)
                {
                    obj.transform.SetParent(parent, false);
                    obj.transform.localPosition = pos;
                    obj.transform.localRotation = Quaternion.Euler(rota);
                    obj.transform.localScale = scale;
                    result = obj.GetComponent<T>();
                    return true;
                }
                else
                {
                    Debug.LogError("Create GameObject Failed!");
                    result = null;
                    return false;
                }
            }
            Debug.LogError("Cannot Create GameObject,Because of the empty prefab!");
            result = null;
            return false;
        }

        public static bool CreateObjectReletiveTo<T>(GameObject prefab, Transform parent, Vector3 pos, out T result) where T : Component
        {
            return CreateObjectReletiveTo<T>(prefab, parent, pos, Vector3.one, Vector3.zero, out result);
        }

        public static bool CreateObjectReletiveTo<T>(GameObject prefab, Transform parent, out T result) where T : Component
        {
            return CreateObjectReletiveTo<T>(prefab, parent, Vector3.zero, Vector3.one, Vector3.zero, out result);
        }

        /// <summary>
        /// 创建一个对象在
        /// </summary>
        /// <typeparam name="T">继承自Component的对象类型</typeparam>
        /// <param name="prefab">创建的预制体</param>
        /// <param name="parent">创建的物体的父级</param>
        /// <param name="pos">创建的物体在世界坐标中的坐标</param>
        /// <param name="scale">创建的物体在世界坐标中的缩放</param>
        /// <param name="rota">创建的物体在世界坐标中的旋转</param>
        /// <param name="result">创建的物体的存储</param>
        /// <returns>如果创建成功，则返回true，否则返回false</returns>
        public static bool CreateObjectTo<T>(GameObject prefab, Transform parent, Vector3 pos, Vector3 scale, Vector3 rota, out T result) where T : Component
        {
            if (prefab != null)
            {
                GameObject obj = GameObject.Instantiate<GameObject>(prefab);
                if (obj != null)
                {
                    obj.transform.SetParent(parent, false);
                    obj.transform.position = pos;
                    obj.transform.rotation = Quaternion.Euler(rota);
                    obj.transform.lossyScale.Scale(scale);
                    result = obj.GetComponent<T>();
                    return true;
                }
                else
                {
                    Debug.LogError("Create GameObject Failed!");
                    result = null;
                    return false;
                }
            }
            Debug.LogError("Cannot Create GameObject,Because of the empty prefab!");
            result = null;
            return false;
        }

        public static bool CreateObjectTo<T>(GameObject prefab, Transform parent, Vector3 pos, out T result) where T : Component
        {
            return CreateObjectTo<T>(prefab, parent, pos, Vector3.one, Vector3.zero, out result);
        }


        /// <summary>
        /// 播放一个特效对象
        /// </summary>
        /// <param name="effect">特效对象</param>
        public static void PlayEffect(GameObject effect)
        {
            if (effect == null)
                return;
            if (!effect.activeInHierarchy)
                effect.SetActive(true);
            ParticleSystem particle = effect.GetComponentInChildren<ParticleSystem>();
            if (particle != null)
            {
                particle.Play(true);
            }
        }

        /// <summary>
        /// 停止播放一个特效
        /// </summary>
        /// <param name="effect">特效对象</param>
        public static void StopEffect(GameObject effect)
        {
            if (effect == null)
                return;
            ParticleSystem particle = effect.GetComponentInChildren<ParticleSystem>();
            if (particle != null)
            {
                particle.Stop(true);
            }
        }

        /// <summary>
        /// 立即停止一个特效的播放
        /// </summary>
        /// <param name="effect">特效对象</param>
        public static void StopEffectImmediate(GameObject effect)
        {
            if (effect == null)
                return;
            ParticleSystem particle = effect.GetComponentInChildren<ParticleSystem>();
            if (particle != null)
            {
                particle.Stop(true);
                particle.Clear(true);
            }
        }

        /// <summary>
        /// 获取当前平台的StreamingAssets路径(目前支持编辑器，windows，Android，IOS平台)
        /// </summary>
        /// <param name="is_www">是否是WWW的路径</param>
        /// <returns></returns>
        public static string StreamingAssetsDataPath(bool is_www)
        {
            string path = "";
            if (is_www)
            {
#if UNITY_EDITOR
                path = "file:///" + Application.streamingAssetsPath + "/";
#elif UNITY_STANDALONE_WIN
                path = "file:///" + Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID
                path = Application.streamingAssetsPath + "/";
#elif UNITY_IPHONE
                path = "file://" + Application.streamingAssetsPath + "/";
#else
                Debug.LogError("Now WWW StreamingAssets Path : Windows/Android/IOS is supported!");
#endif
            }
            else
            {
#if UNITY_EDITOR
                path = Application.streamingAssetsPath + "/";
#elif UNITY_STANDALONE_WIN
                path = Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID
                Debug.LogError("Android Platform is not supported stream StreamingAssets path to read!");
#elif UNITY_IPHONE
                path = Application.streamingAssetsPath + "/";
#else
                Debug.LogError("Now Stream StreamingAssets Path : Windows/IOS is supported!");
#endif
            }
            return path;
        }

        /// <summary>
        /// 获取当前平台的PersistentDataPath路径（目前只支持Windows，Android，IOS平台）
        /// </summary>
        /// <param name="is_www">是否使用WWW协议去读取</param>
        /// <returns></returns>
        public static string PersistentDataPath(bool is_www)
        {
            string path = "";
            if (is_www)
            {
#if UNITY_EDITOR
                path = "file:///" + Application.persistentDataPath + "/";
#elif UNITY_STANDALONE_WIN
                path = "file:///" + Application.persistentDataPath + "/";
#elif UNITY_ANDROID
                path = "file://" + Application.persistentDataPath + "/";
#elif UNITY_IPHONE
                path = "file://" + Application.persistentDataPath + "/";
#else
                Debug.LogError("Now WWW PersistentDataPath : Windows/Android/IOS is supported!");
#endif
            }
            else
            {
#if UNITY_EDITOR
                path = Application.persistentDataPath + "/";
#elif UNITY_STANDALONE_WIN
                path = Application.persistentDataPath + "/";
#elif UNITY_ANDROID
                path = Application.persistentDataPath + "/";
#elif UNITY_IPHONE
                path = Application.persistentDataPath + "/";
#else
                Debug.LogError("Now stream PersistentDataPath : Windows/Android/IOS is supported!");
#endif
            }
            return path;
        }

        private static List<System.Action> IsLoadingRes = new List<System.Action>();
        private static bool IsLoadingAssetBundle = false;
        ///<summary>
        ///根据相对路径加载一个对象
        ///</summary>
        ///<param name = "path">相对路径</param>
        ///<returns>返回这个对象</returns>
        public static void LoadResources<T>(string path, System.Action<T> callback) where T : Object
        {
            if (callback != null)
            {
                if (King.TurnBasedCombat.SystemSetting.UseResourcesPathOrStreamingAssetsPath)
                {
                    callback(Resources.Load<T>(path));
                }
                else
                {
                    IsLoadingRes.Add(() =>
                    {
                        King.TurnBasedCombat.BattleController.Instance.StartCoroutine(LoadWWW<T>(path, callback));
                    });
                    if (IsLoadingRes.Count > 0 && !IsLoadingAssetBundle)
                    {
                        IsLoadingRes[0]();
                    }
                }
            }
        }


        static IEnumerator LoadWWW<T>(string path, System.Action<T> callback) where T : Object
        {
            IsLoadingAssetBundle = true;
            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(StreamingAssetsDataPath(true) + King.TurnBasedCombat.SystemSetting.AssetBundleName);
            yield return www;
            if (string.IsNullOrEmpty(www.error) && www.result == UnityWebRequest.Result.Success)
            {
                if (callback != null)
                {
                    AssetBundle ab = DownloadHandlerAssetBundle.GetContent(www);
                    if (ab != null)
                    {
                        string[] str = path.Split('/');
                        if (str.Length > 1)
                        {
                            callback(ab.LoadAsset<T>(str[str.Length - 1]));
                        }
                        ab.Unload(false);
                    }
                    if (IsLoadingRes.Count > 0)
                    {
                        IsLoadingRes.RemoveAt(0);
                    }
                    IsLoadingAssetBundle = false;
                    //检查是否需要继续加载
                    if (IsLoadingRes.Count > 0)
                    {
                        if (IsLoadingRes[0] != null)
                        {
                            IsLoadingRes[0]();
                        }
                    }
                }
            }
            else
            {
                if (IsLoadingRes.Count > 0)
                {
                    IsLoadingRes.RemoveAt(0);
                }
                IsLoadingAssetBundle = false;
                //检查是否需要继续加载
                if (IsLoadingRes.Count > 0)
                {
                    if (IsLoadingRes[0] != null)
                    {
                        IsLoadingRes[0]();
                    }
                }
                Debug.LogError("Load Resource " + path + " failed !" + " Reason:" + www.error);
            }
        }


        /// <summary>
        /// 输入一个Excel表格转成的TXT文本，然后解析成string[]的List存储
        /// </summary>
        /// <param name="content">TXT文本内容</param>
        /// <param name="need_head">是否需要第一行的表头数据</param>
        /// <returns></returns>
        public static List<string[]> GetTxtFileRowString(string content, bool need_head = false)
        {
            List<string[]> list = new List<string[]>();
            string[] rows = content.Trim().Replace("\r", "").Split('\n');
            for (int i = 0; i < rows.Length; i++)
            {
                if (i == 0 && !need_head)
                    continue;
                string[] colums = rows[i].Split('\t');
                list.Add(colums);
            }
            return list;
        }
    }
}