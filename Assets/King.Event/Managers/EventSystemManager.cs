using UnityEngine;
using UnityEngine.EventSystems;

namespace AUIFramework
{
    public class EventSystemManager : MonoBehaviour
    {
        private static EventSystem _instance = null;
        public static EventSystem Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = FindObjectOfType<EventSystem>();
                    _instance.enabled = Interactable;
                }
                return _instance;
            }
        }

        private static bool interactable;
        public static bool Interactable
        {
            get
            {
                return  interactable;
            }
            set
            {
                interactable = value;
                // Instance.enabled = interactable;
            }
        }
    }
}