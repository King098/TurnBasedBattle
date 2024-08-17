using System;
using AEventManager;
using AUIFramework;
using UnityEngine;


public static class FunsEx
{
    //是否已经添加过EventHandler了
    public static bool HasAddEventHandler(this EventHandler handler, EventHandler checkHandler)
    {
        var handlers = handler?.GetInvocationList();
        if (handlers != null)
        {
            for (int i = 0; i < handlers.Length; i++)
            {
                if (Equals(handlers[i], checkHandler))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // /// <summary>
    // /// 广播一个事件
    // /// </summary>
    // /// <param name="sender">事件发送者</param>
    // /// <param name="eventName">事件名称</param>
    // /// <param name="args">事件参数</param>
    // public static void TriggerEvent(this object sender,string eventName,params object[] args) {
    //     EventManager.Instance.TriggerEventWithSender(eventName, sender,args);
    // }

    // /// <summary>
    // /// 广播一个无参数事件
    // /// </summary>
    // /// <param name="sender">事件发送者</param>
    // /// <param name="eventName">事件名称</param>
    // public static void TriggerEvent(this object sender,string eventName) {
    //     EventManager.Instance.TriggerEventWithSender(eventName, sender);
    // }

    /// <summary>
    /// 广播一个事件
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="eventName">事件名称</param>
    /// <param name="args">事件参数</param>
    public static void TriggerEvent(this object sender, string eventName, EventArgs args)
    {
        EventManager.Instance.TriggerEventWithSender(eventName, sender, args);
    }

    /// <summary>
    /// 广播一个无参数事件
    /// </summary>
    /// <param name="sender">事件发送者</param>
    /// <param name="eventName">事件名称</param>
    public static void TriggerEvent(this object sender, string eventName)
    {
        EventManager.Instance.TriggerEventWithSender(eventName, sender);
    }
}
