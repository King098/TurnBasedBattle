using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AEventManager {
    /// <summary>
    /// 事件管理系统
    /// </summary>
    public class EventManager {
        private static EventManager _instance = null;
        public static EventManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new EventManager();
                }
                return _instance;
            }
        }

        #region 使用EventHandler和EventArgs实现的事件管理，每种不同的EventArgs都需要单独实现一个类型，不灵活，但是有代码提示
        private Dictionary<string, EventHandler> handlers = new Dictionary<string, EventHandler>();
        private Dictionary<string,EventHandler> once_handlers = new Dictionary<string, EventHandler>();

        /// <summary>
        /// 注册一个事件监听
        /// </summary>
        /// <param name="eventName">事件名字</param>
        /// <param name="handler">事件句柄</param>
        public void AddEvent(string eventName, EventHandler handler) {
            if (handlers.ContainsKey(eventName)) {
                if (!handlers[eventName].HasAddEventHandler(handler)) {
                    handlers[eventName] += handler;
                }
            } else {
                handlers.Add(eventName, handler);
            }
        }

        /// <summary>
        /// 添加一个只触发一次的事件监听，触发之后自动移除
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="handler">事件句柄</param>
        public void AddEventOnce(string eventName,EventHandler handler)
        {
            if(once_handlers.ContainsKey(eventName))
            {
                if(!once_handlers[eventName].HasAddEventHandler(handler)){
                    once_handlers[eventName] += handler;
                }
            }
            else
            {
                once_handlers.Add(eventName,handler);
            }
        }

        /// <summary>
        /// 移除一个事件监听
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="handler">事件句柄</param>
        public void RemoveOnceEvent(string eventName,EventHandler handler)
        {
            if(once_handlers.ContainsKey(eventName))
            {
                once_handlers[eventName] -= handler;
            }
        }

        /// <summary>
        /// 移除一个事件监听
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="handler">事件句柄</param>
        public void RemoveEvent(string eventName,EventHandler handler) {
            if(handlers.ContainsKey(eventName)) {
                handlers[eventName] -= handler;
            }
        }

        /// <summary>
        /// 根据事件名称移除所有注册的事件句柄
        /// </summary>
        /// <param name="eventName">事件名称</param>
        public void RemoveOnceEventsByName(string eventName)
        {
            if(once_handlers.ContainsKey(eventName))
            {
                once_handlers.Remove(eventName);
            }
        }

        /// <summary>
        /// 根据事件名称移除所有注册的事件句柄
        /// </summary>
        /// <param name="eventName">事件名称</param>
        public void RemoveEventsByName(string eventName) {
            if (handlers.ContainsKey(eventName)) {
                handlers.Remove(eventName);
            }
        }

        /// <summary>
        /// 移除管理器注册的所有事件句柄
        /// </summary>
        public void RemoveAllEvents() {
            once_handlers.Clear();
            handlers.Clear();
        }

        /// <summary>
        /// 广播一个事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="args">事件参数</param>
        /// <param name="sender">事件发送对象</param>
        public void TriggerEventWithSender(string eventName,object sender,EventArgs args) {
            if(once_handlers.ContainsKey(eventName))
            {
                once_handlers[eventName]?.Invoke(sender,args);
                RemoveOnceEventsByName(eventName);
            }
            if(handlers.ContainsKey(eventName)) {
                handlers[eventName]?.Invoke(sender, args);
            }
        }

        /// <summary>
        /// 广播一个全局事件，事件发送者设定为事件管理器本身
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="args">事件参数</param>
        public void TriggerEvent(string eventName,EventArgs args) {
            this.TriggerEventWithSender(eventName,this, args);
        }

        /// <summary>
        /// 广播一个无参数事件
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="sender">事件发送者</param>
        public void TriggerEventWithSender(string eventName, object sender) {
            this.TriggerEventWithSender(eventName,sender, EventArgs.Empty);
        }

        /// <summary>
        /// 广播一个全局无参数事件，事件发送者设定为事件管理器本身
        /// </summary>
        /// <param name="eventName"></param>
        public void TriggerEvent(string eventName) {
            this.TriggerEventWithSender(eventName, this);
        }
        #endregion

        #region 使用Action<object,object[]>实现事件管理器，使用了变参的形式，所以参数不需要额外的类型处理，但是没有代码高亮提示，依靠代码注释
        // private Dictionary<string,List<Action<object,object[]>>> handlers = new Dictionary<string, List<Action<object,object[]>>>();


        // public void AddEvent(string eventName,Action<object,object[]> action)
        // {
        //     if(handlers.ContainsKey(eventName))
        //     {
        //         if(!handlers[eventName].Contains(action))
        //         {
        //             handlers[eventName].Add(action);
        //         }
        //     }
        //     else
        //     {
        //         handlers.Add(eventName,new List<Action<object,object[]>>(){action});
        //     }
        // }

        // public void RemoveEvent(string eventName,Action<object,object[]> action)
        // {
        //     if(handlers.ContainsKey(eventName))
        //     {
        //         if(handlers[eventName].Contains(action))
        //         {
        //             handlers[eventName].Remove(action);
        //         }
        //     }
        // }

        // public void RemoveEventsByName(string eventName)
        // {
        //     if(handlers.ContainsKey(eventName))
        //     {
        //         handlers[eventName].Clear();
        //     }
        // }

        // public void RemoveAllEvents()
        // {
        //     handlers.Clear();
        // }

        // public void TriggerEventWithSender(string eventName,object sender,params object[] args)
        // {
        //     if(handlers.ContainsKey(eventName))
        //     {
        //         var list = handlers[eventName];
        //         for(int i = 0;i<list.Count;i++)
        //         {
        //             list[i].Invoke(sender,args);
        //         }
        //     }
        // }

        // public void TriggerEventWithSender(string eventName,object sender)
        // {
        //     this.TriggerEventWithSender(eventName,sender,null);
        // }

        // public void TriggerEvent(string eventName,params object[] args)
        // {
        //     this.TriggerEventWithSender(eventName,this,args);
        // }

        // public void TriggerEvent(string eventName)
        // {
        //     this.TriggerEvent(eventName,null);
        // }
        #endregion
    }

}