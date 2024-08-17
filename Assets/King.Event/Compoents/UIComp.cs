using System;
using System.Collections.Generic;
using AEventManager;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace AUIFramework
{
    public class UIComp : MonoBehaviour
    {
        //所有组件对应的资源加载句柄缓存
        protected internal Dictionary<Component,AsyncOperationHandle> loadHandlers = new Dictionary<Component,AsyncOperationHandle>();

        //卸载当前UI使用的所有的加载
        protected virtual void ReleaseAllAsyncOperationHandle()
        {
            foreach(var handle in loadHandlers.Values)
            {
                ResourcesManager.ReleaseHandle(handle);
            }
            loadHandlers.Clear();
        }

        protected void ReleaseCompHandle<T>(T comp) where T : Component
        {
            if(loadHandlers.TryGetValue(comp, out var handle))
            {
                ResourcesManager.ReleaseHandle(handle);
                loadHandlers.Remove(comp);
            }
        }

        protected void AddCompHandle<T>(T comp,AsyncOperationHandle handle) where T : Component
        {
            if(!loadHandlers.ContainsKey(comp))
            {
                loadHandlers[comp] = handle;
            }
            else
            {
                loadHandlers.Add(comp, handle);
            }
        }

        protected Dictionary<string,EventHandler> events = new Dictionary<string, EventHandler>();
        protected List<string> scheduleList = new List<string>();

        protected string regSchedule(Action<TimerNode,object[]> callback,float interval,float delay,int repeat,params object[] param)
        {
            string id = TimerManager.Instance.AddTimer(callback,delay,repeat,interval,1,this.GetInstanceID().ToString(),param);
            this.scheduleList.Add(id);
            return id;
        }

        protected string regScheduleOnce(Action<TimerNode,object[]> callback,float delay,params object[] param)
        {
            return this.regSchedule(callback,0f,delay,1,param);
        }

        protected void unregSchedule(string id)
        {
            TimerManager.Instance.RemoveTimerById(id);
            this.scheduleList.Remove(id);
        }

        protected void closeAllSchedule()
        {
            TimerManager.Instance.RemoveTimerByTag(this.GetInstanceID().ToString());
            this.scheduleList.Clear();
        }

        protected void registerEvent(string eventName,EventHandler handle)
        {
            if(!this.events.ContainsKey(eventName))
            {
                this.events.Add(eventName,handle);
            }
            else
            {
                EventManager.Instance.RemoveEvent(eventName,this.events[eventName]);
                this.events[eventName] = handle;
            }
            EventManager.Instance.AddEvent(eventName,handle);
        }

        protected void unregisterAllEvent()
        {
            foreach(var param in this.events)
            {
                EventManager.Instance.RemoveEvent(param.Key,this.events[param.Key]);
            }
            this.events.Clear();
        }

        public void PlaySound(string sound)
        {
            // Tools.PlaySound(sound);
        }

        public void PlayMusic(string music)
        {
            // Tools.PlaySound(music);
        }

        protected virtual void OnDisable()
        {
            ReleaseAllAsyncOperationHandle();
        }

        public void SetSprite(Image img,string path,bool setNativeSize = false)
        {
            if(img == null)
            {
                return;
            }
            if(string.IsNullOrEmpty(path))
            {
                return;
            }
            ResourcesManager.LoadAsync<Sprite>(path,(handle,sp)=>{
                if(img == null)
                {
                    ResourcesManager.ReleaseHandle(handle);
                    return;
                }
                ReleaseCompHandle<Image>(img);
                AddCompHandle<Image>(img,handle);
                if(img == null)
                {
                    return;
                }
                img.sprite = sp;
                if(setNativeSize)
                {
                    img.SetNativeSize();
                }
            });
        }
    }
}